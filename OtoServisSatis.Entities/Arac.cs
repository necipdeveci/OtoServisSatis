using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ← BU SATIRI EKLE

namespace OtoServisSatis.Entities
{
    public class Arac : IEntity
    {
        public int Id { get; set; }

        [StringLength(20), Display(Name = "Plaka")]
        public string? Plaka { get; set; }

        [Display(Name = "Marka Adı"), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        public int MarkaId { get; set; }
        [StringLength(50), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        public string Renk { get; set; }
        [Display(Name = "Fiyatı")]
        public decimal Fiyati { get; set; }
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        [StringLength(50), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        public string Modeli { get; set; }
        [Display(Name = "Kasa Tipi")]
        [StringLength(50), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        public string KasaTipi { get; set; }
        [Display(Name = "Model Yılı")]
        public int ModelYili { get; set; }
        [Display(Name = "Satışta mı?")]
        public bool SatistaMi { get; set; }
        [Display(Name = "Anasayfa?")]
        public bool Anasayfa { get; set; }
        [Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        public string Notlar { get; set; }
        [StringLength(100)]
        public string? Resim1 { get; set; }
        [StringLength(100)]
        public string? Resim2 { get; set; }
        [StringLength(100)]
        public string? Resim3 { get; set; }
        // Arac.cs içerisine eklenecek kodlar:
        [Display(Name = "Bakım Tarihi")]
        public DateTime BakimTarihi { get; set; } = DateTime.Now.AddMinutes(2); // Test için 2 dakika sonrası. Gerçek senaryoda: DateTime.Now.AddYears(1)

        [Display(Name = "Bakım Maili Gönderildi Mi?")]
        public bool BakimMailiGonderildi { get; set; } = false; // Tekrar tekrar mail atmayı engellemek için
        public virtual Marka? Marka { get; set; }

        [NotMapped]
        [Display(Name = "Renk Model KasaTipi"), ScaffoldColumn(false)]
        public string? AracBilgi
        {
            get
            {
                var markaAdi = Marka?.Adi ?? string.Empty;
                var renk = Renk ?? string.Empty;
                var modeli = Modeli ?? string.Empty;
                var kasa = KasaTipi ?? string.Empty;
                var result = $"{markaAdi} {renk} {modeli} {kasa}".Trim();
                return string.IsNullOrEmpty(result) ? null : result;
            }
        }
    }
}