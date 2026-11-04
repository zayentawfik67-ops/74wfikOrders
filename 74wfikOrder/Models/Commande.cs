using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KayuSushi.Models
{
    public class Commande
    {
        [Key]
        public int IdCommande { get; set; }

        public DateTime DateCommande { get; set; } = DateTime.Now;

        public decimal Total { get; set; }

        [Required]
        public string Statut { get; set; } = "En attente";

        public int IdClient { get; set; }

        [ForeignKey("IdClient")]
        public Client? Client { get; set; }

        [Required]
        public string AdresseLivraison { get; set; } = string.Empty;

        [Required]
        public string VilleLivraison { get; set; } = string.Empty;

        [Required]
        public string TelephoneLivraison { get; set; } = string.Empty;

        public string? Remarques { get; set; }

        public double? DeliveryLatitude { get; set; }
        public double? DeliveryLongitude { get; set; }

        public ICollection<LigneCommande> LignesCommande { get; set; } = new List<LigneCommande>();
    }
}
