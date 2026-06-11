using System.ComponentModel.DataAnnotations;

namespace OtoServisSatis.Entities
{
    public class Rol : IEntity
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Rol adı")]
        [Required(ErrorMessage = "{0} Boş Bırakılamaz!")]
        [RegularExpression(@"^(?!\d+$).+", ErrorMessage = "{0} sadece sayıdan oluşamaz!")]
        public string Adi { get; set; }
    }
}