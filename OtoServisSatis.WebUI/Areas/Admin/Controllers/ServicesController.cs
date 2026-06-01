using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OtoServisSatis.Entities;
using OtoServisSatis.Service.Abstract;
using OtoServisSatis.WebUI.Utils;
using System.Text;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace OtoServisSatis.WebUI.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Policy = "ServisPersoneliPolicy")]
    public class ServicesController : Controller
    {
        private readonly IService<Servis> _service;
        private readonly IService<Arac> _aracService;
        private readonly IService<Musteri> _musteriService;
        private readonly IService<Satis> _satisService; // Eklendi
        private readonly IConverter _pdfConverter; // Eklendi

        public ServicesController(
            IService<Servis> service,
            IService<Arac> aracService,
            IService<Musteri> musteriService,
            IService<Satis> satisService,
            IConverter pdfConverter) // Eklendi
        {
            _service = service;
            _aracService = aracService;
            _musteriService = musteriService;
            _satisService = satisService;
            _pdfConverter = pdfConverter; // Eklendi
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetAllAsync();
            return View(model);
        }

        public async Task<ActionResult> Details(int id)
        {
            var servis = await _service.FindAsync(id);
            if (servis == null) return NotFound();

            // Plaka üzerinden araç ve ardından müşteri bilgisine erişim kurgusu
            var arac = (await _aracService.GetAllAsync()).FirstOrDefault(a => a.Plaka == servis.AracPlaka);
            Musteri musteri = null;
            if (arac != null)
            {
                // Araca ait en güncel satışı bul
                var sonSatis = (await _satisService.GetAllAsync())
                                .Where(s => s.AracId == arac.Id)
                                .OrderByDescending(s => s.SatisTarihi)
                                .FirstOrDefault();

                if (sonSatis != null)
                {
                    // Satıştan gelen MusteriId ile müşteriyi bul
                    musteri = (await _musteriService.GetAllAsync()).FirstOrDefault(m => m.Id == sonSatis.MusteriId);
                }
            }

            ViewBag.Arac = arac;
            ViewBag.Musteri = musteri;

            return View(servis);
        }

        public ActionResult Create()
        {
            return View();
        }

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
            return View(servis);
        }

        public async Task<ActionResult> EditAsync(int id)
        {
            var model = await _service.FindAsync(id);
            if (model == null) return NotFound();

            if (model.DurumTamamlandi)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(int id, Servis servis)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _service.FindAsync(id);
                    if (existing == null) return NotFound();
                    if (existing.DurumTamamlandi) return RedirectToAction(nameof(Index));

                    // EF Core tracking (takip) hatasını engellemek için özellikler eşlenir
                    existing.ServiseGelisTarihi = servis.ServiseGelisTarihi;
                    existing.AracSorunu = servis.AracSorunu;
                    existing.ServisUcreti = servis.ServisUcreti;
                    existing.ServistenCikisTarihi = servis.ServistenCikisTarihi;
                    existing.YapilanIslemler = servis.YapilanIslemler;
                    existing.GarantiKapsamindaMi = servis.GarantiKapsamindaMi;
                    existing.AracPlaka = servis.AracPlaka;
                    existing.Marka = servis.Marka;
                    existing.Model = servis.Model;
                    existing.KasaTipi = servis.KasaTipi;
                    existing.SaseNo = servis.SaseNo;
                    existing.Notlar = servis.Notlar;

                    _service.Update(existing);
                    await _service.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Hata oluştu.");
                }
            }
            return View(servis);
        }

        public async Task<ActionResult> DeleteAsync(int id)
        {
            var model = await _service.FindAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

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

        [HttpGet]
        public async Task<IActionResult> PlakaAra(string term)
        {
            var tumAraclar = await _aracService.GetAllAsync();
            var plakalar = tumAraclar
                .Where(a => !string.IsNullOrEmpty(a.Plaka) && a.Plaka.StartsWith(term, StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Plaka)
                .ToList();

            return Json(plakalar);
        }

        [HttpGet]
        public async Task<IActionResult> AracDetayGetir(string plaka)
        {
            var araclar = await _aracService.GetAllAsync();
            var arac = araclar.FirstOrDefault(a => string.Equals(a.Plaka, plaka, StringComparison.OrdinalIgnoreCase));

            if (arac != null)
            {
                return Json(new
                {
                    bulundu = true,
                    marka = arac.Marka?.Adi, // Dropdown yerine input doldurulduğu için doğrudan metin adı gönderiliyor
                    modeli = arac.Modeli,
                    kasaTipi = arac.KasaTipi
                });
            }
            return Json(new { bulundu = false });
        }

        [HttpPost]
        public async Task<IActionResult> Tamamla(int id)
        {
            var model = await _service.FindAsync(id);
            if (model != null && !model.DurumTamamlandi)
            {
                model.DurumTamamlandi = true;
                _service.Update(model);
                await _service.SaveAsync();

                var arac = (await _aracService.GetAllAsync()).FirstOrDefault(a => a.Plaka == model.AracPlaka);
                if (arac != null)
                {
                    // YENİ MANTIK: Müşteriyi Satis tablosu üzerinden bul
                    var sonSatis = (await _satisService.GetAllAsync())
                                    .Where(s => s.AracId == arac.Id)
                                    .OrderByDescending(s => s.SatisTarihi)
                                    .FirstOrDefault();

                    Musteri musteri = null;
                    if (sonSatis != null)
                    {
                        musteri = (await _musteriService.GetAllAsync()).FirstOrDefault(m => m.Id == sonSatis.MusteriId);
                    }

                    if (musteri == null)
                    {
                        Console.WriteLine("\n[HATA] Bu araca ait bir satış kaydı ve müşteri bulunamadı! Mail atılamaz.\n");
                    }
                    else if (string.IsNullOrEmpty(musteri.Email))
                    {
                        Console.WriteLine($"\n[HATA] {musteri.Adi} {musteri.Soyadi} isimli müşterinin E-Posta adresi boş!\n");
                    }
                    else
                    {
                        try
                        {
                            byte[] faturaBytes = FaturaDosyasiOlustur(model, musteri);
                            await MailHelper.SendServisTamamlandiMailAsync(musteri, model, faturaBytes);
                            Console.WriteLine("\n[BAŞARILI] Servis tamamlandı maili başarıyla gönderildi.\n");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\n[MAİL GÖNDERME HATASI] {ex.Message}\n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\n[HATA] Plakaya ait araç veritabanında bulunamadı!\n");
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private byte[] FaturaDosyasiOlustur(Servis model, Musteri musteri)
        {
            decimal kdvOrani = 0.20m;
            decimal kdvHaric = model.ServisUcreti / (1 + kdvOrani);
            decimal kdvTutari = model.ServisUcreti - kdvHaric;
            var islemler = !string.IsNullOrEmpty(model.YapilanIslemler) ? model.YapilanIslemler.Split(',') : new string[] { "Servis Bakım Hizmeti" };

            var islemlerHtml = "";
            for (int i = 0; i < islemler.Length; i++)
            {
                islemlerHtml += $"<tr><td>{i + 1}</td><td>{islemler[i].Trim()}</td><td style='text-align:center;'>-</td><td style='text-align:right;'>-</td></tr>";
            }

            var htmlTemplate = $@"
            <html>
            <head>
                <meta charset='utf-8' />
                <style>
                    body {{ font-family: sans-serif; color: #333; line-height: 1.1; font-size: 10px; }}
                    .invoice-container {{ max-width: 100%; margin: 0 auto; }}
                    .invoice-box {{ background: #fff; padding: 10px; border: 1px solid #e0e0e0; }}
                    .invoice-header {{ display: flex; justify-content: space-between; border-bottom: 1px solid #2c3e50; padding-bottom: 3px; margin-bottom: 5px; }}
                    .invoice-company h2 {{ margin: 0; color: #2c3e50; font-size: 14px; font-weight: 700; }}
                    .invoice-company p, .invoice-info div {{ margin: 0; font-size: 10px; color: #555; }}
                    .invoice-title {{ color: #e74c3c; text-transform: uppercase; font-weight: bold; font-size: 12px; text-align: right; }}
                    .invoice-parties {{ display: flex; gap: 5px; margin-bottom: 5px; }}
                    .invoice-party {{ flex: 1; padding: 5px; border: 1px solid #eee; background: #f9f9f9; }}
                    .invoice-party h5 {{ margin: 0 0 2px 0; border-bottom: 1px solid #ddd; font-size: 11px; color: #2c3e50; }}
                    .invoice-party p {{ margin: 0; font-size: 9px; }}
                    .invoice-vehicle-info {{ margin-bottom: 5px; }}
                    .invoice-vehicle-info h5 {{ margin: 0 0 3px 0; font-size: 11px; border-bottom: 1px solid #eee; color: #2c3e50; }}
                    .invoice-vehicle-grid {{ display: flex; gap: 3px; }}
                    .invoice-vehicle-item {{ flex: 1; font-size: 9px; padding: 3px; border: 1px solid #eee; }}
                    .invoice-vehicle-item strong {{ display: block; font-size: 8px; color: #666; }}
                    .highlight {{ padding: 3px; border-left: 2px solid #ffc107; font-size: 9px; margin-top: 2px; }}
                    .invoice-items-table {{ width: 100%; border-collapse: collapse; margin-bottom: 5px; }}
                    .invoice-items-table th, .invoice-items-table td {{ padding: 4px; border-bottom: 1px solid #eee; font-size: 9px; }}
                    .invoice-items-table th {{ background: #2c3e50; color: #fff; text-align: left; }}
                    .invoice-summary {{ display: flex; justify-content: flex-end; margin-top: 5px; }}
                    .invoice-summary-table td {{ padding: 2px 5px; font-size: 10px; }}
                    .invoice-summary-table .total-row td {{ font-weight: bold; font-size: 11px; border-top: 1px solid #2c3e50; }}
                    .invoice-footer {{ border-top: 1px solid #eee; padding-top: 3px; font-size: 9px; margin-top: 10px; }}
                </style>
            </head>
            <body>
                <div class='invoice-container'>
                    <div class='invoice-box'>
                        <table style='width:100%; border-bottom:1px solid #2c3e50; padding-bottom:5px; margin-bottom:5px;'>
                            <tr>
                                <td>
                                    <div class='invoice-company'>
                                        <h2>ROTA MOTORLU ARAÇLAR</h2>
                                        <p>Otomotiv San. ve Tic. Ltd. Şti.</p>
                                    </div>
                                </td>
                                <td style='text-align:right;'>
                                    <div class='invoice-title'>e-Arşiv Fatura</div>
                                    <div class='invoice-info'>
                                        <div>Fatura: <strong>#SRV-{model.Id.ToString("D6")}</strong></div>
                                        <div>Tarih: <strong>{DateTime.Now.ToString("dd.MM.yy HH:mm")}</strong></div>
                                        <div>Çıkış: <strong>{model.ServistenCikisTarihi.ToString("dd.MM.yy")}</strong></div>
                                    </div>
                                </td>
                            </tr>
                        </table>

                        <table style='width:100%; margin-bottom:5px;'>
                            <tr>
                                <td style='width:50%; padding-right:5px;'>
                                    <div class='invoice-party'>
                                        <h5>Gönderici</h5>
                                        <p>Rota Motorlu Araçlar Otom. San. Tic. Ltd. Şti.<br>Melikgazi / Kayseri<br><strong>Tel:</strong> 0999 444 2222 | <strong>VD:</strong> Mimarsinan<br><strong>VKN:</strong> 4580912345</p>
                                    </div>
                                </td>
                                <td style='width:50%; padding-left:5px;'>
                                    <div class='invoice-party'>
                                        <h5>Alıcı (Müşteri)</h5>
                                        <p><strong>{(musteri != null ? $"{musteri.Adi} {musteri.Soyadi}" : "-")}</strong><br>{musteri?.Adres ?? "-"}<br><strong>TC/VKN:</strong> {musteri?.TcNo ?? "11111111111"}<br><strong>Tel:</strong> {musteri?.Telefon ?? "-"}</p>
                                    </div>
                                </td>
                            </tr>
                        </table>

                        <div class='invoice-vehicle-info'>
                            <h5>Araç Bilgileri</h5>
                            <table style='width:100%; border-collapse:collapse;'>
                                <tr>
                                    <td class='invoice-vehicle-item'><strong>Plaka:</strong> {model.AracPlaka}</td>
                                    <td class='invoice-vehicle-item'><strong>Marka/Model:</strong> {model.Marka} {model.Model}</td>
                                    <td class='invoice-vehicle-item'><strong>Kasa:</strong> {model.KasaTipi ?? "-"}</td>
                                    <td class='invoice-vehicle-item'><strong>Şase:</strong> {model.SaseNo ?? "-"}</td>
                                </tr>
                            </table>
                            {(!string.IsNullOrEmpty(model.AracSorunu) ? $"<div class='highlight'><strong>Sorun:</strong> {model.AracSorunu}</div>" : "")}
                        </div>

                        <table class='invoice-items-table'>
                            <thead>
                                <tr><th style='width:5%;'>#</th><th style='width:65%;'>Açıklama</th><th style='width:10%; text-align:center;'>KDV</th><th style='width:20%; text-align:right;'>Tutar</th></tr>
                            </thead>
                            <tbody>
                                {islemlerHtml}
                                <tr><td colspan='2' style='font-weight:bold;'>Servis/İşçilik Toplamı</td><td style='text-align:center;'>%20</td><td style='text-align:right;'>{kdvHaric.ToString("N2")} ₺</td></tr>
                            </tbody>
                        </table>

                        <table style='width:100%;'>
                            <tr>
                                <td style='width:60%;'></td>
                                <td style='width:40%; text-align:right;'>
                                    <table class='invoice-summary-table' style='margin-left:auto;'>
                                        <tr><td>Toplam Tutar:</td><td style='text-align:right;'>{kdvHaric.ToString("N2")} ₺</td></tr>
                                        <tr><td>KDV (%20):</td><td style='text-align:right;'>{kdvTutari.ToString("N2")} ₺</td></tr>
                                        <tr class='total-row'><td>ÖDENECEK:</td><td style='text-align:right;'>{model.ServisUcreti.ToString("N2")} ₺</td></tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                        <div class='invoice-footer'>
                            {(!string.IsNullOrEmpty(model.Notlar) ? $"<div style='margin-bottom:2px;'><strong>Notlar:</strong> {model.Notlar}</div>" : "")}
                            <div style='margin-bottom:2px;'><strong>Garanti:</strong> {(model.GarantiKapsamindaMi ? "Kapsamında" : "Dışı")}</div>
                            <div style='text-align:center; color:#777; font-size:8px; margin-top:10px;'>Bizi tercih ettiğiniz için teşekkür ederiz. Bilgi amaçlıdır.</div>
                        </div>
                    </div>
                </div>
            </body>
            </html>";

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlTemplate,
                WebSettings = { DefaultEncoding = "utf-8" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _pdfConverter.Convert(pdf);
        }

        [HttpGet]
        public async Task<IActionResult> FaturaIndir(int id)
        {
            var model = await _service.FindAsync(id);
            if (model == null) return NotFound();

            var arac = (await _aracService.GetAllAsync()).FirstOrDefault(a => a.Plaka == model.AracPlaka);
            Musteri musteri = null;

            if (arac != null)
            {
                var sonSatis = (await _satisService.GetAllAsync())
                                .Where(s => s.AracId == arac.Id)
                                .OrderByDescending(s => s.SatisTarihi)
                                .FirstOrDefault();

                if (sonSatis != null)
                {
                    musteri = (await _musteriService.GetAllAsync()).FirstOrDefault(m => m.Id == sonSatis.MusteriId);
                }
            }

            // Matematiksel Hesaplamalar
            decimal kdvOrani = 0.20m;
            decimal kdvHaric = model.ServisUcreti / (1 + kdvOrani);
            decimal kdvTutari = model.ServisUcreti - kdvHaric;
            var islemler = !string.IsNullOrEmpty(model.YapilanIslemler)
                ? model.YapilanIslemler.Split(',')
                : new string[] { "Servis Bakım Hizmeti" };

            // Dinamik İşlem Satırlarını Oluşturma
            var islemlerHtml = "";
            for (int i = 0; i < islemler.Length; i++)
            {
                islemlerHtml += $"<tr><td>{i + 1}</td><td>{islemler[i].Trim()}</td><td style='text-align:center;'>-</td><td style='text-align:right;'>-</td></tr>";
            }

            // Tasarladığınız CSS ve HTML Şablonu
            var htmlTemplate = $@"
            <html>
            <head>
                <meta charset='utf-8' />
                <style>
                    body {{ font-family: sans-serif; color: #333; line-height: 1.1; font-size: 10px; }}
                    .invoice-container {{ max-width: 100%; margin: 0 auto; }}
                    .invoice-box {{ background: #fff; padding: 10px; border: 1px solid #e0e0e0; }}
                    .invoice-header {{ display: flex; justify-content: space-between; border-bottom: 1px solid #2c3e50; padding-bottom: 3px; margin-bottom: 5px; }}
                    .invoice-company h2 {{ margin: 0; color: #2c3e50; font-size: 14px; font-weight: 700; }}
                    .invoice-company p, .invoice-info div {{ margin: 0; font-size: 10px; color: #555; }}
                    .invoice-title {{ color: #e74c3c; text-transform: uppercase; font-weight: bold; font-size: 12px; text-align: right; }}
                    .invoice-parties {{ display: flex; gap: 5px; margin-bottom: 5px; }}
                    .invoice-party {{ flex: 1; padding: 5px; border: 1px solid #eee; background: #f9f9f9; }}
                    .invoice-party h5 {{ margin: 0 0 2px 0; border-bottom: 1px solid #ddd; font-size: 11px; color: #2c3e50; }}
                    .invoice-party p {{ margin: 0; font-size: 9px; }}
                    .invoice-vehicle-info {{ margin-bottom: 5px; }}
                    .invoice-vehicle-info h5 {{ margin: 0 0 3px 0; font-size: 11px; border-bottom: 1px solid #eee; color: #2c3e50; }}
                    .invoice-vehicle-grid {{ display: flex; gap: 3px; }}
                    .invoice-vehicle-item {{ flex: 1; font-size: 9px; padding: 3px; border: 1px solid #eee; }}
                    .invoice-vehicle-item strong {{ display: block; font-size: 8px; color: #666; }}
                    .highlight {{ padding: 3px; border-left: 2px solid #ffc107; font-size: 9px; margin-top: 2px; }}
                    .invoice-items-table {{ width: 100%; border-collapse: collapse; margin-bottom: 5px; }}
                    .invoice-items-table th, .invoice-items-table td {{ padding: 4px; border-bottom: 1px solid #eee; font-size: 9px; }}
                    .invoice-items-table th {{ background: #2c3e50; color: #fff; text-align: left; }}
                    .invoice-summary {{ display: flex; justify-content: flex-end; margin-top: 5px; }}
                    .invoice-summary-table td {{ padding: 2px 5px; font-size: 10px; }}
                    .invoice-summary-table .total-row td {{ font-weight: bold; font-size: 11px; border-top: 1px solid #2c3e50; }}
                    .invoice-footer {{ border-top: 1px solid #eee; padding-top: 3px; font-size: 9px; margin-top: 10px; }}
                </style>
            </head>
            <body>
                <div class='invoice-container'>
                    <div class='invoice-box'>
                        <table style='width:100%; border-bottom:1px solid #2c3e50; padding-bottom:5px; margin-bottom:5px;'>
                            <tr>
                                <td>
                                    <div class='invoice-company'>
                                        <h2>ROTA MOTORLU ARAÇLAR</h2>
                                        <p>Otomotiv San. ve Tic. Ltd. Şti.</p>
                                    </div>
                                </td>
                                <td style='text-align:right;'>
                                    <div class='invoice-title'>e-Arşiv Fatura</div>
                                    <div class='invoice-info'>
                                        <div>Fatura: <strong>#SRV-{model.Id.ToString("D6")}</strong></div>
                                        <div>Tarih: <strong>{DateTime.Now.ToString("dd.MM.yy HH:mm")}</strong></div>
                                        <div>Çıkış: <strong>{model.ServistenCikisTarihi.ToString("dd.MM.yy")}</strong></div>
                                    </div>
                                </td>
                            </tr>
                        </table>

                        <table style='width:100%; margin-bottom:5px;'>
                            <tr>
                                <td style='width:50%; padding-right:5px;'>
                                    <div class='invoice-party'>
                                        <h5>Gönderici</h5>
                                        <p>Rota Motorlu Araçlar Otom. San. Tic. Ltd. Şti.<br>Melikgazi / Kayseri<br><strong>Tel:</strong> 0999 444 2222 | <strong>VD:</strong> Mimarsinan<br><strong>VKN:</strong> 4580912345</p>
                                    </div>
                                </td>
                                <td style='width:50%; padding-left:5px;'>
                                    <div class='invoice-party'>
                                        <h5>Alıcı (Müşteri)</h5>
                                        <p><strong>{(musteri != null ? $"{musteri.Adi} {musteri.Soyadi}" : "-")}</strong><br>{musteri?.Adres ?? "-"}<br><strong>TC/VKN:</strong> {musteri?.TcNo ?? "11111111111"}<br><strong>Tel:</strong> {musteri?.Telefon ?? "-"}</p>
                                    </div>
                                </td>
                            </tr>
                        </table>

                        <div class='invoice-vehicle-info'>
                            <h5>Araç Bilgileri</h5>
                            <table style='width:100%; border-collapse:collapse;'>
                                <tr>
                                    <td class='invoice-vehicle-item'><strong>Plaka:</strong> {model.AracPlaka}</td>
                                    <td class='invoice-vehicle-item'><strong>Marka/Model:</strong> {model.Marka} {model.Model}</td>
                                    <td class='invoice-vehicle-item'><strong>Kasa:</strong> {model.KasaTipi ?? "-"}</td>
                                    <td class='invoice-vehicle-item'><strong>Şase:</strong> {model.SaseNo ?? "-"}</td>
                                </tr>
                            </table>
                            {(!string.IsNullOrEmpty(model.AracSorunu) ? $"<div class='highlight'><strong>Sorun:</strong> {model.AracSorunu}</div>" : "")}
                        </div>

                        <table class='invoice-items-table'>
                            <thead>
                                <tr><th style='width:5%;'>#</th><th style='width:65%;'>Açıklama</th><th style='width:10%; text-align:center;'>KDV</th><th style='width:20%; text-align:right;'>Tutar</th></tr>
                            </thead>
                            <tbody>
                                {islemlerHtml}
                                <tr><td colspan='2' style='font-weight:bold;'>Servis/İşçilik Toplamı</td><td style='text-align:center;'>%20</td><td style='text-align:right;'>{kdvHaric.ToString("N2")} ₺</td></tr>
                            </tbody>
                        </table>

                        <table style='width:100%;'>
                            <tr>
                                <td style='width:60%;'></td>
                                <td style='width:40%; text-align:right;'>
                                    <table class='invoice-summary-table' style='margin-left:auto;'>
                                        <tr><td>Toplam Tutar:</td><td style='text-align:right;'>{kdvHaric.ToString("N2")} ₺</td></tr>
                                        <tr><td>KDV (%20):</td><td style='text-align:right;'>{kdvTutari.ToString("N2")} ₺</td></tr>
                                        <tr class='total-row'><td>ÖDENECEK:</td><td style='text-align:right;'>{model.ServisUcreti.ToString("N2")} ₺</td></tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                        <div class='invoice-footer'>
                            {(!string.IsNullOrEmpty(model.Notlar) ? $"<div style='margin-bottom:2px;'><strong>Notlar:</strong> {model.Notlar}</div>" : "")}
                            <div style='margin-bottom:2px;'><strong>Garanti:</strong> {(model.GarantiKapsamindaMi ? "Kapsamında" : "Dışı")}</div>
                            <div style='text-align:center; color:#777; font-size:8px; margin-top:10px;'>Bizi tercih ettiğiniz için teşekkür ederiz. Bilgi amaçlıdır.</div>
                        </div>
                    </div>
                </div>
            </body>
            </html>";

            // DinkToPdf Ayarları
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 10, Right = 10 }
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlTemplate,
                WebSettings = { DefaultEncoding = "utf-8" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var fileBytes = _pdfConverter.Convert(pdf);

            // Tarayıcıya dosyanın PDF formatında indirilmesini dikte eder.
            return File(fileBytes, "application/pdf", $"Fatura_{model.AracPlaka}_{model.Id}.pdf");
        }
    }
}
   