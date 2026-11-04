using KayuSushi.Models;
using System.Security.Cryptography;
using System.Text;

namespace KayuSushi.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // --- Seed default users ---
            EnsureUser(context, "Admin", "KayuSushi", "admin@74wfikorder.com", "admin123", "Admin");
            EnsureUser(context, "Client", "Demo", "client@74wfikorder.com", "client123", "Client");

                        var sushis = EnsureCategory(
                context,
                "Sushis & Nigiris",
                "Assortiment de sushis traditionnels"
            );

            var makis = EnsureCategory(
                context,
                "Makis & Rolls",
                "Makis, California rolls et Spring rolls"
            );

            var sashimis = EnsureCategory(
                context,
                "Sashimis",
                "Tranches de poissons crus frais"
            );

            var accompagnements = EnsureCategory(
                context,
                "Accompagnements",
                "Soupes, salades et sauces"
            );

            var boissons = EnsureCategory(
                context,
                "Boissons",
                "Boissons fraîches et thés"
            );

            var desserts = EnsureCategory(
                context,
                "Desserts",
                "Desserts japonais et mochis"
            );

            context.SaveChanges();

            UpsertArticle(context, "Sushi Saumon",
                "Nigiri sushi au saumon frais d'Écosse.",
                6.500m,
                "https://images.unsplash.com/photo-1579871494447-9811cf80d66c?q=80&w=600&auto=format&fit=crop",
                sushis.IdCategorie);

            UpsertArticle(context, "Sushi Thon",
                "Nigiri sushi au thon rouge.",
                7.500m,
                "https://images.unsplash.com/photo-1553621042-f6e147245754?q=80&w=600&auto=format&fit=crop",
                sushis.IdCategorie);

            UpsertArticle(context, "California Roll Saumon Avocat",
                "Rouleau inversé au saumon, avocat et graines de sésame (6 pièces).",
                12.500m,
                "https://images.unsplash.com/photo-1579584425555-c3ce17fd4351?q=80&w=600&auto=format&fit=crop",
                makis.IdCategorie);

            UpsertArticle(context, "Maki Thon Epicé",
                "Maki traditionnel au thon épicé et concombre (6 pièces).",
                11.000m,
                "https://images.unsplash.com/photo-1611143669185-af224c5e3252?q=80&w=600&auto=format&fit=crop",
                makis.IdCategorie);
                
            UpsertArticle(context, "Spring Roll Crevette Tempura",
                "Rouleau de printemps à la crevette tempura, avocat et menthe.",
                14.000m,
                "https://images.unsplash.com/photo-1615361200141-f45040f367be?q=80&w=600&auto=format&fit=crop",
                makis.IdCategorie);

            UpsertArticle(context, "Sashimi Saumon",
                "15 tranches de saumon frais.",
                18.000m,
                "https://images.unsplash.com/photo-1583623025817-d180a2221d0a?q=80&w=600&auto=format&fit=crop",
                sashimis.IdCategorie);

            UpsertArticle(context, "Sashimi Mixte",
                "Assortiment de 15 tranches de saumon, thon et daurade.",
                22.000m,
                "https://images.unsplash.com/photo-1534422298391-e4f8c172dddb?q=80&w=600&auto=format&fit=crop",
                sashimis.IdCategorie);

            UpsertArticle(context, "Soupe Miso",
                "Soupe japonaise traditionnelle au tofu et algues wakame.",
                4.500m,
                "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?q=80&w=600&auto=format&fit=crop",
                accompagnements.IdCategorie);

            UpsertArticle(context, "Salade d'Algues Wakame",
                "Salade d'algues marinées au sésame.",
                6.000m,
                "https://images.unsplash.com/photo-1562158074-678eb1766a5c?q=80&w=600&auto=format&fit=crop",
                accompagnements.IdCategorie);

            UpsertArticle(context, "Boisson Gazeuse",
                "Boisson gazeuse fraîche.",
                3.000m,
                "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?q=80&w=600&auto=format&fit=crop",
                boissons.IdCategorie);

            UpsertArticle(context, "Thé Vert Japonais",
                "Thé vert infusé chaud ou froid.",
                4.000m,
                "https://images.unsplash.com/photo-1558160074-4d7d8bdf4256?q=80&w=600&auto=format&fit=crop",
                boissons.IdCategorie);

            UpsertArticle(context, "Mochi Glacé Mixte",
                "Assortiment de 3 mochis glacés (thé vert, mangue, vanille).",
                8.000m,
                "https://images.unsplash.com/photo-1582329712399-52bb6cfa2b6e?q=80&w=600&auto=format&fit=crop",
                desserts.IdCategorie);

            context.SaveChanges();

            // --- Seed Stock & Fournisseurs ---
            var fournisseur = context.Fournisseurs.FirstOrDefault(f => f.Email == "contact@japon-grossiste.com");
            if (fournisseur == null)
            {
                fournisseur = new Fournisseur
                {
                    Nom = "Japon Grossiste Alimentaire",
                    Email = "contact@japon-grossiste.com",
                    Telephone = "71 000 222"
                };
                context.Fournisseurs.Add(fournisseur);
                context.SaveChanges();
            }

            var riz = EnsureIngredient(context, "Riz à sushi", 50, 15, "kg", fournisseur.IdFournisseur);
            var saumon = EnsureIngredient(context, "Saumon Frais", 20, 5, "kg", fournisseur.IdFournisseur);
            var algue = EnsureIngredient(context, "Algues Nori", 1000, 200, "feuilles", fournisseur.IdFournisseur);
            var avocat = EnsureIngredient(context, "Avocat", 30, 10, "kg", fournisseur.IdFournisseur);

            context.SaveChanges();

            // Link ingredients to Sushi Saumon
            LinkArticleIngredient(context, "Sushi Saumon", riz.IdIngredient, 0.02m);
            LinkArticleIngredient(context, "Sushi Saumon", saumon.IdIngredient, 0.015m);

            // Link ingredients to California Roll Saumon Avocat
            LinkArticleIngredient(context, "California Roll Saumon Avocat", riz.IdIngredient, 0.08m);
            LinkArticleIngredient(context, "California Roll Saumon Avocat", saumon.IdIngredient, 0.04m);
            LinkArticleIngredient(context, "California Roll Saumon Avocat", algue.IdIngredient, 0.5m);
            LinkArticleIngredient(context, "California Roll Saumon Avocat", avocat.IdIngredient, 0.03m);

            context.SaveChanges();
        }

        private static Ingredient EnsureIngredient(ApplicationDbContext context, string nom, decimal stock, decimal seuil, string unite, int idFourn)
        {
            var ing = context.Ingredients.FirstOrDefault(i => i.Nom == nom);
            if (ing == null)
            {
                ing = new Ingredient { Nom = nom, StockActuel = stock, SeuilAlerte = seuil, Unite = unite, IdFournisseurDefaut = idFourn };
                context.Ingredients.Add(ing);
                context.SaveChanges();
            }
            return ing;
        }

        private static void LinkArticleIngredient(ApplicationDbContext context, string articleName, int idIngredient, decimal quantite)
        {
            var article = context.Articles.FirstOrDefault(a => a.NomArticle == articleName);
            if (article != null)
            {
                var link = context.ArticleIngredients.FirstOrDefault(ai => ai.IdArticle == article.IdArticle && ai.IdIngredient == idIngredient);
                if (link == null)
                {
                    context.ArticleIngredients.Add(new ArticleIngredient { IdArticle = article.IdArticle, IdIngredient = idIngredient, QuantiteRequise = quantite });
                }
            }
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static void EnsureUser(
            ApplicationDbContext context,
            string nom,
            string prenom,
            string email,
            string password,
            string role)
        {
            var existing = context.Clients.FirstOrDefault(c => c.Email == email);
            if (existing == null)
            {
                context.Clients.Add(new Client
                {
                    Nom = nom,
                    Prenom = prenom,
                    Email = email,
                    MotDePasse = HashPassword(password),
                    Role = role
                });
                context.SaveChanges();
            }
            else
            {
                existing.Nom = nom;
                existing.Prenom = prenom;
                context.SaveChanges();
            }
        }

        private static Categorie EnsureCategory(
            ApplicationDbContext context,
            string nomCategorie,
            string description)
        {
            var categorie = context.Categories
                .FirstOrDefault(c => c.NomCategorie == nomCategorie);

            if (categorie == null)
            {
                categorie = new Categorie
                {
                    NomCategorie = nomCategorie,
                    Description = description
                };

                context.Categories.Add(categorie);
                context.SaveChanges();
            }
            else
            {
                categorie.Description = description;
            }

            return categorie;
        }

        private static void UpsertArticle(
            ApplicationDbContext context,
            string nomArticle,
            string description,
            decimal prix,
            string imageUrl,
            int idCategorie)
        {
            var article = context.Articles
                .FirstOrDefault(a => a.NomArticle == nomArticle);

            if (article == null)
            {
                article = new Article
                {
                    NomArticle = nomArticle
                };

                context.Articles.Add(article);
            }

            article.Description = description;
            article.Prix = prix;
            article.ImageUrl = imageUrl;
            article.Disponible = true;
            article.IdCategorie = idCategorie;
        }
    }
}
