using Microsoft.VisualStudio.TestTools.UnitTesting;
using Binance.API.Csharp.Client.Models.Enums;
using System.Threading;
using Binance.API.Csharp.Client.Models.WebSocket;
using Binance.LIB.Csharp.Core;
using Binance.API.Csharp.Client.Models.Coin;

namespace Binance.API.Csharp.Client.Test
{
    [TestClass]
    public class BinanceCoreTest
    {
      

      
        [TestMethod]
        public void AddBuyTransaction()
        {
            CoinRepo.AddBuyTransaction(new BaseTransaction() {
                Id = 1,
                Type = 'S',
                Index = 4,
                Date = "101020117",
                Value = 0.000009m,
                Amount = 30,
                Symbol = "TRX"

            });
           
        }

        [TestMethod]
        public void AddSellTransaction()
        {
            CoinRepo.AddSellTransaction(new BaseTransaction()
            {
                Id = 1,
                Type = 'S',
                Index = 4,
                Date = "101020117",
                Value = 0.000009m,
                Amount = 30,
                Symbol = "TRX"
            });

        }

        [TestMethod]
        public void AddIndex()
        {
            DatabaseManager.AddIndex();
           
        }

        [TestMethod]
        public void updateBuyTransaction()
        {
            CoinRepo.updateTransaction(new BaseTransaction()
            {
                Id = 1,
                Type = 'S',
                Index = 4,
                Date = "101020117",
                Value = 0.000009m,
                Amount = 30,
                Symbol = "TRX"

            });

        }

        [TestMethod]
        public void updateSellTransaction()
        {
            CoinRepo.updateTransaction(new BaseTransaction()
            {
                Id = 1,
                Type = 'S',
                Index = 4,
                Date = "101020117",
                Value = 0.000009m,
                Amount = 30,
                Symbol = "TRX"

            });

        }

    }
}
