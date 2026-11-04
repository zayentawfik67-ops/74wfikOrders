using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KayuSushi.Models
{
    public class Panier
    {
        [Key]
        public int IdPanier { get; set; }

        public int IdClient { get; set; }

        [ForeignKey("IdClient")]
        public Client? Client { get; set; }

        public int IdArticle { get; set; }

        [ForeignKey("IdArticle")]
        public Article? Article { get; set; }

        public int Quantite { get; set; }
    }
}
