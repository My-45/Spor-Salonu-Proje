using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;
using SporSalonu.Models.Entities;
using SporSalonu.Models.ViewModels;

namespace SporSalonu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var randevuSayisi = await _context.Randevular
                    .Where(r => r.UyeId == user.Id)
                    .CountAsync();

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    AdSoyad = user.AdSoyad,
                    Email = user.Email ?? "",
                    EmailConfirmed = user.EmailConfirmed,
                    KayitTarihi = user.KayitTarihi,
                    Roller = roles.ToList(),
                    RandevuSayisi = randevuSayisi
                });
            }

            return View(userViewModels);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var randevular = await _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.HizmetTuru)
                .Where(r => r.UyeId == user.Id)
                .OrderByDescending(r => r.RandevuTarihi)
                .ToListAsync();

            ViewBag.Roles = roles;
            ViewBag.Randevular = randevular;

            return View(user);
        }

        // GET: Admin/Users/ManageRoles/5
        public async Task<IActionResult> ManageRoles(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var model = new ManageRolesViewModel
            {
                UserId = user.Id,
                UserName = user.AdSoyad,
                UserEmail = user.Email ?? "",
                Roles = allRoles.Select(r => new RoleSelectionViewModel
                {
                    RoleName = r.Name ?? "",
                    IsSelected = userRoles.Contains(r.Name ?? "")
                }).ToList()
            };

            return View(model);
        }

        // POST: Admin/Users/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Mevcut rolleri kaldırma
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                TempData["Error"] = "Roller kaldırılırken hata oluştu.";
                return RedirectToAction(nameof(ManageRoles), new { id = userId });
            }

            // Yeni rolleri ekleme
            if (selectedRoles != null && selectedRoles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                if (!addResult.Succeeded)
                {
                    TempData["Error"] = "Roller eklenirken hata oluştu.";
                    return RedirectToAction(nameof(ManageRoles), new { id = userId });
                }
            }

            TempData["Success"] = "Kullanıcı rolleri başarıyla güncellendi!";
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        // POST: Admin/Users/ToggleLockout/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLockout(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kendi kendini kilitlemeyi engelleme kısmı
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["Error"] = "Kendi hesabınızı kilitleyemezsiniz!";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                // Kilidi kaldırma
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = "Kullanıcının kilidi kaldırıldı.";
            }
            else
            {
                // Kilitleme
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["Success"] = "Kullanıcı kilitlendi.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kendi kendini silmeyi engelleme
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Kullanıcı başarıyla silindi!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Kullanıcı silinirken hata oluştu.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}