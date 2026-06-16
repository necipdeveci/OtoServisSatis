using System.ComponentModel.DataAnnotations;

namespace OtoServisSatis.Entities
{
    public class Musteri : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Araç")]
        public int AracId { get; set; }
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        [Display(Name = "Adı"), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        [StringLength(50)]
        public string Adi { get; set; }
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        [Display(Name = "Soyadı"), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        [StringLength(50)]
        public string Soyadi { get; set; }
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "TC Numarası 11 haneli ve yalnız sayı olmalıdır.")]
        [Display(Name = "TC Numarası")]
        [StringLength(11)]
        public string? TcNo { get; set; }
        [EmailAddress(ErrorMessage = "{0} geçerli bir email adresi olmalıdır")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Geçerli email formatı: metinsayi@domain.com")]
        [StringLength(50), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        public string Email { get; set; }
        [StringLength(500)]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        public string? Adres { get; set; }
        [StringLength(15)]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Telefon numarası 10 haneli ve yalnız sayı olmalıdır.")]
        public string? Telefon { get; set; }
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        public string? Notlar { get; set; }
        [Display(Name = "Araç")]
        public virtual Arac? Arac { get; set; }
        [Display(Name = "Ad Soyad"), ScaffoldColumn(false)]
        public string? AdSoyad
        {
            get { return this.Adi + " " + this.Soyadi; }
        }
    }
}
