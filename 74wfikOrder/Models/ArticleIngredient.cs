using System.ComponentModel.DataAnnotations;

namespace KayuSushi.Models
{
    public class ArticleIngredient
    {
        [Key]
        public int IdArticleIngredient { get; set; }

        public int IdArticle { get; set; }
        public Article? Article { get; set; }

        public int IdIngredient { get; set; }
        public Ingredient? Ingredient { get; set; }

        [Required]
        public decimal QuantiteRequise { get; set; }
    }
}
