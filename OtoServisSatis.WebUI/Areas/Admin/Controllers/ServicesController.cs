using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OtoServisSatis.Entities;
using OtoServisSatis.Service.Abstract;

namespace OtoServisSatis.WebUI.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Policy = "ServisPersoneliPolicy")]
    public class ServicesController : Controller
    {
        private readonly IService<Servis> _service;
        private readonly IService<Marka> _markaService;

        public ServicesController(IService<Servis> service, IService<Marka> markaService)
        {
            _service = service;
            _markaService = markaService;
        }

        // GET: ServicesController
        public async Task<IActionResult> Index()
        {
            var model = await _service.GetAllAsync();
            return View(model);
        }

        // GET: ServicesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ServicesController/Create
        public async Task<ActionResult> Create()
        {
            var markalar = await _markaService.GetAllAsync();
            ViewBag.Markalar = new SelectList(markalar, "Id", "Adi");
            return View();
        }

        // POST: ServicesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(Servis servis)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.AddAsync(servis);
                    await _service.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Hata oluştu.");
                }
            }
            var markalar = await _markaService.GetAllAsync();
            ViewBag.Markalar = new SelectList(markalar, "Id", "Adi");
            return View(servis);
        }

        // EditAsync metoduna da ekleyin
        public async Task<ActionResult> EditAsync(int id)
        {
            var model = await _service.FindAsync(id);
            var markalar = await _markaService.GetAllAsync();
            ViewBag.Markalar = new SelectList(markalar, "Id", "Adi");
            return View(model);
        }

        // POST EditAsync
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(int id, Servis servis)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _service.Update(servis);
                    await _service.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Hata oluştu.");
                }
            }
            var markalar = await _markaService.GetAllAsync();
            ViewBag.Markalar = new SelectList(markalar, "Id", "Adi");
            return View(servis);
        }

        // GET: ServicesController/Delete/5
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var model = await _service.FindAsync(id);
            return View(model);
        }

        // POST: ServicesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Servis servis)
        {
            try
            {
                _service.Delete(servis);
                await _service.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}