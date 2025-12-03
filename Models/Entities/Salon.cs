using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SporSalonu.Models.Entities;
    // Salon Entity
    public class Salon
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Salon adı zorunludur")]
    [StringLength(100)]
    public string? Ad { get; set; }

    [StringLength(200)]
    public string? Adres { get; set; }

    public TimeSpan AcilisSaati { get; set; }
    public TimeSpan KapanisSaati { get; set; }

    public bool Aktif { get; set; } = true;

    public  ICollection<Antrenor> Antrenorler { get; set; } = new List<Antrenor>();
    public  ICollection<HizmetTuru> HizmetTurleri { get; set; } = new List<HizmetTuru>();    
} 
