namespace IBKR_Service
{
    public class Order
    {
        public string acctId { get; set; }
        public int conid { get; set; }
        public string orderType { get; set; }
        public string side { get; set; }
        public string ticker { get; set; }
        public string tif { get; set; }
        public double quantity { get; set; }
    }
}

