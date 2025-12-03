using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SporSalonu.Models.Entities;
    public class AntrenorUzmanlik
{
    public int AntrenorId { get; set; }
    public Antrenor? Antrenor { get; set; }

    public int UzmanlikAlaniId { get; set; }
    public  UzmanlikAlani? UzmanlikAlani { get; set; }    
}
