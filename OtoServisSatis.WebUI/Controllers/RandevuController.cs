using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtoServisSatis.Entities;
using OtoServisSatis.Service.Abstract;
using System.Security.Claims;

namespace OtoServisSatis.WebUI.Controllers
{
    public class RandevuController : Controller
    {
        private readonly IService<Randevu> _randevuService;
        private readonly IUserService _userService;
        private readonly ICarService _carService;

        public RandevuController(IService<Randevu> randevuService, IUserService userService, ICarService carService)
        {
            _randevuService = randevuService;
            _userService = userService;
            _carService = carService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("api/randevu/get-car-info")]
        public async Task<IActionResult> GetCarInfo(string plaka)
        {
            if (string.IsNullOrEmpty(plaka))
                return BadRequest("Plaka gerekli");

            var arac = await _carService.GetCarByPlaka(plaka);

            if (arac == null)
                return Ok(new { success = false, message = "Araç bulunamadı" });

            return Ok(new
            {
                success = true,
                data = new
                {
                    marka = arac.Marka?.Adi ?? "Bilinmiyor",
                    model = arac.Modeli,
                    kasaTipi = arac.KasaTipi
                }
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Randevu randevu)
        {
            if (!ModelState.IsValid)
                return View("Index", randevu);

            try
            {
                var kullaniciId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                randevu.KullaniciId = int.Parse(kullaniciId);
                randevu.EklenmeTarihi = DateTime.Now;

                await _randevuService.AddAsync(randevu);
                await _randevuService.SaveAsync();

                TempData["Message"] = "<div class='alert alert-success'>Randevunuz başarıyla oluşturuldu!</div>";
                return RedirectToAction("Onarim");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
                return View("Index", randevu);
            }
        }

        [Authorize]
        public async Task<IActionResult> Onarim()
        {
            var randevular = await _randevuService.GetAllAsync();
            return View(randevular);
        }

        private Kullanici? GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var uguid = User.FindFirst(ClaimTypes.UserData)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(uguid))
                return null;

            return _userService.Get(u => u.Email == email && u.UserGuid.ToString() == uguid);
        }
    }
}