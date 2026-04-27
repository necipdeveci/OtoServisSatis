using Microsoft.AspNetCore.Mvc;
using OtoServisSatis.Entities;
using OtoServisSatis.Service.Abstract;
using OtoServisSatis.WebUI.ExtensionMethods;

namespace OtoServisSatis.WebUI.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ICarService _serviceArac;

        public FavoritesController(ICarService serviceArac)
        {
            _serviceArac = serviceArac;
        }

        public IActionResult Index()
        {
            var favoriler = GetFavorites();
            return View(favoriler);
        }

        private List<Arac> GetFavorites()
        {
            List<Arac>? araclar = new();
            araclar = HttpContext.Session.GetJson<List<Arac>>("GetFavorites");
            return araclar;
        }
    }
}
