using Binance.API.Csharp.Client.Models.Enums;
using System.Threading;
using System.Web.Mvc;
using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models;
using Binance.API.Csharp.Client.Models.WebSocket;
using System.Linq;
using System;
using Binance.LIB.Csharp.Core;
using Binance.API.Csharp.Client.Models.Coin;
using System.Collections.Generic;

namespace Binance.UI.Controllers
{
    public class HomeController : Controller
    {
        private static ApiClient apiClient = new ApiClient("@YourApiKey", "@YourApiSecret");
        private static BinanceClient binanceClient = new BinanceClient(apiClient, false);
        // GET: Home
        public ActionResult Index()
        {
           // var test = binanceClient.GetAllPrices().Result.Where(x=>x.Symbol.Contains("BTC")).OrderBy(x=>x.Symbol);
            return View();
        }

        public JsonResult ListCalculatedOpenTransactions()
        {
            try
            {
                var prices = binanceClient.GetPriceChange24H();
                var coins = binanceClient.GetAllPrices().Result.Where(x => x.Symbol.Contains("BTC")).OrderBy(x => x.Symbol);
                var transactions = CoinRepo.ListCalculatedOpenTransactions(coins, prices.Result).OrderByDescending(x => x.BuyTotal);
                var btc = coins.Where(x => x.Symbol == "BTCUSDT").FirstOrDefault();
                var totAlavailable = DatabaseManager.GetBitcoin();
                var className =   "";
                if (transactions.Sum(x => x.DiffTotal) * 100 > 0)
                    className = "profit";
                else className = "debit";

                return JReturn(new LumenResult()
                {

                    Data = new MainReturn()
                    {
                        Transactions = transactions.ToList(),
                        TotalBoughtOldValu = transactions.Sum(x => x.BuyTotal),
                        TotalBoughtOldValueDollar = (btc.Price * transactions.Sum(x => x.ActualTotal)),

                        TotalBoughtActualValue = transactions.Sum(x => x.ActualTotal),
                        TotalActualValueDollar = (btc.Price * transactions.Sum(x => x.ActualTotal)),

                        TotalSumBtc = transactions.Sum(x => x.ActualTotal) + totAlavailable,
                        TotalSumDollar = (btc.Price * totAlavailable) + (transactions.Sum(x => x.ActualTotal)* btc.Price),

                        TotalBoughtPerc = transactions.Sum(x => x.DiffTotal)*100,

                        TotalDollarFree = totAlavailable  * btc.Price,
                        TotalBtcFree = totAlavailable,
                        cssClass = className,

                        BctPrice = btc.Price,
                        SellTransactions = CoinRepo.ListAllSellTransactions(coins, prices.Result).OrderByDescending(x => x.BuyTotal).ToList(),
                        TargetTransactions = CoinRepo.ListAllTargetTransactions(coins, prices.Result).OrderByDescending(x => x.BuyTotal).ToList(),



                    }
                });
            }
            catch (Exception ex)
            {
                return JReturn(new LumenResult() {ErrorMessage="Erro" });
            }
        }

        public JsonResult ListAllSellTransactions()
        {
            try
            {
                var prices = binanceClient.GetPriceChange24H();
                var coins = binanceClient.GetAllPrices().Result.Where(x => x.Symbol.Contains("BTC")).OrderBy(x => x.Symbol);
                var transactions = CoinRepo.ListAllSellTransactions(coins, prices.Result).OrderByDescending(x => x.BuyTotal);
                var btc = coins.Where(x => x.Symbol == "BTCUSDT").FirstOrDefault();
                var totAlavailable = DatabaseManager.GetBitcoin();
                var className = "";
                if (transactions.Sum(x => x.DiffTotal) * 100 > 0)
                    className = "profit";
                else className = "debit";

                return JReturn(new LumenResult()
                {

                    Data = new MainReturn()
                    {
                       SellTransactions = CoinRepo.ListAllSellTransactions(coins, prices.Result).OrderByDescending(x => x.BuyTotal).ToList()


                    }
                });
            }
            catch (Exception ex)
            {
                return JReturn(new LumenResult() { ErrorMessage = "Erro" });
            }
        }

        public JsonResult ListSellTransactions(string symbol)
        {
            try
            {
                var prices = binanceClient.GetPriceChange24H();
                var coins = binanceClient.GetAllPrices().Result.Where(x => x.Symbol.Contains("BTC")).OrderBy(x => x.Symbol);
                var transactions = CoinRepo.ListAllSellTransactions(coins, prices.Result).Where(x=>x.Symbol==symbol).OrderByDescending(x => x.BuyTotal);
                var btc = coins.Where(x => x.Symbol == "BTCUSDT").FirstOrDefault();
                var totAlavailable = DatabaseManager.GetBitcoin();
                var className = "";
                if (transactions.Sum(x => x.DiffTotal) * 100 > 0)
                    className = "profit";
                else className = "debit";

                return JReturn(new LumenResult()
                {

                    Data = new MainReturn()
                    {
                        SellTransactions = CoinRepo.ListAllSellTransactions(coins, prices.Result).Where(x => x.Symbol ==symbol).OrderByDescending(x => x.BuyTotal).ToList()

                        

                    }
                });
            }
            catch (Exception ex)
            {
                return JReturn(new LumenResult() { ErrorMessage = "Erro" });
            }
        }

