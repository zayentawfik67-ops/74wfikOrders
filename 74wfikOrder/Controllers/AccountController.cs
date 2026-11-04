using KayuSushi.Data;
using KayuSushi.Models;
using KayuSushi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KayuSushi.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var hashedPassword = DbInitializer.HashPassword(model.Password);

            var client = _context.Clients
                .FirstOrDefault(c => c.Email == model.Email && c.MotDePasse == hashedPassword);

            if (client == null)
            {
                ViewBag.ErrorMessage = "Email ou mot de passe incorrect.";
                return View(model);
            }

            HttpContext.Session.SetString("UserRole", client.Role);
            HttpContext.Session.SetString("UserName", $"{client.Prenom} {client.Nom}");
            HttpContext.Session.SetInt32("UserId", client.IdClient);

            if (client.Role == "Admin")
            {
                return RedirectToAction("Admin", "Home");
            }

            return RedirectToAction("Index", "Menu");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            var existingClient = _context.Clients.FirstOrDefault(c => c.Email == model.Email);
            if (existingClient != null)
            {
                ViewBag.ErrorMessage = "Un compte avec cet email existe déjà.";
                return View(model);
            }

            var client = new Client
            {
                Nom = model.Nom, 
                Prenom = model.Prenom,
                Email = model.Email,
                Telephone = model.Telephone,
                MotDePasse = DbInitializer.HashPassword(model.Password),
                Role = "Client"
            };

            _context.Clients.Add(client);
            _context.SaveChanges();

            // Auto-login after registration
            HttpContext.Session.SetString("UserRole", "Client");
            HttpContext.Session.SetString("UserName", $"{client.Prenom} {client.Nom}");
            HttpContext.Session.SetInt32("UserId", client.IdClient);

            return RedirectToAction("Index", "Menu");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
