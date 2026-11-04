using KayuSushi.Data;
using KayuSushi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KayuSushi.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategorieAdoRepository _repository;

        public CategoriesController(CategorieAdoRepository repository)
        {
            _repository = repository;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        private IActionResult RedirectIfNotAdmin()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            return null!;
        }

        public IActionResult Index()
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            var categories = _repository.GetAll();
            return View(categories);
        }

        public IActionResult Details(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            var categorie = _repository.GetById(id);

            if (categorie == null)
            {
                return NotFound();
            }

            return View(categorie);
        }

        public IActionResult Create()
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categorie categorie)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            if (ModelState.IsValid)
            {
                _repository.Create(categorie);
                return RedirectToAction(nameof(Index));
            }

            return View(categorie);
        }

        public IActionResult Edit(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            var categorie = _repository.GetById(id);

            if (categorie == null)
            {
                return NotFound();
            }

            return View(categorie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Categorie categorie)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            if (id != categorie.IdCategorie)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _repository.Update(categorie);
                return RedirectToAction(nameof(Index));
            }

            return View(categorie);
        }

        public IActionResult Delete(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            var categorie = _repository.GetById(id);

            if (categorie == null)
            {
                return NotFound();
            }

            return View(categorie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var redirect = RedirectIfNotAdmin();
            if (redirect != null) return redirect;

            _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
