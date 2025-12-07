using System.ComponentModel.DataAnnotations;

namespace SporSalonu.Models.ViewModels
{
    public class YorumViewModel
    {
        [Required(ErrorMessage = "Yorum yazmalısınız")]
        [StringLength(500, ErrorMessage = "Yorum en fazla 500 karakter olabilir")]
        [Display(Name = "Yorumunuz")]
        public required string Icerik { get; set; }

        [Required(ErrorMessage = "Puan vermelisiniz")]
        [Range(1, 5, ErrorMessage = "1-5 arası puan verebilirsiniz")]
        [Display(Name = "Puanınız")]
        public int Puan { get; set; } = 5;
    }
}