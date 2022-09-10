namespace IMS.Models.DTO
{
    public class DTOBillSearchPurchase
    {
        public int SupplierId { get; set; }
        public DateTimeOffset BillCreateDate { get; set; }
        public DateTimeOffset startdate { get; set; }
        public DateTimeOffset enddate { get; set; }
    }
}
