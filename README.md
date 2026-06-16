# OtoServisSatis

Repo: https://github.com/necipdeveci/OtoServisSatis.git
Mimari: ASP.NET Core MVC, N-Layer (Entities / Data / Service / WebUI), Entity Framework Core, Areas (Admin), Cookie Authentication + Policy-based Authorization, Chart.js, WkHtmlToPdfDotNet (DinkToPdf), SMTP (System.Net.Mail), BackgroundService.

---

## 1. Rol Bazlı Giriş

**Konum:** `Program.cs` (AddAuthorization), `Controllers/AccountController.cs`, `Entities/Rol.cs`, `Entities/Kullanici.cs`

- Roller: `Admin`, `ServisPersoneli`, `SatisTemsilcisi`, `Customer` (+ tarihsel `User`).
- Policy tanımları (`Program.cs`):
  - `AdminPolicy` → sadece Admin
  - `ServisPersoneliPolicy` → Admin, ServisPersoneli
  - `SatisTemsilcisiPolicy` → Admin, SatisTemsilcisi
  - `CustomerPolicy` → Admin, User, Customer
  - `AdminPagePolicy` → tüm roller
- `AccountController.LoginAsync`: Email + Şifre + AktifMi kontrolü → `ClaimsPrincipal` oluşturma (Name, Email, UserData=UserGuid, Role, NameIdentifier) → Cookie sign-in.
- Yönlendirme: Admin/ServisPersoneli/SatisTemsilcisi → `/Admin`; Customer → `/Account`.
- Admin alanındaki controller'lar `[Authorize(Policy = "...")]` ile korunuyor: `ServicesController` → ServisPersoneliPolicy, `SalesController` → SatisTemsilcisiPolicy, `MainController` → AdminPolicy.

## 2. User Girişi Sekmeleri

**Konum:** `Controllers/FavoritesController.cs`, `Controllers/AracController.cs`, `Controllers/AccountController.cs`, `Controllers/RandevuController.cs`, `Entities/Kullanici.cs` (FavoriAraclarJson)

- **Favoriler:** `[Authorize(Policy = "CustomerPolicy")]`. `Kullanici.FavoriAraclarJson` alanında JSON dizi olarak saklanıyor (`System.Text.Json` ile serialize/deserialize). `Add`/`Remove` action'ları, maksimum 5 favori sınırı (`MAX_FAVORITES`).
- **Araçlarım:** `AracController` + `ICarRepository.GetCarByPlaka(string plaka)` (EF Core `FirstOrDefaultAsync` ile `Arac` tablosuna plaka üzerinden sorgu). `RandevuController.GetCarInfo` da plaka bazlı araç bilgisi döndürüyor.
- **Servis Geçmişi:** `AccountController.PlakaSorgula(string plaka)` → `Servis` tablosunda plaka ile `FirstOrDefault` araması, sonuç `Views/Account/Index.cshtml` üzerinde ViewBag.ServisKaydi olarak gösteriliyor.
- **Randevu Oluştur:** `Entities/Randevu.cs` — `TalepTipi` enum (`TalepServis=1`, `TalepSatis=2`). Satış için: Ad, Soyad, Tarih, AracMarka, AracModel, Butce alanları. Servis için: Ad, Soyad, TalepNotu, Plaka, AracMarka, AracModel, Tarih, AracKasaTipi alanları. `RandevuController.Create` → giriş yapan kullanıcının `NameIdentifier` claim'i ile `KullaniciId` atanıyor.

## 3. Grafikler

**Konum:** `Areas/Admin/Controllers/MainController.cs`, `Areas/Admin/Views/Main/Index.cshtml` (Chart.js 3.9.1)

| Endpoint | Açıklama |
|---|---|
| `GetAylikSatis(int? ay)` | Varsayılan: bir önceki ay. Seçili ay/yıl için satışları güne göre `GroupBy` → günlük adet (line chart). |
| `GetGunlukSatis(DateTime? tarih)` | Varsayılan: tarihten 1 gün önce. O güne ait satışları markaya göre gruplar (bar chart). |
| `GetServisAylik(int? ay)` | Seçili ay için servise gelen araçları markaya göre gruplar. |
| `GetServisGunluk(DateTime? tarih)` | Seçili gün için servise gelen araçları markaya göre gruplar. |
| `GetSatisMarkaModel(string marka, int? ay)` | Seçilen markaya göre, o ay hangi modelden kaç adet satıldığını gruplar (5 farklı marka için ayrı grafik üretimi `Index.cshtml` içinde tekrarlanan `loadChart` çağrılarıyla sağlanıyor). |

