using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisSatis.Data;
using OtoServisSatis.Entities;

namespace OtoServisSatis.WebUI.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Policy = "AdminPolicy")]
    public class MainController : Controller
    {
        private readonly DatabaseContext _context;

        public MainController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAylikSatis(int? ay)
        {
            var oncekiAy = DateTime.Now.AddMonths(-1);
            int seciliAy = ay ?? oncekiAy.Month;
            int seciliYil = ay.HasValue ? DateTime.Now.Year : oncekiAy.Year;

            var veriler = await _context.Satislar
                .Where(s => s.SatisTarihi.Month == seciliAy && s.SatisTarihi.Year == seciliYil)
                .GroupBy(s => s.SatisTarihi.Day)
                .Select(g => new { Etiket = g.Key.ToString() + ". Gün", Deger = g.Count() })
                .OrderBy(x => x.Etiket)
                .ToListAsync();

            return Json(veriler);
        }

        [HttpGet]
        public async Task<IActionResult> GetGunlukSatis(DateTime? tarih)
        {
            var seciliTarih = tarih?.Date ?? DateTime.Now.AddDays(-1).Date;

            var veriler = await _context.Satislar
                .Include(s => s.Arac)
                .ThenInclude(a => a.Marka)
                .Where(s => s.SatisTarihi.Date == seciliTarih)
                .GroupBy(s => s.Arac.Marka.Adi)
                .Select(g => new { Etiket = g.Key, Deger = g.Count() })
                .ToListAsync();

            return Json(veriler);
        }

        [HttpGet]
        public async Task<IActionResult> GetServisAylik(int? ay)
        {
            int seciliAy = ay ?? DateTime.Now.Month;

            var servisler = await _context.Servisler
                .Where(s => s.ServiseGelisTarihi.Month == seciliAy && s.ServiseGelisTarihi.Year == DateTime.Now.Year)
                .ToListAsync();

            var veriler = servisler
                .GroupBy(s => s.Marka)
                .Select(g => new
                {
                    Etiket = string.IsNullOrWhiteSpace(g.Key) ? "Bilinmeyen" : g.Key,
                    Deger = g.Count()
                })
                .ToList();

            return Json(veriler);
        }

        [HttpGet]
        public async Task<IActionResult> GetServisGunluk(DateTime? tarih)
        {
            var seciliTarih = tarih?.Date ?? DateTime.Now.Date;

            var servisler = await _context.Servisler
                .Where(s => s.ServiseGelisTarihi.Date == seciliTarih)
                .ToListAsync();

            var veriler = servisler
                .GroupBy(s => s.Marka)
                .Select(g => new
                {
                    Etiket = string.IsNullOrWhiteSpace(g.Key) ? "Bilinmeyen" : g.Key,
                    Deger = g.Count()
                })
                .ToList();

            return Json(veriler);
        }

        [HttpGet]
        public async Task<IActionResult> GetSatisMarkaModel(string marka, int? ay)
        {
            int seciliAy = ay ?? DateTime.Now.Month;
            if (string.IsNullOrEmpty(marka)) return Json(new object[] { });

            var veriler = await _context.Satislar
                .Include(s => s.Arac)
                .ThenInclude(a => a.Marka)
                .Where(s => s.SatisTarihi.Month == seciliAy && s.SatisTarihi.Year == DateTime.Now.Year && s.Arac.Marka.Adi == marka)
                .GroupBy(s => s.Arac.Modeli)
                .Select(g => new { Etiket = g.Key, Deger = g.Count() })
                .ToListAsync();

            return Json(veriler);
        }
    }
}