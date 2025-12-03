using System.ComponentModel.DataAnnotations;

namespace SporSalonu.Models.ViewModels
{
    public class HizmetTuruViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur")]
        [StringLength(100)]
        [Display(Name = "Hizmet Adı")]
        public string? Ad { get; set; }

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Süre zorunludur")]
        [Range(15, 300, ErrorMessage = "Süre 15-300 dakika arasında olmalıdır")]
        [Display(Name = "Süre (Dakika)")]
        public int Sure { get; set; }

        [Required(ErrorMessage = "Ücret zorunludur")]
        [Range(0, 10000, ErrorMessage = "Ücret 0-10000 TL arasında olmalıdır")]
        [Display(Name = "Ücret (TL)")]
        public decimal Ucret { get; set; }

        [Required(ErrorMessage = "Salon seçmelisiniz")]
        [Display(Name = "Salon")]
        public int SalonId { get; set; }

        [Display(Name = "Aktif")]
        public bool Aktif { get; set; } = true;
    }
}