using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KayuSushi.Models
{
    public class Client
    {
        [Key]
        public int IdClient { get; set; }

        [Required]
        public string Nom { get; set; } = string.Empty;

        [Required]
        public string Prenom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Telephone { get; set; }

        public string? Adresse { get; set; }

        [Required]
        public string MotDePasse { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Client";

        public ICollection<Commande> Commandes { get; set; } = new List<Commande>();

        public ICollection<Panier> Paniers { get; set; } = new List<Panier>();
    }
}
