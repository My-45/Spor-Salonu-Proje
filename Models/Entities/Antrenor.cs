using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SporSalonu.Models.Entities;
    public class Antrenor
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? AdSoyad { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Telefon { get; set; }

    [StringLength(500)]
    public string? Biyografi { get; set; }

    public bool Aktif { get; set; } = true;

    public int SalonId { get; set; }
    public  Salon? Salon { get; set; }

    public ICollection<AntrenorUzmanlik> AntrenorUzmanliklar { get; set; } = new List<AntrenorUzmanlik>();
    public  ICollection<MusaitlikSaati> MusaitlikSaatleri { get; set; } = new List<MusaitlikSaati>();
    public  ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
}
