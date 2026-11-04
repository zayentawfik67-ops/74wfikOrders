using KayuSushi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KayuSushi.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsConnected()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Client" || role == "Admin";
        }

        private IActionResult? RedirectIfNotConnected()
        {
            if (!IsConnected())
            {
                return RedirectToAction("Login", "Account");
            }

            return null;
        }

        public async Task<IActionResult> Index()
        {
            var redirect = RedirectIfNotConnected();
            if (redirect != null) return redirect;

            var articles = await _context.Articles
                .Include(a => a.Categorie)
                .Where(a => a.Disponible)
                .ToListAsync();

            ViewBag.AllCategories = await _context.Categories
                .Select(c => c.NomCategorie)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return View(articles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var redirect = RedirectIfNotConnected();
            if (redirect != null) return redirect;

            var article = await _context.Articles
                .Include(a => a.Categorie)
                .FirstOrDefaultAsync(a => a.IdArticle == id && a.Disponible);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }
    }
}
