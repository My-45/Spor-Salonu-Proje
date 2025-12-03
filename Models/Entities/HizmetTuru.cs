using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SporSalonu.Models.Entities;
    public class HizmetTuru
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? Ad { get; set; }

    [StringLength(300)]
    public string? Aciklama { get; set; }

    public int Sure { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Ucret { get; set; }

    public bool Aktif { get; set; } = true;

    public int SalonId { get; set; }
    public  Salon? Salon { get; set; }

    public  ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
}

