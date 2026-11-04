using System.ComponentModel.DataAnnotations;

namespace KayuSushi.Models
{
    public class Fournisseur
    {
        [Key]
        public int IdFournisseur { get; set; }

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Telephone { get; set; } = string.Empty;

        public ICollection<CommandeFournisseur> CommandesFournisseur { get; set; } = new List<CommandeFournisseur>();
    }
}
