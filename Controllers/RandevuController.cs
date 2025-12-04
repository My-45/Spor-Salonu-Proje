using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;
using SporSalonu.Models.Entities;
using SporSalonu.Models.ViewModels;

namespace SporSalonu.Controllers
{
    [Authorize(Roles = "Uye")]
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public RandevuController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Randevu/RandevuAl
        public async Task<IActionResult> RandevuAl()
        {
            // Salonları getir
            ViewBag.Salonlar = new SelectList(
                await _context.Salonlar.Where(s => s.Aktif).ToListAsync(),
                "Id", "Ad"
            );

            var model = new RandevuAlViewModel
            {
                RandevuTarihi = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd")
            };

            return View(model);
        }

        // AJAX: Salona göre antrenörleri getir
        [HttpGet]
        public async Task<JsonResult> GetAntrenorler(int salonId)
        {
            var antrenorler = await _context.Antrenorler
                .Where(a => a.SalonId == salonId && a.Aktif)
                .Select(a => new { a.Id, a.AdSoyad })
                .ToListAsync();

            return Json(antrenorler);
        }

        // AJAX: Antrenöre göre hizmetleri getir
        [HttpGet]
        public async Task<JsonResult> GetHizmetler(int salonId)
        {
            var hizmetler = await _context.HizmetTurleri
                .Where(h => h.SalonId == salonId && h.Aktif)
                .Select(h => new { h.Id, h.Ad, h.Sure, h.Ucret })
                .ToListAsync();

            return Json(hizmetler);
        }

        // AJAX: Müsait saatleri getir
        [HttpGet]
        public async Task<JsonResult> GetMusaitSaatler(int antrenorId, string tarih)
        {
            var randevuTarihi = DateTime.Parse(tarih);
            var gun = randevuTarihi.DayOfWeek;

            // Antrenörün o gün için müsaitliği var mı? kontrol 
            var musaitlik = await _context.MusaitlikSaatleri
                .FirstOrDefaultAsync(m => m.AntrenorId == antrenorId && m.Gun == gun && m.Aktif);

            if (musaitlik == null)
            {
                return Json(new { success = false, message = "Antrenör bu gün çalışmıyor" });
            }

            // O gün için mevcut randevuları getir
            var mevcutRandevular = await _context.Randevular
                .Where(r => r.AntrenorId == antrenorId &&
                           r.RandevuTarihi.Date == randevuTarihi.Date &&
                           r.Durum != RandevuDurumu.IptalEdildi)
                .Select(r => r.RandevuSaati)
                .ToListAsync();

            // Müsait saatleri oluştur 
            var musaitSaatler = new List<string>();
            var baslangic = musaitlik.BaslangicSaati;
            var bitis = musaitlik.BitisSaati;

            for (var saat = baslangic; saat < bitis; saat = saat.Add(TimeSpan.FromHours(1)))
            {
                if (!mevcutRandevular.Contains(saat))
                {
                    musaitSaatler.Add(saat.ToString(@"hh\:mm"));
                }
            }

            return Json(new { success = true, saatler = musaitSaatler });
        }

        // POST: Randevu/RandevuAl
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandevuAl(RandevuAlViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var randevu = new Randevu
                {
                    UyeId = user.Id,
                    AntrenorId = model.AntrenorId,
                    HizmetTuruId = model.HizmetTuruId,
                    RandevuTarihi = DateTime.Parse(model.RandevuTarihi),
                    RandevuSaati = TimeSpan.Parse(model.RandevuSaati + ":00"),
                    Durum = RandevuDurumu.Beklemede,
                    OlusturmaTarihi = DateTime.Now
                };

                _context.Randevular.Add(randevu);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Randevunuz başarıyla oluşturuldu! Onay bekliyor.";
                return RedirectToAction("Randevularim");
            }

            
            ViewBag.Salonlar = new SelectList(
                await _context.Salonlar.Where(s => s.Aktif).ToListAsync(),
                "Id", "Ad"
            );

            return View(model);
        }

        // GET: Randevu/Randevularim
        public async Task<IActionResult> Randevularim()
        {
            var user = await _userManager.GetUserAsync(User);

            var randevular = await _context.Randevular
                .Include(r => r.Antrenor)
                    .ThenInclude(a => a.Salon)
                .Include(r => r.HizmetTuru)
                .Where(r => r.UyeId == user.Id)
                .OrderByDescending(r => r.RandevuTarihi)
                .ThenByDescending(r => r.RandevuSaati)
                .ToListAsync();

            return View(randevular);
        }

        // POST: Randevu İptal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var randevu = await _context.Randevular
                .FirstOrDefaultAsync(r => r.Id == id && r.UyeId == user.Id);

            if (randevu == null)
            {
                return NotFound();
            }

           
            if (randevu.Durum == RandevuDurumu.Beklemede ||
                randevu.Durum == RandevuDurumu.Onaylandi)
            {
                randevu.Durum = RandevuDurumu.IptalEdildi;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu başarıyla iptal edildi.";
            }
            else
            {
                TempData["Error"] = "Bu randevu iptal edilemez.";
            }

            return RedirectToAction("Randevularim");
        }
    }
}