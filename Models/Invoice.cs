using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IMS.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        [Required]
        public string InvoiceCode { get; set; }
        [Required]
        [Display(Name = "Invoice Create Date")]
        public DateTimeOffset InvoiceCreateDate { get; set; }
        public string AdminId { get; set; }
        public Admin? Admin { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }
}
