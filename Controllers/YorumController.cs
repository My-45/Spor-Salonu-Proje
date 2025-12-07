using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;
using SporSalonu.Models.Entities;
using SporSalonu.Models.ViewModels;

namespace SporSalonu.Controllers
{
    [Authorize(Roles = "Uye")]
    public class YorumController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public YorumController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Yorum/YorumYap
        public IActionResult YorumYap()
        {
            return View();
        }

        // POST: Yorum/YorumYap
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> YorumYap(YorumViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kullanıcının daha önce yorum yapıp yapmadığını kontrol et
                var mevcutYorum = await _context.Yorumlar
                    .FirstOrDefaultAsync(y => y.UyeId == user.Id);

                if (mevcutYorum != null)
                {
                    TempData["Error"] = "Zaten bir yorumunuz bulunmaktadır. Her kullanıcı sadece bir yorum yapabilir.";
                    return View(model);
                }

                var yorum = new Yorum
                {
                    UyeId = user.Id,
                    Icerik = model.Icerik,
                    Puan = model.Puan,
                    Onaylandi = false, // Admin onayı bekleyecek
                    OlusturmaTarihi = DateTime.Now
                };

                _context.Yorumlar.Add(yorum);
                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Yorumunuz başarıyla gönderildi! Admin onayından sonra yayınlanacaktır.";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: Yorum/Yorumlarim
        public async Task<IActionResult> Yorumlarim()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var yorumlar = await _context.Yorumlar
                .Where(y => y.UyeId == user.Id)
                .OrderByDescending(y => y.OlusturmaTarihi)
                .ToListAsync();

            return View(yorumlar);
        }

        // POST: Yorum/Sil/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sil(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var yorum = await _context.Yorumlar
                .FirstOrDefaultAsync(y => y.Id == id && y.UyeId == user.Id);

            if (yorum == null)
            {
                TempData["Error"] = "Yorum bulunamadı!";
                return RedirectToAction(nameof(Yorumlarim));
            }

            _context.Yorumlar.Remove(yorum);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Yorumunuz başarıyla silindi.";
            return RedirectToAction(nameof(Yorumlarim));
        }
    }
}
