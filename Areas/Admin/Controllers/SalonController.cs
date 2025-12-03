using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;
using SporSalonu.Models.Entities;
using SporSalonu.Models.ViewModels;

namespace SporSalonu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SalonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Salon
        public async Task<IActionResult> Index()
        {
            var salonlar = await _context.Salonlar
                .OrderByDescending(s => s.Id)
                .ToListAsync();
            return View(salonlar);
        }

        // GET: Admin/Salon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salon = await _context.Salonlar
                .FirstOrDefaultAsync(m => m.Id == id);

            if (salon == null)
            {
                return NotFound();
            }

            return View(salon);
        }

        // GET: Admin/Salon/Create
        public IActionResult Create()
        {
            var model = new SalonViewModel
            {
                AcilisSaati = "06:00",
                KapanisSaati = "23:00",
                Aktif = true
            };
            return View(model);
        }

        // POST: Admin/Salon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalonViewModel model)
        {
            
            var aktifValue = Request.Form["Aktif"].ToString();
            model.Aktif = aktifValue.Contains("true");

            if (ModelState.IsValid)
            {
                var salon = new Salon
                {
                    Ad = model.Ad,
                    Adres = model.Adres,
                    AcilisSaati = TimeSpan.Parse(model.AcilisSaati + ":00"),
                    KapanisSaati = TimeSpan.Parse(model.KapanisSaati + ":00"),
                    Aktif = model.Aktif
                };

                _context.Add(salon);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Salon başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Admin/Salon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salon = await _context.Salonlar.FindAsync(id);
            if (salon == null)
            {
                return NotFound();
            }

            var model = new SalonViewModel
            {
                Id = salon.Id,
                Ad = salon.Ad,
                Adres = salon.Adres,
                AcilisSaati = salon.AcilisSaati.ToString(@"hh\:mm"),
                KapanisSaati = salon.KapanisSaati.ToString(@"hh\:mm"),
                Aktif = salon.Aktif
            };

            return View(model);
        }
        // POST: Admin/Salon/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SalonViewModel model)
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
                    var salon = await _context.Salonlar.FindAsync(id);
                    if (salon == null)
                    {
                        return NotFound();
                    }

                    salon.Ad = model.Ad;
                    salon.Adres = model.Adres;
                    salon.AcilisSaati = TimeSpan.Parse(model.AcilisSaati + ":00");
                    salon.KapanisSaati = TimeSpan.Parse(model.KapanisSaati + ":00");
                    salon.Aktif = model.Aktif;

                    _context.Update(salon);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Salon başarıyla güncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalonExists(model.Id))
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
            return View(model);
        }

        // GET: Admin/Salon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salon = await _context.Salonlar
                .FirstOrDefaultAsync(m => m.Id == id);

            if (salon == null)
            {
                return NotFound();
            }

            return View(salon);
        }

        // POST: Admin/Salon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salon = await _context.Salonlar.FindAsync(id);
            if (salon != null)
            {
                _context.Salonlar.Remove(salon);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Salon başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SalonExists(int id)
        {
            return _context.Salonlar.Any(e => e.Id == id);
        }
    }
}