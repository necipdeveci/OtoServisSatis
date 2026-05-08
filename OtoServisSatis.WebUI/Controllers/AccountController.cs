using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OtoServisSatis.Entities;
using OtoServisSatis.Service.Abstract;
using OtoServisSatis.WebUI.Models;
using System.Security.Claims;

namespace OtoServisSatis.WebUI.Controllers
{
    [Authorize(Policy = "AdminPagePolicy")]
    public class AccountController : Controller
    {
        private readonly IUserService _service;
        private readonly IService<Rol> _serviceRol;
        private readonly IService<Servis> _servisService;

        public AccountController(IUserService service, IService<Rol> serviceRol, IService<Servis> servisService)
        {
            _service = service;
            _serviceRol = serviceRol;
            _servisService = servisService;
        }

        public IActionResult Index()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var uguid = User.FindFirst(ClaimTypes.UserData)?.Value;
            if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(uguid))
            {
                var user = _service.Get(u => u.Email == email && u.UserGuid.ToString() == uguid);
                if (user != null)
                {
                    return View(user);
                }
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult UserUpdate(Kullanici kullanici)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var uguid = User.FindFirst(ClaimTypes.UserData)?.Value;
                if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(uguid))
                {
                    var user = _service.Get(u => u.Email == email && u.UserGuid.ToString() == uguid);
                    if (user != null)
                    {
                        user.Adi = kullanici.Adi;
                        user.Soyadi = kullanici.Soyadi;
                        user.AktifMi = kullanici.AktifMi;
                        user.Email = kullanici.Email;
                        user.UserGuid = kullanici.UserGuid;
                        user.Sifre = kullanici.Sifre;
                        user.EklenmeTarihi = kullanici.EklenmeTarihi;
                        user.Telefon = kullanici.Telefon;

                        _service.Update(user);
                        _service.Save();
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Hata Oluştu!");
            }

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var rol = await _serviceRol.GetAsync(r => r.Adi == "Customer");
                    kullanici.AktifMi = true;
                    if (rol == null)
                    {
                        ModelState.AddModelError("", "Kayıt Başarısız!");
                        return View();
                    }
                    kullanici.RolId = rol.Id;
                    await _service.AddAsync(kullanici);
                    await _service.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Hata Oluştu!");
                }
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(CustomerLoginViewModel customerViewModel)
        {
            try
            {
                var account = await _service.GetAsync(k => k.Email == customerViewModel.Email && k.Sifre == customerViewModel.Sifre && k.AktifMi == true);
                if (account == null)
                {
                    ModelState.AddModelError("", "Giriş Başarısız!");
                }
                else
                {
                    var rol = _serviceRol.Get(r => r.Id == account.RolId);

                    if (rol == null)
                    {
                        ModelState.AddModelError("", "Rol Bulunamadı!");
                        return View();
                    }

                    var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, account.Adi),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.UserData, account.UserGuid.ToString()),
                new Claim(ClaimTypes.Role, rol.Adi)
            };

                    var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(principal);

                    if (rol.Adi == "Admin" || rol.Adi == "SatisTemsilcisi" || rol.Adi == "ServisPersoneli")
                    {
                        return Redirect("/Admin");
                    }
                    return Redirect("/Account");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Hata Oluştu!");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult PlakaSorgula(string plaka)
        {
            if (string.IsNullOrEmpty(plaka))
                return View("Index");

            var servisKaydi = _servisService.GetAll()
                .FirstOrDefault(s => s.AracPlaka.ToLower() == plaka.ToLower());

            ViewBag.ServisKaydi = servisKaydi;
            return View("Index");
        }
    }
}