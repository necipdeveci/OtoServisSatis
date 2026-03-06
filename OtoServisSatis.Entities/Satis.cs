using System.ComponentModel.DataAnnotations;

namespace OtoServisSatis.Entities
{
    public class Satis : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Araç")]
        public int AracId { get; set; }
        [Display(Name = "Müşteri")]
        public int MusteriId { get; set; }
        [Display(Name = "Satış Fiyatı")]
        public decimal SatisFiyati { get; set; }
        [Display(Name = "Satış Tarihi")]
        public DateTime SatisTarihi { get; set; }
        [Display(Name = "Araç")]
        public Arac? Arac { get; set; }
        [Display(Name = "Müşteri")]
        public Musteri? Musteri { get; set; }
    }
}
