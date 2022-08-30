using System.ComponentModel.DataAnnotations;

namespace IMS.Models
{
    public class CustomerGroup
    {
        public int CustomerGroupId { get; set; }
        [Required]
        [Display(Name = "Customer Group")]
        public string CustomerGroupName { get; set; }
        public string ?Description { get; set; }
    }
}
