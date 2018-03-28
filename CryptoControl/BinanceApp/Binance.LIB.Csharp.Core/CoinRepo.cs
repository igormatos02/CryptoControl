using Binance.API.Csharp.Client.Models.Coin;
using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance.LIB.Csharp.Core
{
    public static class CoinRepo
    {
        private static readonly string SHORT = "SHORT";
        private static readonly string TARGET = "TARGET";
        private static readonly string LONG = "LONG";
        private static readonly string CLOSE = "CLOSE";


        //public static List<Transaction> GetTransactions(IEnumerable<SymbolPrice> symbols, IEnumerable<PriceChangeInfo> prices)
        //{
        //    var transactions = new List<Transaction>();
        //    var coins = DirectoryManager.ListDirectories(@"C:\Binance\BinanceApp\Database\");
        //    foreach(var coin in coins)
        //    {
        //        var files = DirectoryManager.ListFiles(@"C:\Binance\BinanceApp\Database\" + coin);
        //        foreach(var file in files) {
        //            var transaction = new Transaction();
        //            var content  = DirectoryManager.ReadFile(@"C:\Binance\BinanceApp\Database\" + coin+ @"\"+file);

        //            var contenParts = content.Split(';');
                  
        //            var buyPart = contenParts[0].Split('|');
        //            transaction.Status = "open";
        //            transaction.ActualPrice = symbols.Where(x => x.Symbol == coin).FirstOrDefault().Price;
        //            transaction.Amount = Convert.ToDecimal(buyPart[1].Replace(".", ","));
        //            transaction.BuyPrice =buyPart[2];
        //            transaction.BuyDate = buyPart[3];
        //            transaction.Target = Convert.ToDecimal(buyPart[4].Replace(".", ","));
        //            if (contenParts.Length > 1 && contenParts[1].Trim()!="")
        //            {
        //                var sellPart = contenParts[1].Split('|');
        //                transaction.SellPrice = sellPart[2];
        //                transaction.SellDate = sellPart[3];
        //                transaction.Status = "closed";
        //            }

        //            transaction.Diff = transaction.ActualPrice - Convert.ToDecimal(transaction.BuyPrice.Replace(".",","));
        //            transaction.ActualTotal = transaction.ActualPrice * transaction.Amount;
        //            transaction.BuyTotal = Convert.ToDecimal(transaction.BuyPrice.Replace(".", ",")) * transaction.Amount;
        //            transaction.DiffTotal = transaction.Diff * transaction.Amount;

        //            transaction.TargetTotal = transaction.Amount*transaction.Target;
        //            var percentage = transaction.DiffTotal / transaction.BuyTotal*100;
        //            transaction.Perc = percentage;
        //            transaction.Percentage = percentage.ToString().Substring(0,6)+" %";

        //           transaction.TargetPerc = (transaction.TargetTotal / transaction.BuyTotal * 100).ToString().Substring(0, 6) + " %"; ;

        //            if (percentage > 0)
        //                transaction.PercClass = "profit";
        //            else transaction.PercClass = "debit";

        //            var binancePercentage = prices.Where(x => x.Symbol.ToUpper() == coin).FirstOrDefault().PriceChangePercent;
        //            transaction.BinancePercentage = binancePercentage.ToString();

        //            if (binancePercentage > 0)
        //                transaction.BinancePercClass = "profit";
        //            else transaction.BinancePercClass = "debit";

        //            transaction.Symbol = coin;
        //            transaction.Logo = coin.Replace("BTC","");
        //            if (transaction.Logo == "XLM")
        //                transaction.Logo = "STR";
        //            transactions.Add(transaction);


        //            //var index  = file.Replace("file", "").Replace("txt", "");
        //        }

        //    }
        //    return transactions;
        //}

        public static BaseTransaction AddBuyTransaction(BaseTransaction transaction)
        {
            transaction.Id = 1;
            transaction.Status = LONG;
            var line = DatabaseManager.BuildTransaction(transaction);
            DatabaseManager.reWriteTransactionFile(transaction.Symbol, DatabaseManager.AddIndex(), line);
            DatabaseManager.SubBitcoin(transaction.Amount * transaction.Value);
            return transaction;
        }

        public static BaseTransaction AddSellTransaction(BaseTransaction transaction)
        {
            transaction.Status = SHORT;
            transaction.Id = DatabaseManager.GetNextId(transaction);
            var line = DatabaseManager.BuildTransaction(transaction);
            DatabaseManager.appendTransactionFile(transaction.Symbol, transaction.Index, line);
            DatabaseManager.AddBitcoin(transaction.Amount*transaction.Value);

            return transaction;
        }

        public static BaseTransaction AddTargetTransaction(BaseTransaction transaction)
        {
            transaction.Status = TARGET;
            transaction.Id = DatabaseManager.GetNextId(transaction);
            var line = DatabaseManager.BuildTransaction(transaction);
            DatabaseManager.appendTransactionFile(transaction.Symbol, transaction.Index, line);
            

            return transaction;
        }

        public static BaseTransaction updateTransaction(BaseTransaction updatedTransaction)
        {

            var oldContent = DatabaseManager.ReadFile(updatedTransaction.Symbol, updatedTransaction.Index);
            var contenParts = oldContent.Split(';');



            var newStr = "";
            foreach (var line in contenParts)
            {
                if (line.Trim() != "")
                {
                    var transaction = DatabaseManager.ReadTransaction(line, updatedTransaction.Symbol, updatedTransaction.Index);
                    if (transaction.Id == updatedTransaction.Id)
                        newStr += DatabaseManager.BuildTransaction(updatedTransaction);
                    else
                        newStr += line;
                }
            }

            var btcDiff = updatedTransaction.OldValue - (updatedTransaction.Value * updatedTransaction.Amount);
            if (btcDiff > 0)
            {
                DatabaseManager.AddBitcoin(btcDiff);
            }
            else if (btcDiff < 0)
            {
                DatabaseManager.SubBitcoin(btcDiff);
            }

            DatabaseManager.reWriteTransactionFile(updatedTransaction.Symbol, updatedTransaction.Index, newStr);
            return updatedTransaction;
        }

        public static void shortTransaction(BaseTransaction transaction)
        {
            transaction.Status = SHORT;
            updateTransaction(transaction);
        }

        public static void closeTransaction(BaseTransaction transaction)
        {
            transaction.Status = CLOSE;
            updateTransaction(transaction);
        }

        public static List<Transaction> ListCalculatedOpenTransactions(IEnumerable<SymbolPrice> symbols, IEnumerable<PriceChangeInfo> prices)
        {
            var transactions = new List<Transaction>();
            var icos = DatabaseManager.ListLocalIcos();
            foreach (var symbol in icos)
            {
                var files = DatabaseManager.ListTransactionFiles(symbol);
                foreach (var file in files)
                {
                    var transaction = new Transaction();
                    var content = DatabaseManager.ReadFile(symbol,file);
                    var index = DatabaseManager. GetIndex(file);

                    var lines = content.Split(';');

                    var buyTransaction = DatabaseManager.ReadTransaction(lines[0], symbol, index);
                    if (buyTransaction.Status == CLOSE || buyTransaction.Status==TARGET)
                        continue;

                    var sellTransactions = new List<BaseTransaction>();
                    foreach(var line in lines)
                    {   if (line.Trim() != "")
                        {
                            var sellTransaction = DatabaseManager.ReadTransaction(line, symbol, index);
                            if (sellTransaction.Id != 1 && sellTransaction.Status == SHORT)
                            {
                                sellTransactions.Add(sellTransaction);
                            }
                        }
                    }
                    try
                    {
                        transactions.Add(GetCalculatedBuyTransaction(buyTransaction, sellTransactions, symbols, prices));
                    }
                    catch (Exception ex) {

                    }
                    
                }

            }
            return transactions;
        }

    
        public static List<Transaction> ListAllSellTransactions(IEnumerable<SymbolPrice> symbols, IEnumerable<PriceChangeInfo> prices)
        {
            var transactions = new List<Transaction>();
            var icos = DatabaseManager.ListLocalIcos();
            foreach (var symbol in icos)
            {
                var files = DatabaseManager.ListTransactionFiles(symbol);
                foreach (var file in files)
                {
                    var transaction = new Transaction();
                    var content = DatabaseManager.ReadFile(symbol, file);
                    var index = DatabaseManager.GetIndex(file);

                    var lines = content.Split(';');

                    var buyTransaction = DatabaseManager.ReadTransaction(lines[0], symbol, index);
                    if (buyTransaction.Status == TARGET)
                        continue;

                    var sellTransactions = new List<BaseTransaction>();
                    foreach (var line in lines)
                    {
                        if (line.Trim() != "" )
                        {
                            var sellTransaction = DatabaseManager.ReadTransaction(line, symbol, index);
                            if (sellTransaction.Id != 1 && sellTransaction.Status==SHORT)
                            {   
                                transactions.Add(GetCalculatedSellTransaction(buyTransaction, sellTransaction, symbols, prices));
                            }
                        }
                    }
                   

                }

            }
            return transactions;
        }


        public static List<Transaction> ListAllTargetTransactions(IEnumerable<SymbolPrice> symbols, IEnumerable<PriceChangeInfo> prices)
        {
            var transactions = new List<Transaction>();
            var icos = DatabaseManager.ListLocalIcos();
            foreach (var symbol in icos)
            {
                var files = DatabaseManager.ListTransactionFiles(symbol);
                foreach (var file in files)
                {
                    var transaction = new Transaction();
                    var content = DatabaseManager.ReadFile(symbol, file);
                    var index = DatabaseManager.GetIndex(file);

                    var lines = content.Split(';');

                    var buyTransaction = DatabaseManager.ReadTransaction(lines[0], symbol, index);
                    if (buyTransaction.Status == CLOSE)
                        continue;

                    var sellTransactions = new List<BaseTransaction>();
                    foreach (var line in lines)
                    {
                        if (line.Trim() != "")
                        {
                            var sellTransaction = DatabaseManager.ReadTransaction(line, symbol, index);
                            if (sellTransaction.Id != 1 && sellTransaction.Status==TARGET)
                            {
                                transactions.Add(GetCalculatedSellTransaction(buyTransaction, sellTransaction, symbols, prices));
                            }
                        }
                    }


                }

            }
            return transactions;
        }

        private static Transaction GetCalculatedBuyTransaction(BaseTransaction buyTransaction, List<BaseTransaction> sellTransactions, IEnumerable<SymbolPrice> symbols, IEnumerable<PriceChangeInfo> prices)
        {
            Transaction transaction = new Transaction();
            transaction.Index = buyTransaction.Index;
            transaction.Symbol = buyTransaction.Symbol;
            transaction.Status = buyTransaction.Status;

            transaction.BuyPrice = buyTransaction.Value;
            transaction.BuyAmount = buyTransaction.Amount;
            transaction.BuyDate = buyTransaction.Date;
            transaction.BuyTotal = buyTransaction.Amount * buyTransaction.Value;

            transaction.OldBuyTotal = transaction.BuyTotal;

            transaction.SellAmount = sellTransactions.Sum(x => x.Amount);

            transaction.ActualPrice = symbols.Where(x => x.Symbol == buyTransaction.Symbol).FirstOrDefault().Price;
            transaction.Diff = transaction.ActualPrice - transaction.BuyPrice;
            transaction.DiffStr = transaction.Diff.ToString();
            transaction.Amount = transaction.BuyAmount - transaction.SellAmount;
            transaction.ActualTotal = transaction.ActualPrice * transaction.Amount;
            transaction.DiffTotal = transaction.Diff * transaction.Amount;


            var percentage = transaction.DiffTotal / transaction.BuyTotal * 100;
            transaction.Perc = percentage;
            transaction.Percentage = percentage.ToString().Length > 6 ? percentage.ToString().Substring(0, 6) + " %" : percentage.ToString() + " %";

            // transaction.TargetPerc = (transaction.TargetTotal / transaction.BuyTotal * 100).ToString().Substring(0, 6) + " %"; ;

            if (percentage > 0)
                transaction.PercClass = "profit";
            else transaction.PercClass = "debit";

            var binancePercentage = prices.Where(x => x.Symbol.ToUpper() == buyTransaction.Symbol).FirstOrDefault().PriceChangePercent;
            transaction.BinancePercentage = binancePercentage.ToString();

            if (binancePercentage > 0)
                transaction.BinancePercClass = "profit";
            else transaction.BinancePercClass = "debit";


            transaction.Logo = buyTransaction.Symbol.Replace("BTC", "");
            if (transaction.Logo == "XLM")
                transaction.Logo = "STR";

            return transaction;
        }


        private static Transaction GetCalculatedSellTransaction(BaseTransaction buyTransaction, BaseTransaction sellTransaction, IEnumerable<SymbolPrice> symbols, IEnumerable<PriceChangeInfo> prices)
        {
            Transaction transaction = new Transaction();
            transaction.Index = buyTransaction.Index;
            transaction.Symbol = buyTransaction.Symbol;
            transaction.Status = buyTransaction.Status;

            transaction.BuyPrice = buyTransaction.Value;
            transaction.BuyAmount = buyTransaction.Amount;
            transaction.BuyDate = buyTransaction.Date;
            transaction.BuyTotal = buyTransaction.Amount * buyTransaction.Value;

            transaction.ActualPrice = symbols.Where(x => x.Symbol == buyTransaction.Symbol).FirstOrDefault().Price;
           

            transaction.SellAmount = sellTransaction.Amount;
            transaction.SellPrice = sellTransaction.Value;

            transaction.Diff = transaction.SellPrice - transaction.BuyPrice;
            transaction.DiffStr = transaction.Diff.ToString();

            transaction.Amount = transaction.SellAmount;
            transaction.SellTotal = transaction.SellPrice * transaction.SellAmount;
            transaction.DiffTotal = transaction.Diff * transaction.Amount;


            var percentage = transaction.DiffTotal / transaction.SellTotal * 100;
            transaction.Perc = percentage;
            transaction.Percentage = percentage.ToString().Length > 6 ? percentage.ToString().Substring(0, 6) + " %" : percentage.ToString() + " %";

            // transaction.TargetPerc = (transaction.TargetTotal / transaction.BuyTotal * 100).ToString().Substring(0, 6) + " %"; ;

            if (percentage > 0)
                transaction.PercClass = "profit";
            else transaction.PercClass = "debit";

            var binancePercentage = prices.Where(x => x.Symbol.ToUpper() == buyTransaction.Symbol).FirstOrDefault().PriceChangePercent;
            transaction.BinancePercentage = binancePercentage.ToString();

            if (binancePercentage > 0)
                transaction.BinancePercClass = "profit";
            else transaction.BinancePercClass = "debit";


            transaction.Logo = buyTransaction.Symbol.Replace("BTC", "");
            if (transaction.Logo == "XLM")
                transaction.Logo = "STR";

            return transaction;
        }

    }
}
