using KayuSushi.Data;
using KayuSushi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KayuSushi.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        private IActionResult? RedirectIfNotAdmin()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            return null;
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            var articles = _context.Articles.Include(a => a.Categorie);
            return View(await articles.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Categorie)
                .FirstOrDefaultAsync(m => m.IdArticle == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            ViewData["IdCategorie"] = new SelectList(_context.Categories, "IdCategorie", "NomCategorie");
            return View();
        }

        // POST: Articles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdArticle,NomArticle,Description,Prix,ImageUrl,Disponible,IdCategorie")] Article article)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCategorie"] = new SelectList(_context.Categories, "IdCategorie", "NomCategorie", article.IdCategorie);
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            ViewData["IdCategorie"] = new SelectList(_context.Categories, "IdCategorie", "NomCategorie", article.IdCategorie);
            return View(article);
        }

        // POST: Articles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdArticle,NomArticle,Description,Prix,ImageUrl,Disponible,IdCategorie")] Article article)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            if (id != article.IdArticle)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.IdArticle))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCategorie"] = new SelectList(_context.Categories, "IdCategorie", "NomCategorie", article.IdCategorie);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Categorie)
                .FirstOrDefaultAsync(m => m.IdArticle == id);

            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            var article = await _context.Articles.FindAsync(id);

            if (article != null)
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.IdArticle == id);
        }
    }
}
