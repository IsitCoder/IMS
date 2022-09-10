using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace IMS.Models
{
    public class Bill
    {
        public int BillId { get; set; }
        [Required]
        public string BillCode { get; set; }
        [Required]
        [Display(Name = "Bill Create Date")]
        public DateTimeOffset BillCreateDate { get; set; }
        public string AdminId { get; set; }
        public Admin? Admin { get; set; }
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

    }
}
