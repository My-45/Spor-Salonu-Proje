using System.ComponentModel.DataAnnotations;

namespace SporSalonu.Models.Entities
{
    public class Yorum
    {
        public int Id { get; set; }

        [Required]
        public required string UyeId { get; set; }
        public virtual User? Uye { get; set; }

        [Required(ErrorMessage = "Yorum metni zorunludur")]
        [StringLength(500, ErrorMessage = "Yorum en fazla 500 karakter olabilir")]
        public required string Icerik { get; set; }

        [Range(1, 5, ErrorMessage = "Puan 1-5 arasında olmalıdır")]
        public int Puan { get; set; } = 5;

        public bool Onaylandi { get; set; } = false;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    }
}