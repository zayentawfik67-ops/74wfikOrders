using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KayuSushi.Models
{
    public class LigneCommande
    {
        [Key]
        public int IdLigneCommande { get; set; }

        public int IdCommande { get; set; }

        [ForeignKey("IdCommande")]
        public Commande? Commande { get; set; }

        public int IdArticle { get; set; }

        [ForeignKey("IdArticle")]
        public Article? Article { get; set; }

        public int Quantite { get; set; }

        public decimal PrixUnitaire { get; set; }

        public decimal SousTotal { get; set; }
    }
}
