using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;
using SporSalonu.Models.Entities;

namespace SporSalonu.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandevularController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public RandevularController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Randevular
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> GetRandevular()
        {
            var userId = _userManager.GetUserId(User);

            var randevular = await _context.Randevular
                .Where(r => r.UyeId == userId)
                .Include(r => r.HizmetTuru)
                    .ThenInclude(h => h.Salon)
                .Include(r => r.Antrenor)
                .OrderByDescending(r => r.RandevuTarihi)
                .Select(r => new
                {
                    r.Id,
                    RandevuTarihi = r.RandevuTarihi.ToString("yyyy-MM-dd"),
                    RandevuSaati = r.RandevuSaati.ToString(@"hh\:mm"),
                    r.Durum,
                    Hizmet = r.HizmetTuru.Ad,
                    Salon = r.HizmetTuru.Salon.Ad,
                    Antrenor = r.Antrenor.AdSoyad,
                    Ucret = r.HizmetTuru.Ucret,
                    Sure = r.HizmetTuru.Sure,
                    r.Notlar
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = randevular,
                count = randevular.Count
            });
        }

        // POST: api/Randevular
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> CreateRandevu([FromBody] RandevuCreateDto dto)
        {
            var userId = _userManager.GetUserId(User);

            // Hizmeti kontrol et
            var hizmet = await _context.HizmetTurleri.FindAsync(dto.HizmetTuruId);
            if (hizmet == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Hizmet bulunamadı"
                });
            }

            // Antrenörü kontrol et
            var antrenor = await _context.Antrenorler.FindAsync(dto.AntrenorId);
            if (antrenor == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Antrenör bulunamadı"
                });
            }

            // Müsaitlik kontrol
            var mevcutRandevu = await _context.Randevular
                .AnyAsync(r => r.AntrenorId == dto.AntrenorId &&
                              r.RandevuTarihi.Date == dto.RandevuTarihi.Date &&
                              r.RandevuSaati == dto.RandevuSaati &&
                              r.Durum != RandevuDurumu.IptalEdildi);

            if (mevcutRandevu)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Bu saat için zaten randevu var"
                });
            }

            // Randevu oluştur
            var randevu = new Randevu
            {
                UyeId = userId,
                HizmetTuruId = dto.HizmetTuruId,
                AntrenorId = dto.AntrenorId,
                RandevuTarihi = dto.RandevuTarihi,
                RandevuSaati = dto.RandevuSaati,
                Notlar = dto.Notlar,
                Durum = RandevuDurumu.Beklemede,
                OlusturmaTarihi = DateTime.Now
            };

            _context.Randevular.Add(randevu);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRandevular), new
            {
                success = true,
                message = "Randevu başarıyla oluşturuldu",
                data = new
                {
                    randevu.Id,
                    RandevuTarihi = randevu.RandevuTarihi.ToString("yyyy-MM-dd"),
                    RandevuSaati = randevu.RandevuSaati.ToString(@"hh\:mm"),
                    randevu.Durum
                }
            });
        }

        // DELETE: api/Randevular/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<object>> DeleteRandevu(int id)
        {
            var userId = _userManager.GetUserId(User);

            var randevu = await _context.Randevular
                .FirstOrDefaultAsync(r => r.Id == id && r.UyeId == userId);

            if (randevu == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Randevu bulunamadı"
                });
            }

            // Sadece beklemedeki randevular iptal edilebilir
            if (randevu.Durum != RandevuDurumu.Beklemede)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Bu randevu iptal edilemez"
                });
            }

            randevu.Durum = RandevuDurumu.IptalEdildi;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Randevu iptal edildi"
            });
        }
    }

    // DTO Sınıfı
    public class RandevuCreateDto
    {
        public int HizmetTuruId { get; set; }
        public int AntrenorId { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public TimeSpan RandevuSaati { get; set; }
        public string? Notlar { get; set; }
    }
}
