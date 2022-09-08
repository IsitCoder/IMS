using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace IMS.Models
{
    public class Bill
    {
        public string BillId { get; set; }
        [Required]
        public string BillCode { get; set; }
        public int AdminId { get; set; }
        public Admin Admin { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        
    }
}
