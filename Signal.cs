namespace IBKR_Service
{
    public class Signal
    {
        public string Action { get; set; }
        public string Symbol { get; set; }
        public string AdjustedSymbol { get; set; }
        public double Lots { get; set; }
        public double AdjustedLots { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public double StopLoss { get; set; }
        public double TakeProfit { get; set; }
        public string Command { get; set; }
        public bool IsPending { get; set; }
        public int AcctId { get; set; }
    }
}

