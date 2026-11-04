using System.ComponentModel.DataAnnotations;

namespace KayuSushi.Models
{
    public class CommandeFournisseur
    {
        [Key]
        public int IdCommandeFournisseur { get; set; }

        public DateTime DateCommande { get; set; }

        public int IdFournisseur { get; set; }
        public Fournisseur? Fournisseur { get; set; }

        public int IdIngredient { get; set; }
        public Ingredient? Ingredient { get; set; }

        [Required]
        public decimal QuantiteCommandee { get; set; }

        [Required]
        public string Statut { get; set; } = "En attente"; // En attente, Reçue
    }
}
