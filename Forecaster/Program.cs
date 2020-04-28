using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Forecaster.Forecasting.Entities;
using Forecaster.Forecasting.Prediction;

namespace Forecaster
{
    class Program
    {
        static int Main(string[] args)
        {
            //var list = Reader.ReadCSV("D:/NSE-TATAGLOBAL11.csv");

            //list.RemoveAt(0);

            //List<StockDataset> knownStockList = new List<StockDataset>(1000),
            //    realStockList = new List<StockDataset>(200);

            //List<BasicDataset> predictedStockList;

            //FillList(knownStockList, list, 987, 248);
            //FillList(realStockList, list, 248, 0);

            //knownStockList.Sort((x, y) => x.Date.CompareTo(y.Date));
            //realStockList.Sort((x, y) => x.Date.CompareTo(y.Date));

            //predictedStockList = new MovingAverage().GetAllPrediction(knownStockList, 248);

            ////foreach (string[] stockStringValues in list)
            ////    knownStockList.Add(new StockDataset(stockStringValues));



            //var test = new StockDataset(list[1]);

            //Writer.WriteStockDatasets("knownStockList.txt", knownStockList);
            //Writer.WriteStockDatasets("realStockList.txt", realStockList);
            //Writer.WriteStockDatasets("predictedStockList.txt", predictedStockList);

            //AsynchronousSocketListener.StartListening();
            return 0;
        }
    }
}