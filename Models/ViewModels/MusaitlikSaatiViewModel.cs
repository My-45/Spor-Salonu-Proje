using System.ComponentModel.DataAnnotations;

namespace SporSalonu.Models.ViewModels
{
    public class MusaitlikSaatiViewModel
    {
        public int Id { get; set; }

        [Required]
        public int AntrenorId { get; set; }

        [Required(ErrorMessage = "Gün seçmelisiniz")]
        [Display(Name = "Gün")]
        public DayOfWeek Gun { get; set; }

        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        [Display(Name = "Başlangıç Saati")]
        public string? BaslangicSaati { get; set; } // HH:mm formatında

        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        [Display(Name = "Bitiş Saati")]
        public string? BitisSaati { get; set; } // HH:mm formatında

        [Display(Name = "Aktif")]
        public bool Aktif { get; set; } = true;
    }
}