using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance.API.Csharp.Client.Models.Coin
{
    public class BaseTransaction
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Symbol { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }//LONG,SHORT,CLOSE
        public decimal Amount { get; set; }
        public char Type { get; set; }//Buy or Sell
        public decimal Value { get; set; }
        public decimal  OldValue { get; set; }

    }
    public class Transaction
    {
        public char Type { get; set; }
        public int Id { get; set; }
        public int Index { get; set; }
        public string Symbol { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }
        public string Logo { get; set; }
        public string DiffStr { get; set; }
        public decimal ActualPrice { get; set; }
        public string BuyDate { get; set; }
        public string SellDate { get; set; }
        public decimal Amount { get; set; }
        public decimal BuyAmount { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal SellAmount { get; set; }
        public string Status { get; set; }
        public decimal Diff { get; set; }
        public string Percentage { get; set; }
        public decimal Perc { get; set; }
        public decimal ActualTotal { get; set; }
        public decimal BuyTotal { get; set; }
        public decimal SellTotal { get; set; }
        public decimal DiffTotal { get; set; }
        public decimal Target { get; set; }
        public decimal TargetTotal { get; set; }
        public string TargetPerc{ get; set; }
        public string PercClass { get; set; }
        public string BinancePercentage { get; set; }
        public string BinancePercClass { get; set; }
        public decimal OldBuyTotal { get; set; }
    }


}
