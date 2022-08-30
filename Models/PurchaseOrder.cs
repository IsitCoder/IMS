using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IMS.Models
{
    public class PurchaseOrder
    {
        public int PurchaseOrderId { get; set; }

        [Display(Name = "Supplier")]
 
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        [Display(Name = "Branch")]

        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public DateTimeOffset OrderDate { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }

        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public string? Remarks { get; set; }
        //public ICollection<PurchaseOrder_Product> PurchaseOrder_Product { get; set; }


    }
}
