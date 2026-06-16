using System.ComponentModel.DataAnnotations;

namespace OtoServisSatis.Entities
{
    public class Slider : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gerekli")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        [StringLength(150, ErrorMessage = "Başlık 150 karakteri geçemez")]
        [Display(Name = "Başlık")]
        public string? Baslik { get; set; }

        [Required(ErrorMessage = "Açıklama gerekli")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        [StringLength(500, ErrorMessage = "Açıklama 500 karakteri geçemez")]
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        
        [StringLength(100)]
        public string? Resim { get; set; }

        [StringLength(100, ErrorMessage = "Link 100 karakteri geçemez")]
        [Display(Name = "Link")]
        public string? Link { get; set; }
    }
}