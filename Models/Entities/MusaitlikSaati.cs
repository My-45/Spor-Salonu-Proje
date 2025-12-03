using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SporSalonu.Models.Entities;
// MusaitlikSaati Entity
public class MusaitlikSaati
{
    public int Id { get; set; }

    public int AntrenorId { get; set; }
    public  Antrenor? Antrenor { get; set; }

    [Required]
    public DayOfWeek Gun { get; set; }

    public TimeSpan BaslangicSaati { get; set; }
    public TimeSpan BitisSaati { get; set; }

    public bool Aktif { get; set; } = true;
}
