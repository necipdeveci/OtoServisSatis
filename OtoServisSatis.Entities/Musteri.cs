using System.ComponentModel.DataAnnotations;

namespace OtoServisSatis.Entities
{
    public class Musteri : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Araç No")]
        public int AracId { get; set; }
        [Display(Name = "Adı"), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        [StringLength(50)]
        public string Adi { get; set; }
        [Display(Name = "Soyadı"), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        [StringLength(50)]
        public string Soyadi { get; set; }
        [Display(Name = "TC Numarası")]
        [StringLength(11)]
        public string? TcNo { get; set; }
        [StringLength(50), Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        public string Email { get; set; }
        [StringLength(500)]
        public string? Adres { get; set; }
        [StringLength(15)]
        public string? Telefon { get; set; }
        public string? Notlar { get; set; }
        public Arac? Arac { get; set; }
    }
}