Tüm endpoint'ler JSON formatında `{Etiket, Deger}` listesi döndürür; istemci tarafında Chart.js ile `<canvas>` elemanlarına render edilir.

## 4. Fatura / Sözleşme PDF ve Servis Tamamlama Kısıtı

**Konum:** `Areas/Admin/Controllers/ServicesController.cs`, `Areas/Admin/Controllers/SalesController.cs`, NuGet: `WkHtmlToPdfDotNet`

- **Servis tamamlama:** `ServicesController.Tamamla(int id)` → `Servis.DurumTamamlandi = true` set edilir; `EditAsync` action'ı, `DurumTamamlandi == true` olan kayıtlar için düzenleme sayfasına izin vermeyip `Index`'e yönlendirir (salt görüntüleme kısıtı).
- **Fatura PDF:** `FaturaDosyasiOlustur` metodu, HTML şablonunu (`%20` KDV hesaplamasıyla) `IConverter` (DinkToPdf/WkHtmlToPdfDotNet, `Program.cs`'te `AddSingleton` ile DI'a eklenmiş) kullanarak `byte[]` PDF'e dönüştürür. `FaturaIndir(int id)` action'ı bu PDF'i `File(...)` ile tarayıcıya indirir.
- **Satış Sözleşmesi:** `SalesController.PrintContract(int id)` → `Satis`, `Arac`, `Musteri` bilgileriyle `Layout = null` bir HTML sözleşme görünümü render eder; tarayıcı tarafında yazdırma/PDF olarak indirilir.

## 5. Mail Bildirimleri ve Bakım Hatırlatma

**Konum:** `Utils/MailHelper.cs`, `Services/BakimHatirlatmaService.cs`, `Entities/Arac.cs` (BakimTarihi, BakimMailiGonderildi)

- **Servis tamamlandı maili:** `MailHelper.SendServisTamamlandiMailAsync(musteri, servis, faturaBytes)` — SMTP (Gmail, port 587, SSL) üzerinden HTML gövdeli mail + PDF fatura eki gönderir. `ServicesController.Tamamla` içinde, aracın son satışı üzerinden müşteri bulunup mail tetiklenir; müşteri veya e-posta yoksa konsola hata loglanır (mail gönderimi engellenmez, sessizce atlanır).
- **Bakım hatırlatma:** `Arac` entity'sinde `BakimTarihi` (DateTime) ve `BakimMailiGonderildi` (bool, tekrar mail önleme bayrağı) alanları bulunur. `BakimHatirlatmaService : BackgroundService` (Program.cs'te `AddHostedService` ile kayıtlı), sonsuz döngüde (test ortamında 30 saniyelik aralıklarla) `Satis` tablosunu `Arac` ve `Musteri` ile `Include` ederek `BakimTarihi <= DateTime.Now && !BakimMailiGonderildi` koşuluna uyan kayıtları bulur, her biri için `MailHelper.SendBakimHatirlatmaMailAsync` çağırır ve `BakimMailiGonderildi = true` olarak günceller.

---

## Kullanılan Teknolojiler (Özet)

- **Backend:** ASP.NET Core MVC (.NET), N-Layer mimari (Entities, Data/Repository, Service, WebUI)
- **Veritabanı:** Entity Framework Core (Code-First, Migrations)
- **Kimlik Doğrulama:** Cookie Authentication + Claims-based Policy Authorization
- **Grafik:** Chart.js (line/bar), AJAX ile JSON veri çekme
- **PDF Üretimi:** WkHtmlToPdfDotNet (DinkToPdf) — HTML → PDF dönüşümü
- **E-posta:** System.Net.Mail (SmtpClient, Gmail SMTP)
- **Arka Plan Görevi:** IHostedService / BackgroundService (periyodik bakım kontrolü)
- **Önyüz:** Razor Views, Bootstrap, Chart.js
