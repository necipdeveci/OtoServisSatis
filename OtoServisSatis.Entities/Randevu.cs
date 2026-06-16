using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OtoServisSatis.Entities
{
    public class Randevu : IEntity
    {
        public int Id { get; set; }

        [StringLength(50), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        public string Ad { get; set; }

        [StringLength(50), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "Talep Tipi Boş Bırakılamaz!")]
        public TalepTipi TalepTipi { get; set; }

        [StringLength(20), Required(ErrorMessage = "Plaka Boş Bırakılamaz!")]
        public string Plaka { get; set; }

        [StringLength(50), Required(ErrorMessage = "Marka Boş Bırakılamaz!")]
        public string? AracMarka { get; set; }

        [StringLength(50), Required(ErrorMessage = "Model Boş Bırakılamaz!")]
        public string? AracModel { get; set; }

        [StringLength(50), Required(ErrorMessage = "Kasa Tipi Boş Bırakılamaz!")]
        public string? AracKasaTipi { get; set; }

        [Required(ErrorMessage = "Tarih Boş Bırakılamaz!")]
        public DateTime Tarih { get; set; }

        [StringLength(500)]
        public string? TalepNotu { get; set; }

        // Sadece Talep Satış için
        [Range(0, double.MaxValue, ErrorMessage = "Bütçe negatif olamaz!")]
        public decimal? Butce { get; set; }

        [Required]
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        public bool OnaylıMı { get; set; } = false;

        public int? KullaniciId { get; set; }
        [ForeignKey(nameof(KullaniciId))]
        public virtual Kullanici? Kullanici { get; set; }
    }

    public enum TalepTipi
    {
        [Display(Name = "Servis Talebi")]
        TalepServis = 1,
        [Display(Name = "Satış Talebi")]
        TalepSatis = 2
    }
}