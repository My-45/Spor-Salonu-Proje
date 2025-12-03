using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SporSalonu.Models.Entities;
    public class UzmanlikAlani
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string? Ad { get; set; }

    [StringLength(200)]
    public string? Aciklama { get; set; }

    public  ICollection<AntrenorUzmanlik> AntrenorUzmanliklar { get; set; } = new List<AntrenorUzmanlik>();
}