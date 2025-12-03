namespace SporSalonu.Models.ViewModels
{
    public class UserViewModel
    {
        public required string Id { get; set; }
        public required string AdSoyad { get; set; }
        public required string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime KayitTarihi { get; set; }
        public List<string> Roller { get; set; } = new();
        public int RandevuSayisi { get; set; }
    }
}
