using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;

namespace SporSalonu.Areas.Admin.Controllers
{
    public class YorumAdminController : AdminBaseController
    {
        public YorumAdminController(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var yorumlar = await _context.Yorumlar
                .Include(y => y.Uye)
                .OrderByDescending(y => y.OlusturmaTarihi)
                .ToListAsync();

            return View(yorumlar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onayla(int id)
        {
            var yorum = await _context.Yorumlar.FindAsync(id);

            if (yorum == null)
            {
                TempData["Error"] = "Yorum bulunamadı!";
                return RedirectToAction(nameof(Index));
            }

            yorum.Onaylandi = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Yorum başarıyla onaylandı ve ana sayfada yayınlandı!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reddet(int id)
        {
            var yorum = await _context.Yorumlar.FindAsync(id);

            if (yorum == null)
            {
                TempData["Error"] = "Yorum bulunamadı!";
                return RedirectToAction(nameof(Index));
            }

            _context.Yorumlar.Remove(yorum);
            await _context.SaveChangesAsync();

            TempData["Success"] = "🗑️ Yorum başarıyla silindi!";
            return RedirectToAction(nameof(Index));
        }
    }
}