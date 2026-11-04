using KayuSushi.Models;
using Microsoft.EntityFrameworkCore;

namespace KayuSushi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<LigneCommande> LignesCommande { get; set; }
        public DbSet<Panier> Paniers { get; set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<ArticleIngredient> ArticleIngredients { get; set; }
        public DbSet<CommandeFournisseur> CommandesFournisseur { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArticleIngredient>()
                .HasOne(ai => ai.Article)
                .WithMany(a => a.ArticleIngredients)
                .HasForeignKey(ai => ai.IdArticle);

            modelBuilder.Entity<ArticleIngredient>()
                .HasOne(ai => ai.Ingredient)
                .WithMany(i => i.ArticleIngredients)
                .HasForeignKey(ai => ai.IdIngredient);
        }
    }
}
