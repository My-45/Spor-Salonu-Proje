using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;
using SporSalonu.Models.Entities;

namespace SporSalonu.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalonlarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalonlarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Salonlar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetSalonlar()
        {
            var salonlar = await _context.Salonlar
                .Where(s => s.Aktif)
                .Select(s => new
                {
                    s.Id,
                    s.Ad,
                    s.Adres,
                    AcilisSaati = s.AcilisSaati.ToString(@"hh\:mm"),
                    KapanisSaati = s.KapanisSaati.ToString(@"hh\:mm"),
                    AntrenorSayisi = s.Antrenorler.Count(a => a.Aktif),
                    HizmetSayisi = s.HizmetTurleri.Count(h => h.Aktif)
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = salonlar,
                count = salonlar.Count
            });
        }

        // GET: api/Salonlar/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetSalon(int id)
        {
            var salon = await _context.Salonlar
                .Where(s => s.Id == id)
                .Include(s => s.Antrenorler)
                    .ThenInclude(a => a.AntrenorUzmanliklar)
                        .ThenInclude(au => au.UzmanlikAlani)
                .Include(s => s.HizmetTurleri)
                .Select(s => new
                {
                    s.Id,
                    s.Ad,
                    s.Adres,
                    AcilisSaati = s.AcilisSaati.ToString(@"hh\:mm"),
                    KapanisSaati = s.KapanisSaati.ToString(@"hh\:mm"),
                    Antrenorler = s.Antrenorler.Where(a => a.Aktif).Select(a => new
                    {
                        a.Id,
                        a.AdSoyad,
                        a.Email,
                        a.Telefon,
                        a.Biyografi,
                        Uzmanliklar = a.AntrenorUzmanliklar.Select(au => au.UzmanlikAlani.Ad).ToList()
                    }),
                    Hizmetler = s.HizmetTurleri.Where(h => h.Aktif).Select(h => new
                    {
                        h.Id,
                        h.Ad,
                        h.Aciklama,
                        h.Sure,
                        h.Ucret
                    })
                })
                .FirstOrDefaultAsync();

            if (salon == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Salon bulunamadı"
                });
            }

            return Ok(new
            {
                success = true,
                data = salon
            });
        }

        // GET: api/Salonlar/Musait?tarih=2024-01-15&hizmetId=1
        [HttpGet("Musait")]
        public async Task<ActionResult<object>> GetMusaitSaatler([FromQuery] DateTime tarih, [FromQuery] int hizmetId)
        {
            var hizmet = await _context.HizmetTurleri
                .Include(h => h.Salon)
                .FirstOrDefaultAsync(h => h.Id == hizmetId);

            if (hizmet == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Hizmet bulunamadı"
                });
            }

            // Randevuları kontrol et
            var mevcutRandevular = await _context.Randevular
                .Where(r => r.HizmetTuruId == hizmetId &&
                           r.RandevuTarihi.Date == tarih.Date &&
                           r.Durum != RandevuDurumu.IptalEdildi)
                .Select(r => r.RandevuSaati)
                .ToListAsync();

            // Salonun açılış-kapanış saatleri arası
            var acilis = hizmet.Salon.AcilisSaati;
            var kapanis = hizmet.Salon.KapanisSaati;

            var tumSaatler = new List<string>();
            var currentTime = acilis;

            while (currentTime < kapanis)
            {
                if (!mevcutRandevular.Contains(currentTime))
                {
                    tumSaatler.Add(currentTime.ToString(@"hh\:mm"));
                }
                currentTime = currentTime.Add(TimeSpan.FromHours(1));
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    tarih = tarih.ToString("yyyy-MM-dd"),
                    hizmet = hizmet.Ad,
                    salon = hizmet.Salon.Ad,
                    acilisSaati = acilis.ToString(@"hh\:mm"),
                    kapanisSaati = kapanis.ToString(@"hh\:mm"),
                    musaitSaatler = tumSaatler,
                    toplamMusait = tumSaatler.Count
                }
            });
        }
    }
}