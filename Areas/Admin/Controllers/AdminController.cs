using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;

namespace SporSalonu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // İstatistikler
            ViewBag.SalonSayisi = await _context.Salonlar.CountAsync();
            ViewBag.AntrenorSayisi = await _context.Antrenorler.CountAsync();
            ViewBag.UyeSayisi = await _context.Users.CountAsync();
            ViewBag.RandevuSayisi = await _context.Randevular.CountAsync();
            ViewBag.BekleyenRandevuSayisi = await _context.Randevular
                .Where(r => r.Durum == Models.Entities.RandevuDurumu.Beklemede)
                .CountAsync();

            // Son randevular
            ViewBag.SonRandevular = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.HizmetTuru)
                .OrderByDescending(r => r.OlusturmaTarihi)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}
