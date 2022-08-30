using System.ComponentModel.DataAnnotations;

namespace IMS.Models
{
    public class ProductType
    {
        public int ProductTypeId { get; set; }
        [Required]
        [Display(Name = "Product Type")]
        public string ProductTypeName { get; set; }
        
        public string? Description { get; set; }
    }
}
