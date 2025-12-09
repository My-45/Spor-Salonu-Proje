using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SporSalonu.Data;

namespace SporSalonu.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminBaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public AdminBaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Her admin sayfası yüklendiğinde bekleyen yorum sayısını hesapla
            var bekleyenYorumSayisi = _context.Yorumlar.Count(y => !y.Onaylandi);
            ViewBag.BekleyenYorumSayisi = bekleyenYorumSayisi;

            base.OnActionExecuting(context);
        }
    }
}
