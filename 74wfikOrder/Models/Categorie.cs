using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KayuSushi.Models
{
    public class Categorie
    {
        [Key]
        public int IdCategorie { get; set; }

        [Required]
        public string NomCategorie { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
