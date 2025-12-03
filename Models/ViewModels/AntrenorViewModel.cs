using System.ComponentModel.DataAnnotations;

namespace SporSalonu.Models.ViewModels
{
    public class AntrenorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [Display(Name = "Ad Soyad")]
        public required string AdSoyad { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }

        [Display(Name = "Biyografi")]
        [StringLength(500)]
        public string? Biyografi { get; set; }

        [Display(Name = "Aktif")]
        public bool Aktif { get; set; } = true;

        [Required(ErrorMessage = "Salon seçimi zorunludur")]
        [Display(Name = "Salon")]
        public int SalonId { get; set; }

        [Display(Name = "Uzmanlık Alanları")]
        public List<int> SecilenUzmanliklar { get; set; } = new List<int>();
    }
}