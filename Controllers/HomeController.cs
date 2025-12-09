using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;

namespace SporSalonu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Ana sayfa için veriler
            ViewBag.SalonSayisi = await _context.Salonlar.CountAsync();
            ViewBag.AntrenorSayisi = await _context.Antrenorler.CountAsync();
            ViewBag.HizmetSayisi = await _context.HizmetTurleri.CountAsync();

            // Antrenörler 
            ViewBag.Antrenorler = await _context.Antrenorler
                .Include(a => a.Salon)
                .Include(a => a.AntrenorUzmanliklar)
                    .ThenInclude(au => au.UzmanlikAlani)
                .Where(a => a.Aktif)
                .Take(3)
                .ToListAsync();

            // Hizmetler
            ViewBag.Hizmetler = await _context.HizmetTurleri
                .Include(h => h.Salon)
                .Where(h => h.Aktif)
                .Take(6)
                .ToListAsync();

            // YORUMLAR - Onaylanmýþ yorumlarý getir
            ViewBag.Yorumlar = await _context.Yorumlar
                .Include(y => y.Uye)
                .Where(y => y.Onaylandi == true)
                .OrderByDescending(y => y.OlusturmaTarihi)
                .Take(6) // En fazla 6 yorum göster
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}