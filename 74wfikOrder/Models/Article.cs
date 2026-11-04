using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KayuSushi.Models
{
    public class Article
    {
        [Key]
        public int IdArticle { get; set; }

        [Required]
        public string NomArticle { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public decimal Prix { get; set; }

        public string? ImageUrl { get; set; }

        public bool Disponible { get; set; } = true;

        public int IdCategorie { get; set; }

        [ForeignKey("IdCategorie")]
        public Categorie? Categorie { get; set; }

        public ICollection<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();

        public ICollection<Panier> Paniers { get; set; } = new List<Panier>();

        public ICollection<ArticleIngredient> ArticleIngredients { get; set; } = new List<ArticleIngredient>();
    }
}
