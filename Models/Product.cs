using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IMS.Models
{
    public class Product
    {

        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ProductCode { get; set; }
        public string? Barcode { get; set; }
        public int CurrentQuantity{ get; set; }
        public string ? Description { get; set; }
        public byte[] ? ProductImageUrl { get; set; }
        [Range(0, 9999.99)]
        public double ? DefaultBuyingPrice { get; set; } = 0.0;
        [Range(0, 9999.99)]
        public double ? DefaultSellingPrice { get; set; } = 0.0;
        public int? LowStockLevel { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrder { get; set; }

        public ICollection<SalesOrder> SalesOrder { get; set; }

  



    }
}
