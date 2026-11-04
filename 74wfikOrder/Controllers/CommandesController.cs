using KayuSushi.Data;
using KayuSushi.Models;
using KayuSushi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KayuSushi.Controllers
{
    public class CommandesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommandesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        private bool IsClient()
        {
            return HttpContext.Session.GetString("UserRole") == "Client";
        }

        private IActionResult? RedirectIfNotConnected()
        {
            if (!IsAdmin() && !IsClient())
            {
                return RedirectToAction("Login", "Account");
            }

            return null;
        }

        private Client GetOrCreateClient()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var client = _context.Clients.FirstOrDefault(c => c.IdClient == userId);
                if (client != null) return client;
            }
            
            // Fallback for debugging
            return _context.Clients.First();
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var commandes = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Article)
                .OrderByDescending(c => c.DateCommande)
                .ToListAsync();

            return View(commandes);
        }

        public async Task<IActionResult> MesCommandes()
        {
            if (!IsClient())
            {
                return RedirectToAction("Login", "Account");
            }

            var client = GetOrCreateClient();

            var commandes = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Article)
                .Where(c => c.IdClient == client.IdClient)
                .OrderByDescending(c => c.DateCommande)
                .ToListAsync();

            return View(commandes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var redirect = RedirectIfNotConnected();
            if (redirect != null) return redirect;

            var commande = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Article)
                .ThenInclude(a => a!.Categorie)
                .FirstOrDefaultAsync(c => c.IdCommande == id);

            if (commande == null)
            {
                return NotFound();
            }

            if (IsClient())
            {
                var client = GetOrCreateClient();

                if (commande.IdClient != client.IdClient)
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            return View(commande);
        }

        public async Task<IActionResult> Tracking(int id)
        {
            var redirect = RedirectIfNotConnected();
            if (redirect != null) return redirect;

            var commande = await _context.Commandes
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.IdCommande == id);

            if (commande == null)
            {
                return NotFound();
            }

            if (IsClient())
            {
                var client = GetOrCreateClient();
                if (commande.IdClient != client.IdClient)
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            return View(commande);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (!IsClient())
            {
                return RedirectToAction("Login", "Account");
            }

            var client = GetOrCreateClient();

            var panier = await _context.Paniers
                .Include(p => p.Article)
                .Where(p => p.IdClient == client.IdClient)
                .ToListAsync();

            if (!panier.Any())
            {
                return RedirectToAction("Index", "Panier");
            }

            var total = panier.Sum(p => p.Article != null ? p.Article.Prix * p.Quantite : 0);

            var model = new CheckoutViewModel
            {
                AdresseLivraison = client.Adresse ?? "",
                TelephoneLivraison = client.Telephone ?? "",
                VilleLivraison = "Sfax", // Default or extract from address if needed
                Total = total,
                ItemsPanier = panier
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!IsClient())
            {
                return RedirectToAction("Login", "Account");
            }

            var client = GetOrCreateClient();

            var panier = await _context.Paniers
                .Include(p => p.Article)
                .Where(p => p.IdClient == client.IdClient)
                .ToListAsync();

            if (!panier.Any())
            {
                return RedirectToAction("Index", "Panier");
            }

            if (!ModelState.IsValid)
            {
                model.Total = panier.Sum(p => p.Article != null ? p.Article.Prix * p.Quantite : 0);
                model.ItemsPanier = panier;
                return View(model);
            }

            var total = panier.Sum(p => p.Article != null ? p.Article.Prix * p.Quantite : 0);

            var commande = new Commande
            {
                DateCommande = DateTime.Now,
                Total = total,
                Statut = "En attente",
                IdClient = client.IdClient,
                AdresseLivraison = model.AdresseLivraison,
                VilleLivraison = model.VilleLivraison,
                TelephoneLivraison = model.TelephoneLivraison,
                Remarques = model.Remarques,
                DeliveryLatitude = model.DeliveryLatitude,
                DeliveryLongitude = model.DeliveryLongitude,
                LignesCommande = new List<LigneCommande>()
            };

            foreach (var item in panier)
            {
                var ligne = new LigneCommande
                {
                    IdArticle = item.IdArticle,
                    Quantite = item.Quantite,
                    PrixUnitaire = item.Article!.Prix,
                    SousTotal = item.Article.Prix * item.Quantite
                };
                commande.LignesCommande.Add(ligne);

                // --- Stock Deduction Logic ---
                // Fetch required ingredients for this article
                var articleIngredients = await _context.ArticleIngredients
                    .Include(ai => ai.Ingredient)
                    .Where(ai => ai.IdArticle == item.IdArticle)
                    .ToListAsync();

                foreach (var ai in articleIngredients)
                {
                    if (ai.Ingredient != null)
                    {
                        // Deduct stock
                        var quantiteADeduire = ai.QuantiteRequise * item.Quantite;
                        ai.Ingredient.StockActuel -= quantiteADeduire;

                        // Check for threshold and trigger auto-order
                        if (ai.Ingredient.StockActuel <= ai.Ingredient.SeuilAlerte)
                        {
                            // Check if an order is already pending
                            var existingOrder = await _context.CommandesFournisseur
                                .FirstOrDefaultAsync(cf => cf.IdIngredient == ai.Ingredient.IdIngredient && cf.Statut == "En attente");

                            if (existingOrder == null)
                            {
                                // Order double the threshold to restock well
                                var quantiteACommander = Math.Max(ai.Ingredient.SeuilAlerte * 2, 10);
                                
                                var commandeFournisseur = new CommandeFournisseur
                                {
                                    IdFournisseur = ai.Ingredient.IdFournisseurDefaut ?? 1, // Fallback to first supplier if none
                                    IdIngredient = ai.Ingredient.IdIngredient,
                                    QuantiteCommandee = quantiteACommander,
                                    DateCommande = DateTime.Now,
                                    Statut = "En attente"
                                };
                                _context.CommandesFournisseur.Add(commandeFournisseur);
                            }
                        }
                        _context.Update(ai.Ingredient);
                    }
                }
            }

            _context.Commandes.Add(commande);
            _context.Paniers.RemoveRange(panier);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmation), new { id = commande.IdCommande });
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            if (!IsClient())
            {
                return RedirectToAction("Login", "Account");
            }

            var client = GetOrCreateClient();

            var commande = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Article)
                .FirstOrDefaultAsync(c => c.IdCommande == id && c.IdClient == client.IdClient);

            if (commande == null)
            {
                return NotFound();
            }

            return View(commande);
        }

        public async Task<IActionResult> ChangerStatut(int id, string statut)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var commande = await _context.Commandes.FindAsync(id);

            if (commande == null)
            {
                return NotFound();
            }

            commande.Statut = statut;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
