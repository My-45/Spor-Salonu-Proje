using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;
using SporSalonu.Models.Entities;

namespace SporSalonu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Randevu
        public async Task<IActionResult> Index(string durum = "")
        {
            var query = _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                    .ThenInclude(a => a.Salon)
                .Include(r => r.HizmetTuru)
                .AsQueryable();

            // Duruma göre filtrele
            if (!string.IsNullOrEmpty(durum))
            {
                if (Enum.TryParse<RandevuDurumu>(durum, out var durumEnum))
                {
                    query = query.Where(r => r.Durum == durumEnum);
                }
            }

            var randevular = await query
                .OrderByDescending(r => r.OlusturmaTarihi)
                .ToListAsync();

            // İstatistikler
            ViewBag.ToplamRandevu = await _context.Randevular.CountAsync();
            ViewBag.BekleyenRandevu = await _context.Randevular
                .CountAsync(r => r.Durum == RandevuDurumu.Beklemede);
            ViewBag.OnaylananRandevu = await _context.Randevular
                .CountAsync(r => r.Durum == RandevuDurumu.Onaylandi);
            ViewBag.TamamlananRandevu = await _context.Randevular
                .CountAsync(r => r.Durum == RandevuDurumu.Tamamlandi);
            ViewBag.IptalEdilenRandevu = await _context.Randevular
                .CountAsync(r => r.Durum == RandevuDurumu.IptalEdildi);

            ViewBag.AktifFiltre = durum;

            return View(randevular);
        }

        // GET: Admin/Randevu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                    .ThenInclude(a => a.Salon)
                .Include(r => r.HizmetTuru)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        // POST: Admin/Randevu/Onayla/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            if (randevu.Durum != RandevuDurumu.Beklemede)
            {
                TempData["Error"] = "Sadece beklemedeki randevular onaylanabilir!";
                return RedirectToAction(nameof(Index));
            }

            randevu.Durum = RandevuDurumu.Onaylandi;
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Randevu başarıyla onaylandı!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Randevu/Reddet/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reddet(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            if (randevu.Durum != RandevuDurumu.Beklemede)
            {
                TempData["Error"] = "Sadece beklemedeki randevular reddedilebilir!";
                return RedirectToAction(nameof(Index));
            }

            randevu.Durum = RandevuDurumu.IptalEdildi;
            await _context.SaveChangesAsync();

            TempData["Success"] = "❌ Randevu reddedildi!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Randevu/Tamamla/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tamamla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }

            if (randevu.Durum != RandevuDurumu.Onaylandi)
            {
                TempData["Error"] = "Sadece onaylanmış randevular tamamlanabilir!";
                return RedirectToAction(nameof(Index));
            }

            randevu.Durum = RandevuDurumu.Tamamlandi;
            await _context.SaveChangesAsync();

            TempData["Success"] = "✔️ Randevu tamamlandı olarak işaretlendi!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Randevu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Bugünkü Randevular (Hızlı Erişim)
        public async Task<IActionResult> Bugun()
        {
            var bugun = DateTime.Today;
            var randevular = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                    .ThenInclude(a => a.Salon)
                .Include(r => r.HizmetTuru)
                .Where(r => r.RandevuTarihi.Date == bugun)
                .Where(r => r.Durum != RandevuDurumu.IptalEdildi)
                .OrderBy(r => r.RandevuSaati)
                .ToListAsync();

            ViewBag.Tarih = bugun;
            return View(randevular);
        }
    }
}