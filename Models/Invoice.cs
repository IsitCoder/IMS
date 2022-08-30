using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IMS.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        public int SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }  

        //[Display(Name = "Shipment")]
        //public int ShipmentId { get; set; }
        [Display(Name = "Invoice Date")]
        public DateTimeOffset InvoiceDate { get; set; }
        [Display(Name = "Invoice Due Date")]
        public DateTimeOffset InvoiceDueDate { get; set; }
  
    }
}
