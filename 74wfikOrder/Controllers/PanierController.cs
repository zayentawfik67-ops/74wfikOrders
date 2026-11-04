using KayuSushi.Data;
using KayuSushi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KayuSushi.Controllers
{
    public class PanierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PanierController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsClient()
        {
            return HttpContext.Session.GetString("UserRole") == "Client";
        }

        private IActionResult? RedirectIfNotClient()
        {
            if (!IsClient())
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
            
            // Fallback for debugging if session is lost but Role is still Client
            return _context.Clients.First();
        }

        public async Task<IActionResult> Index()
        {
            var redirect = RedirectIfNotClient();
            if (redirect != null) return redirect;

            var client = GetOrCreateClient();

            var paniers = await _context.Paniers
                .Include(p => p.Article)
                .ThenInclude(a => a!.Categorie)
                .Where(p => p.IdClient == client.IdClient)
                .ToListAsync();

            ViewBag.Total = paniers.Sum(p => p.Article != null ? p.Article.Prix * p.Quantite : 0);

            return View(paniers);
        }

        public async Task<IActionResult> Ajouter(int id)
        {
            var redirect = RedirectIfNotClient();
            if (redirect != null) return redirect;

            var client = GetOrCreateClient();

            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.IdArticle == id && a.Disponible);

            if (article == null)
            {
                return NotFound();
            }

            var panierItem = await _context.Paniers
                .FirstOrDefaultAsync(p => p.IdClient == client.IdClient && p.IdArticle == id);

            if (panierItem == null)
            {
                panierItem = new Panier
                {
                    IdClient = client.IdClient,
                    IdArticle = id,
                    Quantite = 1
                };

                _context.Paniers.Add(panierItem);
            }
            else
            {
                panierItem.Quantite++;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Augmenter(int id)
        {
            var redirect = RedirectIfNotClient();
            if (redirect != null) return redirect;

            var client = GetOrCreateClient();

            var panierItem = await _context.Paniers
                .FirstOrDefaultAsync(p => p.IdPanier == id && p.IdClient == client.IdClient);

            if (panierItem == null)
            {
                return NotFound();
            }

            panierItem.Quantite++;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Diminuer(int id)
        {
            var redirect = RedirectIfNotClient();
            if (redirect != null) return redirect;

            var client = GetOrCreateClient();

            var panierItem = await _context.Paniers
                .FirstOrDefaultAsync(p => p.IdPanier == id && p.IdClient == client.IdClient);

            if (panierItem == null)
            {
                return NotFound();
            }

            panierItem.Quantite--;

            if (panierItem.Quantite <= 0)
            {
                _context.Paniers.Remove(panierItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Supprimer(int id)
        {
            var redirect = RedirectIfNotClient();
            if (redirect != null) return redirect;

            var client = GetOrCreateClient();

            var panierItem = await _context.Paniers
                .FirstOrDefaultAsync(p => p.IdPanier == id && p.IdClient == client.IdClient);

            if (panierItem == null)
            {
                return NotFound();
            }

            _context.Paniers.Remove(panierItem);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Vider()
        {
            var redirect = RedirectIfNotClient();
            if (redirect != null) return redirect;

            var client = GetOrCreateClient();

            var paniers = await _context.Paniers
                .Where(p => p.IdClient == client.IdClient)
                .ToListAsync();

            _context.Paniers.RemoveRange(paniers);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
