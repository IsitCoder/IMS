using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IMS.Models
{
    public class SalesOrder
    {
        public int SalesOrderId { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public double Price { get; set; }

        public double Amount { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public DateTimeOffset DeliveryDate { get; set; }
        public string? Remarks { get; set; }

        public string? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public int AdminId { get; set; }
        public Admin Admin { get; set; }

    }
}