        public JsonResult AddBuyTransaction(decimal amount,decimal value , string symbol)
        {
            try
            {
                
                return JReturn(new LumenResult()
                {
                    Data = CoinRepo.AddBuyTransaction(new BaseTransaction() {
                        Date = DateTime.Now.Year.ToString()+DateTime.Now.Month.ToString()+DateTime.Now.Day.ToString(),
                      Amount = amount,
                      Symbol = symbol,
                    Value = value
                    })
                 });
            }
            catch
            {
                return JReturn(new LumenResult() { ErrorMessage = "Erro" });
            }
        }

        public JsonResult AddSellTransaction(decimal amount, decimal value, string symbol, int index)
        {
            try
            {

                return JReturn(new LumenResult()
                {
                    Data = CoinRepo.AddSellTransaction(new BaseTransaction()
                    {
                        Date = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString(),
                        Amount = amount,
                        Symbol = symbol,
                        Value = value,
                        Index = index,
                    })
                });
            }
            catch
            {
                return JReturn(new LumenResult() { ErrorMessage = "Erro" });
            }
        }

        public JsonResult AddTargetTransaction(decimal amount, decimal value, string symbol, int index)
        {
            try
            {

                return JReturn(new LumenResult()
                {
                    Data = CoinRepo.AddTargetTransaction(new BaseTransaction()
                    {
                        Date = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString(),
                        Amount = amount,
                        Symbol = symbol,
                        Value = value,
                        Index = index,
                    })
                });
            }
            catch
            {
                return JReturn(new LumenResult() { ErrorMessage = "Erro" });
            }
        }

        public JsonResult updateTransaction(int id,decimal amount, decimal value, string symbol, int index,string type, string status,decimal old)
        {
            try
            {

                return JReturn(new LumenResult()
                {
                    Data = CoinRepo.updateTransaction(new BaseTransaction()
                    {
                        Date = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString(),
                        Amount = amount,
                        Symbol = symbol,
                        Value = value,
                        Id=id,
                        Index = index,
                        Type = type=="B"?'B':'S',
                        Status = status,
                        OldValue = old
                    })
                });
            }
            catch
            {
                return JReturn(new LumenResult() { ErrorMessage = "Erro" });
            }
        }

        [HttpGet]
        public JsonResult GetCandleSticks(string coin)
        {
            var candlestick = binanceClient.GetCandleSticks("ethbtc", TimeInterval.Minutes_15, new System.DateTime(2017, 11, 24), new System.DateTime(2017, 11, 26)).Result;
            return JReturn(new LumenResult() { Data = candlestick });
        }

        public JsonResult GetAllIcos()
        {

            var coins = binanceClient.GetAllPrices().Result.Where(x => x.Symbol.Contains("BTC")).OrderBy(x => x.Symbol);
            // var transactions = CoinRepo.GetTransactions(coins);
            return JReturn(new LumenResult() { Data = coins });
        }

        public JsonResult JReturn(LumenResult result)
        {
            //Adicionando headers para remoção de cachecamento no cliente
            try
            {
                HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                HttpContext.Response.Headers.Add("Pragma", "no-cache"); // HTTP 1.0.
                HttpContext.Response.Headers.Add("Expires", "0"); // Proxies.
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }



    }

    public class LumenResult
    {
        public LumenResult()
        {
           // ErrorCode = ErrorCodes.None;
            ErrorMessage = "";
        }
        public Object Data;
        public Int64 Count;
        public String ErrorMessage;
        //public ErrorCodes ErrorCode;
    }

    public class MainReturn
    {
        public List<Transaction> Transactions{ get; set; }
        public decimal TotalBoughtActualValue { get; set; }
        public decimal TotalBoughtOldValu { get; set; }
        public decimal TotalBoughtOldValueDollar { get; set; }
        public decimal TotalActualValueDollar { get; set; }
        public decimal TotalSumDollar { get; set; }
        public decimal TotalBoughtPerc { get; set; }
        public decimal TotalDollarFree{ get; set; }
        public decimal TotalBtcFree { get; set; }
        public decimal TotalSumBtc { get; set; }
        public string cssClass { get; set; }
        public List<Transaction> SellTransactions { get; set; }
        public List<Transaction> TargetTransactions { get; set; }
        public decimal BctPrice { get; set; }
    }
}