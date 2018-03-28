using Binance.API.Csharp.Client.Models.Coin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binance.LIB.Csharp.Core
{
    public static class DatabaseManager
    {
        private static readonly string path = @"C:\Binance\BinanceApp\Database2\";
        public static int GetNextId(BaseTransaction trans)
        {
            var content = DirectoryManager.ReadFile(path + @"\" + trans.Symbol + @"BTC\file" + trans.Index + ".txt");
            var contenParts = content.Split(';');
            return contenParts.Count()+1;

        }

        public static int GetIndex(string file)
        {

            return Convert.ToInt32(file.Replace("BTC", "").Replace("file", "").Replace(".txt", ""));

        }

        public static decimal GetBitcoin()
        {

              return Convert.ToDecimal(DirectoryManager.ReadFile(path + @"\bitcoin.txt"));

        }

        public static void AddBitcoin(decimal value)
        {

            var btc = Convert.ToDecimal(DirectoryManager.ReadFile(path + @"\bitcoin.txt"))+Math.Abs (value);
            DirectoryManager.WriteFile(btc.ToString(), path + @"\bitcoin.txt");

        }

        public static void SubBitcoin(decimal value)
        {

            var btc = Convert.ToDecimal(DirectoryManager.ReadFile(path + @"\bitcoin.txt")) - Math.Abs(value);
            DirectoryManager.WriteFile(btc.ToString(), path + @"\bitcoin.txt");

        }

        public static int AddIndex()
        {
            var content = DirectoryManager.ReadFile(path + @"\index.txt");
            var index = Convert.ToInt32(content);
            index = index + 1;

            DirectoryManager.WriteFile(index.ToString(), @path + @"\index.txt");
            return index;
        }

        public static void reWriteTransactionFile(string Symbol,int index,string content)
        {
         
            DirectoryManager.CreateDirectory(path + @"\" + Symbol);
            DirectoryManager.WriteFile(content, path + @"\" + Symbol + @"\file" +index.ToString() + ".txt");
           
        }
        public static void appendTransactionFile(string Symbol, int index, string content)
        {

            DirectoryManager.CreateDirectory(path + @"\" + Symbol + "");
            DirectoryManager.AppendToFile(content, path + @"\" + Symbol + "" + @"\file" + index.ToString() + ".txt");

        }

        public static string  ReadFile(string Symbol, int index)
        {

          return DirectoryManager.ReadFile(path + @"\" + Symbol + @"\file" + index + ".txt");

        }
        public static string ReadFile(string Symbol, string file)
        {

            return DirectoryManager.ReadFile(path + @"\" + Symbol + @"\" + file);

        }
        public static List<string> ListLocalIcos()
        {
           return  DirectoryManager.ListDirectories(path);
        }

        public static List<string> ListTransactionFiles(string symbol)
        {
           return DirectoryManager.ListFiles(path+@"\" + symbol);
        }

        public static BaseTransaction ReadTransaction(string line, string symbol,int index)
        {
            var transactionParts = line.Split('|');
            BaseTransaction transaction = new BaseTransaction()
            {

                Id = Convert.ToInt32(transactionParts[0]),
                Amount = Convert.ToDecimal(transactionParts[1]),
                Date = transactionParts[2],
                Value = Convert.ToDecimal(transactionParts[3]),
                Status = transactionParts[4],
                Symbol = symbol,
                Type = transactionParts[0] == "1" ? 'B' : 'S',
                Index = index

            };
            return transaction;
        }

        public static string BuildTransaction(BaseTransaction transaction)
        {

            var line = transaction.Id.ToString() + "|" + transaction.Amount.ToString().Replace(",", ".");
            line = line + "|" + transaction.Date.Replace(",", ".").ToString();
            line = line + "|" + transaction.Value.ToString();
            line = line + "|" + transaction.Status + ";";

            return line;
        }
    }
}
