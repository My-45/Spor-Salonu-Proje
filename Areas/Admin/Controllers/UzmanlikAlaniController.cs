using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;
using SporSalonu.Models.Entities;

namespace SporSalonu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UzmanlikAlaniController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UzmanlikAlaniController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/UzmanlikAlani
        public async Task<IActionResult> Index()
        {
            var uzmanliklar = await _context.UzmanlikAlanlari
                .Include(u => u.AntrenorUzmanliklar)
                .ToListAsync();
            return View(uzmanliklar);
        }

        // GET: Admin/UzmanlikAlani/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uzmanlik = await _context.UzmanlikAlanlari
                .Include(u => u.AntrenorUzmanliklar)
                    .ThenInclude(au => au.Antrenor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (uzmanlik == null)
            {
                return NotFound();
            }

            return View(uzmanlik);
        }

        // GET: Admin/UzmanlikAlani/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/UzmanlikAlani/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ad,Aciklama")] UzmanlikAlani uzmanlik)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uzmanlik);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Uzmanlık alanı başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }
            return View(uzmanlik);
        }

        // GET: Admin/UzmanlikAlani/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uzmanlik = await _context.UzmanlikAlanlari.FindAsync(id);
            if (uzmanlik == null)
            {
                return NotFound();
            }
            return View(uzmanlik);
        }

        // POST: Admin/UzmanlikAlani/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Aciklama")] UzmanlikAlani uzmanlik)
        {
            if (id != uzmanlik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uzmanlik);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Uzmanlık alanı başarıyla güncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UzmanlikExists(uzmanlik.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(uzmanlik);
        }

        // GET: Admin/UzmanlikAlani/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uzmanlik = await _context.UzmanlikAlanlari
                .Include(u => u.AntrenorUzmanliklar)
                    .ThenInclude(au => au.Antrenor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (uzmanlik == null)
            {
                return NotFound();
            }

            return View(uzmanlik);
        }

        // POST: Admin/UzmanlikAlani/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uzmanlik = await _context.UzmanlikAlanlari
                .Include(u => u.AntrenorUzmanliklar)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (uzmanlik != null)
            {
                // İlişkili kayıtları temizleme
                _context.AntrenorUzmanliklar.RemoveRange(uzmanlik.AntrenorUzmanliklar);

                _context.UzmanlikAlanlari.Remove(uzmanlik);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Uzmanlık alanı başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UzmanlikExists(int id)
        {
            return _context.UzmanlikAlanlari.Any(e => e.Id == id);
        }
    }
}