using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SporSalonu.Models.Entities;
     // Randevu Entity
    public class Randevu
{
    public int Id { get; set; }

    public string?   UyeId { get; set; }
    public  User? Uye { get; set; }

    public int? AntrenorId { get; set; }
    public  Antrenor? Antrenor { get; set; }

    public int HizmetTuruId { get; set; }
    public  HizmetTuru? HizmetTuru { get; set; }

    [Required]
    public DateTime RandevuTarihi { get; set; }

    public TimeSpan RandevuSaati { get; set; }

    [StringLength(500)]
    public string? Notlar { get; set; }

    public RandevuDurumu Durum { get; set; } = RandevuDurumu.Beklemede;

    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
}

// Randevu Durumu Enum
public enum RandevuDurumu
{
    Beklemede,
    Onaylandi,
    Tamamlandi,
    IptalEdildi
}
