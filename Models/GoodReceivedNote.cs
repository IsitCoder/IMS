using System.Security.Policy;

namespace IMS.Models
{
    public class GoodReceivedNote
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        
        public Admin Admin { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
