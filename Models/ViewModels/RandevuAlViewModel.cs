using System.ComponentModel.DataAnnotations;

namespace SporSalonu.Models.ViewModels
{
    public class RandevuAlViewModel
    {
        [Required(ErrorMessage = "Salon seçmelisiniz")]
        [Display(Name = "Salon")]
        public int SalonId { get; set; }

        [Required(ErrorMessage = "Antrenör seçmelisiniz")]
        [Display(Name = "Antrenör")]
        public int AntrenorId { get; set; }

        [Required(ErrorMessage = "Hizmet seçmelisiniz")]
        [Display(Name = "Hizmet")]
        public int HizmetTuruId { get; set; }

        [Required(ErrorMessage = "Tarih seçmelisiniz")]
        [Display(Name = "Randevu Tarihi")]
        public string? RandevuTarihi { get; set; }

        [Required(ErrorMessage = "Saat seçmelisiniz")]
        [Display(Name = "Randevu Saati")]
        public string? RandevuSaati { get; set; }
    }
}