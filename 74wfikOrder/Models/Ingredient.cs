using System.ComponentModel.DataAnnotations;

namespace KayuSushi.Models
{
    public class Ingredient
    {
        [Key]
        public int IdIngredient { get; set; }

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public decimal StockActuel { get; set; }

        [Required]
        public decimal SeuilAlerte { get; set; }

        [Required]
        public string Unite { get; set; } = "kg"; // e.g., kg, L, pieces

        // The default supplier to reorder from
        public int? IdFournisseurDefaut { get; set; }
        public Fournisseur? FournisseurDefaut { get; set; }

        public ICollection<ArticleIngredient> ArticleIngredients { get; set; } = new List<ArticleIngredient>();
    }
}
