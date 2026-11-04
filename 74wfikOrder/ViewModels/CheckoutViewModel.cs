using System.ComponentModel.DataAnnotations;
using KayuSushi.Models;

namespace KayuSushi.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "L'adresse de livraison est requise")]
        [Display(Name = "Adresse de livraison")]
        public string AdresseLivraison { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ville de livraison est requise")]
        [Display(Name = "Ville")]
        public string VilleLivraison { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le numéro de téléphone est requis")]
        [Phone(ErrorMessage = "Numéro de téléphone invalide")]
        [Display(Name = "Numéro de téléphone")]
        public string TelephoneLivraison { get; set; } = string.Empty;

        [Display(Name = "Remarques (Optionnel)")]
        public string? Remarques { get; set; }

        public double? DeliveryLatitude { get; set; }
        public double? DeliveryLongitude { get; set; }

        // These properties are for display purposes in the checkout view
        [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
        public decimal Total { get; set; }
        
        [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
        public List<Panier> ItemsPanier { get; set; } = new List<Panier>();
    }
}
