using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtoServisSatis.Entities;
using OtoServisSatis.Service.Abstract;
using OtoServisSatis.WebUI.ExtensionMethods;
using System.Security.Claims;
using System.Text.Json;

namespace OtoServisSatis.WebUI.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ICarService _serviceArac;
        private readonly IUserService _serviceKullanici;
        private const int MAX_FAVORITES = 5;

        public FavoritesController(ICarService serviceArac, IUserService serviceKullanici)
        {
            _serviceArac = serviceArac;
            _serviceKullanici = serviceKullanici;
        }

        [Authorize(Policy = "CustomerPolicy")]
        public IActionResult Index()
        {
            var favoriler = GetFavoritesFromDatabase();
            return View(favoriler);
        }

        private List<Arac> GetFavoritesFromDatabase()
        {
            var kullanici = GetCurrentUser();
            if (kullanici == null)
                return new List<Arac>();

            var aracIdleri = ParseFavoritesJson(kullanici.FavoriAraclarJson);
            var favoriler = new List<Arac>();

            foreach (var id in aracIdleri)
            {
                var arac = _serviceArac.Find(id);
                if (arac != null)
                    favoriler.Add(arac);
            }

            return favoriler;
        }

        private List<int> ParseFavoritesJson(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return new List<int>();

            try
            {
                return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }

        private Kullanici? GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var uguid = User.FindFirst(ClaimTypes.UserData)?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(uguid))
                return null;

            return _serviceKullanici.Get(u => u.Email == email && u.UserGuid.ToString() == uguid);
        }

        [Authorize(Policy = "CustomerPolicy")]
        [HttpPost]
        public IActionResult Add(int aracId)
        {
            try
            {
                var kullanici = GetCurrentUser();
                if (kullanici == null)
                {
                    TempData["Message"] = "<div class='alert alert-warning'>Lütfen oturum açın.</div>";
                    return RedirectToAction("Login", "Account");
                }

                var arac = _serviceArac.Find(aracId);
                if (arac == null)
                {
                    TempData["Message"] = "<div class='alert alert-danger'>Araç bulunamadı.</div>";
                    return RedirectToAction("Index");
                }

                var aracIdleri = ParseFavoritesJson(kullanici.FavoriAraclarJson);

                if (aracIdleri.Contains(aracId))
                {
                    TempData["Message"] = "<div class='alert alert-info'>Bu araç zaten favori listesinde.</div>";
                    return RedirectToAction("Index");
                }

                if (aracIdleri.Count >= MAX_FAVORITES)
                {
                    TempData["Message"] = $"<div class='alert alert-warning'>Maksimum {MAX_FAVORITES} araç favorilere ekleyebilirsiniz.</div>";
                    return RedirectToAction("Index");
                }

                aracIdleri.Add(aracId);
                kullanici.FavoriAraclarJson = JsonSerializer.Serialize(aracIdleri);

                _serviceKullanici.Update(kullanici);
                _serviceKullanici.Save();

                TempData["Message"] = "<div class='alert alert-success'>Araç favorilere eklendi.</div>";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "<div class='alert alert-danger'>Bir hata oluştu.</div>";
            }

            return RedirectToAction("Index");
        }

        [Authorize(Policy = "CustomerPolicy")]
        [HttpPost]
        public IActionResult Remove(int aracId)
        {
            try
            {
                var kullanici = GetCurrentUser();
                if (kullanici == null)
                {
                    TempData["Message"] = "<div class='alert alert-warning'>Lütfen oturum açın.</div>";
                    return RedirectToAction("Login", "Account");
                }

                var aracIdleri = ParseFavoritesJson(kullanici.FavoriAraclarJson);

                if (!aracIdleri.Contains(aracId))
                {
                    TempData["Message"] = "<div class='alert alert-info'>Bu araç favori listesinde değil.</div>";
                    return RedirectToAction("Index");
                }

                aracIdleri.Remove(aracId);
                kullanici.FavoriAraclarJson = JsonSerializer.Serialize(aracIdleri);

                _serviceKullanici.Update(kullanici);
                _serviceKullanici.Save();

                TempData["Message"] = "<div class='alert alert-success'>Araç favorilerden kaldırıldı.</div>";
            }
            catch (Exception)
            {
                TempData["Message"] = "<div class='alert alert-danger'>Bir hata oluştu.</div>";
            }

            return RedirectToAction("Index");
        }
    }
}