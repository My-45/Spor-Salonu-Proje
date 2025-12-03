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
    public class AntrenorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AntrenorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Antrenor
        public async Task<IActionResult> Index()
        {
            var antrenorler = await _context.Antrenorler
                .Include(a => a.Salon)
                .Include(a => a.AntrenorUzmanliklar)
                    .ThenInclude(au => au.UzmanlikAlani)
                .Include(a => a.MusaitlikSaatleri)
                .ToListAsync();

            return View(antrenorler);
        }

        // GET: Admin/Antrenor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler
                .Include(a => a.Salon)
                .Include(a => a.AntrenorUzmanliklar)
                    .ThenInclude(au => au.UzmanlikAlani)
                .Include(a => a.MusaitlikSaatleri)
                .Include(a => a.Randevular)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            return View(antrenor);
        }

        // GET: Admin/Antrenor/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Admin/Antrenor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AntrenorViewModel model)
        {

            var aktifValue = Request.Form["Aktif"].ToString();
            model.Aktif = aktifValue.Contains("true");

            if (ModelState.IsValid)
            {
                var antrenor = new Antrenor
                {
                    AdSoyad = model.AdSoyad,
                    Email = model.Email,
                    Telefon = model.Telefon,
                    Biyografi = model.Biyografi,
                    Aktif = model.Aktif,
                    SalonId = model.SalonId
                };

                _context.Add(antrenor);
                await _context.SaveChangesAsync();

                // Uzmanlık alanlarını ekle
                if (model.SecilenUzmanliklar != null && model.SecilenUzmanliklar.Any())
                {
                    foreach (var uzmanlikId in model.SecilenUzmanliklar)
                    {
                        _context.AntrenorUzmanliklar.Add(new AntrenorUzmanlik
                        {
                            AntrenorId = antrenor.Id,
                            UzmanlikAlaniId = uzmanlikId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Antrenör başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns();
            return View(model);
        }
        // GET: Admin/Antrenor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler
                .Include(a => a.AntrenorUzmanliklar)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            var model = new AntrenorViewModel
            {
                Id = antrenor.Id,
                AdSoyad = antrenor.AdSoyad,
                Email = antrenor.Email,
                Telefon = antrenor.Telefon,
                Biyografi = antrenor.Biyografi,
                Aktif = antrenor.Aktif,
                SalonId = antrenor.SalonId,
                SecilenUzmanliklar = antrenor.AntrenorUzmanliklar
                    .Select(au => au.UzmanlikAlaniId)
                    .ToList()
            };

            await PopulateDropdowns();
            return View(model);
        }

        // POST: Admin/Antrenor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AntrenorViewModel model)
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
                    var antrenor = await _context.Antrenorler
                        .Include(a => a.AntrenorUzmanliklar)
                        .FirstOrDefaultAsync(a => a.Id == id);

                    if (antrenor == null)
                    {
                        return NotFound();
                    }

                    antrenor.AdSoyad = model.AdSoyad;
                    antrenor.Email = model.Email;
                    antrenor.Telefon = model.Telefon;
                    antrenor.Biyografi = model.Biyografi;
                    antrenor.Aktif = model.Aktif;
                    antrenor.SalonId = model.SalonId;

                    // Mevcut uzmanlıkları sil
                    _context.AntrenorUzmanliklar.RemoveRange(antrenor.AntrenorUzmanliklar);

                    // yeni uzmanlık alanı ekle
                    if (model.SecilenUzmanliklar != null && model.SecilenUzmanliklar.Any())
                    {
                        foreach (var uzmanlikId in model.SecilenUzmanliklar)
                        {
                            _context.AntrenorUzmanliklar.Add(new AntrenorUzmanlik
                            {
                                AntrenorId = antrenor.Id,
                                UzmanlikAlaniId = uzmanlikId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Antrenör başarıyla güncellendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AntrenorExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await PopulateDropdowns();
            return View(model);
        }
        // GET: Admin/Antrenor/Musaitlik/5
        public async Task<IActionResult> Musaitlik(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler
                .Include(a => a.MusaitlikSaatleri)
                .Include(a => a.Salon)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            ViewBag.AntrenorId = id;
            ViewBag.AntrenorAdi = antrenor.AdSoyad;

            return View(antrenor);
        }

        // GET: Admin/Antrenor/MusaitlikEkle/5
        public IActionResult MusaitlikEkle(int antrenorId)
        {
            var model = new MusaitlikSaatiViewModel
            {
                AntrenorId = antrenorId,
                BaslangicSaati = "09:00",
                BitisSaati = "18:00",
                Aktif = true
            };

            return View(model);
        }

        // POST: Admin/Antrenor/MusaitlikEkle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MusaitlikEkle(MusaitlikSaatiViewModel model)
        {
           
            var aktifValue = Request.Form["Aktif"].ToString();
            model.Aktif = aktifValue.Contains("true");

            if (ModelState.IsValid)
            {
                
                var mevcutMusaitlik = await _context.MusaitlikSaatleri
                    .AnyAsync(m => m.AntrenorId == model.AntrenorId && m.Gun == model.Gun);

                if (mevcutMusaitlik)
                {
                    TempData["Error"] = "Bu gün için zaten bir müsaitlik kaydı var!";
                    return View(model);
                }

                var musaitlik = new MusaitlikSaati
                {
                    AntrenorId = model.AntrenorId,
                    Gun = model.Gun,
                    BaslangicSaati = TimeSpan.Parse(model.BaslangicSaati + ":00"),
                    BitisSaati = TimeSpan.Parse(model.BitisSaati + ":00"),
                    Aktif = model.Aktif
                };

                _context.MusaitlikSaatleri.Add(musaitlik);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Müsaitlik saati başarıyla eklendi!";
                return RedirectToAction("Musaitlik", new { id = model.AntrenorId });
            }

            return View(model);
        }

        // GET: Admin/Antrenor/MusaitlikDuzenle/5
        public async Task<IActionResult> MusaitlikDuzenle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musaitlik = await _context.MusaitlikSaatleri.FindAsync(id);
            if (musaitlik == null)
            {
                return NotFound();
            }

            var model = new MusaitlikSaatiViewModel
            {
                Id = musaitlik.Id,
                AntrenorId = musaitlik.AntrenorId,
                Gun = musaitlik.Gun,
                BaslangicSaati = musaitlik.BaslangicSaati.ToString(@"hh\:mm"),
                BitisSaati = musaitlik.BitisSaati.ToString(@"hh\:mm"),
                Aktif = musaitlik.Aktif
            };

            return View(model);
        }

        // POST: Admin/Antrenor/MusaitlikDuzenle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MusaitlikDuzenle(int id, MusaitlikSaatiViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            
            var aktifValue = Request.Form["Aktif"].ToString();
            model.Aktif = aktifValue.Contains("true");

            if (ModelState.IsValid)
            {
                var musaitlik = await _context.MusaitlikSaatleri.FindAsync(id);
                if (musaitlik == null)
                {
                    return NotFound();
                }

                musaitlik.Gun = model.Gun;
                musaitlik.BaslangicSaati = TimeSpan.Parse(model.BaslangicSaati + ":00");
                musaitlik.BitisSaati = TimeSpan.Parse(model.BitisSaati + ":00");
                musaitlik.Aktif = model.Aktif;

                _context.Update(musaitlik);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Müsaitlik saati başarıyla güncellendi!";
                return RedirectToAction("Musaitlik", new { id = model.AntrenorId });
            }

            return View(model);
        }

        // POST: Admin/Antrenor/MusaitlikSil/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MusaitlikSil(int id, int antrenorId)
        {
            var musaitlik = await _context.MusaitlikSaatleri.FindAsync(id);
            if (musaitlik != null)
            {
                _context.MusaitlikSaatleri.Remove(musaitlik);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Müsaitlik saati başarıyla silindi!";
            }

            return RedirectToAction("Musaitlik", new { id = antrenorId });
        }
        // GET: Admin/Antrenor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler
                .Include(a => a.Salon)
                .Include(a => a.AntrenorUzmanliklar)
                    .ThenInclude(au => au.UzmanlikAlani)
                .Include(a => a.Randevular)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            return View(antrenor);
        }

        // POST: Admin/Antrenor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var antrenor = await _context.Antrenorler
                .Include(a => a.AntrenorUzmanliklar)
                .Include(a => a.MusaitlikSaatleri)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (antrenor != null)
            {
                // İlişkili verileri temizle
                _context.AntrenorUzmanliklar.RemoveRange(antrenor.AntrenorUzmanliklar);
                _context.MusaitlikSaatleri.RemoveRange(antrenor.MusaitlikSaatleri);

                _context.Antrenorler.Remove(antrenor);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Antrenör başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AntrenorExists(int id)
        {
            return _context.Antrenorler.Any(e => e.Id == id);
        }

        private async Task PopulateDropdowns()
        {
            ViewBag.Salonlar = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad");
            ViewBag.UzmanlikAlanlari = await _context.UzmanlikAlanlari.ToListAsync();
        }
    }
}