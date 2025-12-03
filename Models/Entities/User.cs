using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SporSalonu.Models.Entities
{
    // User Entity
    public class User : IdentityUser
    {
        public required string AdSoyad { get; set; }
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        public  ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
    }
}