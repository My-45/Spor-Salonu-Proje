using System.ComponentModel.DataAnnotations;

namespace SporSalonu.Models.ViewModels
{
    public class SalonViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Salon adı zorunludur")]
        [StringLength(100)]
        [Display(Name = "Salon Adı")]
        public string? Ad { get; set; }

        [StringLength(200)]
        [Display(Name = "Adres")]
        public string? Adres { get; set; }

        [Required(ErrorMessage = "Açılış saati zorunludur")]
        [Display(Name = "Açılış Saati")]
        public string? AcilisSaati { get; set; } 

        [Required(ErrorMessage = "Kapanış saati zorunludur")]
        [Display(Name = "Kapanış Saati")]
        public string? KapanisSaati { get; set; } 

        [Display(Name = "Aktif")]
        public bool Aktif { get; set; } = true;
    }
}