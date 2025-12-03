using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;
using SporSalonu.Models.Entities;
using SporSalonu.Models.ViewModels;

namespace SporSalonu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HizmetTuruController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HizmetTuruController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HizmetTuru
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _context.HizmetTurleri
                .Include(h => h.Salon)
                .OrderByDescending(h => h.Id)
                .ToListAsync();

            return View(hizmetler);
        }

        // GET: Admin/HizmetTuru/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.HizmetTurleri
                .Include(h => h.Salon)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hizmet == null)
            {
                return NotFound();
            }

            return View(hizmet);
        }

        // GET: Admin/HizmetTuru/Create
        public async Task<IActionResult> Create()
        {
            await PopulateSalonDropdown();

            var model = new HizmetTuruViewModel
            {
                Sure = 60,
                Ucret = 100,
                Aktif = true
            };

            return View(model);
        }

        // POST: Admin/HizmetTuru/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HizmetTuruViewModel model)
        {
          
            var aktifValue = Request.Form["Aktif"].ToString();
            model.Aktif = aktifValue.Contains("true");

            if (ModelState.IsValid)
            {
                var hizmet = new HizmetTuru
                {
                    Ad = model.Ad,
                    Aciklama = model.Aciklama,
                    Sure = model.Sure,
                    Ucret = model.Ucret,
                    SalonId = model.SalonId,
                    Aktif = model.Aktif 
                };

                _context.Add(hizmet);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Hizmet türü başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateSalonDropdown();
            return View(model);
        }

        // GET: Admin/HizmetTuru/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.HizmetTurleri.FindAsync(id);
            if (hizmet == null)
            {
                return NotFound();
            }

            var model = new HizmetTuruViewModel
            {
                Id = hizmet.Id,
                Ad = hizmet.Ad,
                Aciklama = hizmet.Aciklama,
                Sure = hizmet.Sure,
                Ucret = hizmet.Ucret,
                SalonId = hizmet.SalonId,
                Aktif = hizmet.Aktif
            };

            await PopulateSalonDropdown();
            return View(model);
        }

       
        // POST: Admin/HizmetTuru/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HizmetTuruViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var aktifValue = Request.Form["Aktif"].ToString();
            model.Aktif = aktifValue.Contains("true");

            if (ModelState.IsValid)
            {
                try
                {
                    var hizmet = await _context.HizmetTurleri.FindAsync(id);
                    if (hizmet == null)
                    {
                        return NotFound();
                    }

                    hizmet.Ad = model.Ad;
                    hizmet.Aciklama = model.Aciklama;
                    hizmet.Sure = model.Sure;
                    hizmet.Ucret = model.Ucret;
                    hizmet.SalonId = model.SalonId;
                    hizmet.Aktif = model.Aktif;  

                    _context.Update(hizmet);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Hizmet türü başarıyla güncellendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HizmetExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await PopulateSalonDropdown();
            return View(model);
        }

        // GET: Admin/HizmetTuru/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.HizmetTurleri
                .Include(h => h.Salon)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hizmet == null)
            {
                return NotFound();
            }

            return View(hizmet);
        }

        // POST: Admin/HizmetTuru/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _context.HizmetTurleri.FindAsync(id);
            if (hizmet != null)
            {
                _context.HizmetTurleri.Remove(hizmet);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hizmet türü başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HizmetExists(int id)
        {
            return _context.HizmetTurleri.Any(e => e.Id == id);
        }

        private async Task PopulateSalonDropdown()
        {
            var salonlar = await _context.Salonlar
                .Where(s => s.Aktif)
                .OrderBy(s => s.Ad)
                .ToListAsync();

            ViewBag.Salonlar = new SelectList(salonlar, "Id", "Ad");
        }
    }
}