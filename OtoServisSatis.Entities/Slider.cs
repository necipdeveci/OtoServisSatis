using System.ComponentModel.DataAnnotations;

namespace OtoServisSatis.Entities
{
    public class Slider :IEntity
    {
        public int Id { get; set; }
        [StringLength(150)]
        public string? Baslik { get; set; }
        [StringLength(500)]
        public string? Aciklama { get; set; }
        [StringLength(100)]
        public string? Resim { get; set; }
        [StringLength(100)]
        public string? Link { get; set; }
    }
}
