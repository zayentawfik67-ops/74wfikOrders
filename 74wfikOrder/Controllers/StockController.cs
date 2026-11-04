using Microsoft.AspNetCore.Mvc;
using KayuSushi.Data;
using Microsoft.EntityFrameworkCore;
using KayuSushi.Models;

namespace KayuSushi.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var ingredients = await _context.Ingredients
                .Include(i => i.FournisseurDefaut)
                .OrderBy(i => i.Nom)
                .ToListAsync();

            return View(ingredients);
        }

        public async Task<IActionResult> CommandesFournisseur()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var commandes = await _context.CommandesFournisseur
                .Include(cf => cf.Fournisseur)
                .Include(cf => cf.Ingredient)
                .OrderByDescending(cf => cf.DateCommande)
                .ToListAsync();

            return View(commandes);
        }

        [HttpPost]
        public async Task<IActionResult> MarquerReçue(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var commande = await _context.CommandesFournisseur
                .Include(cf => cf.Ingredient)
                .FirstOrDefaultAsync(cf => cf.IdCommandeFournisseur == id);

            if (commande != null && commande.Statut != "Reçue")
            {
                commande.Statut = "Reçue";
                if (commande.Ingredient != null)
                {
                    commande.Ingredient.StockActuel += commande.QuantiteCommandee;
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(CommandesFournisseur));
        }
    }
}
