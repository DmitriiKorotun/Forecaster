using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Forecaster.Net;

namespace Forecaster.Server.Local
{
    public static class FileManager
    {
        public static IEnumerable<string> GetStockList()
        {
            string stocksDir = ConfigurationManager.AppSettings["defaultStocksDir"];

            IEnumerable<string> stocks = Directory.EnumerateDirectories(stocksDir),
                formattedStocks = FormatStockName(stocks);           

            return formattedStocks;
        }

        private static IEnumerable<string> FormatStockName(IEnumerable<string> stocks)
        {
            List<string> formattedStocks = new List<string>(stocks.Count());

            foreach (string stock in stocks)
            {
                string formattedStock = new DirectoryInfo(stock).Name;

                formattedStocks.Add(formattedStock);
            }

            return formattedStocks;
        }

        public static IEnumerable<string> GetStockList(string pathToDirs)
        {
            if (Directory.Exists(pathToDirs))
                return Directory.EnumerateDirectories(pathToDirs);
            else
                throw new Exception("Directory doesn't exist");
        }

        //private IEnumerable<PredictionAlgorithm> GetAvailableAlgorithms(int stockId)
        //{
        //    string stockDir = GetStockDir(stockId),
        //        predictionsDir = stockDir + '/' + ConfigurationManager.AppSettings["defaultPredictionsDir"];

        //    IEnumerable<string> directories = Directory.EnumerateDirectories(predictionsDir);

        //    foreach (string directory in directories)
        //    {
        //        if (IsDirectoryAlghDir(directory))
        //            ;
        //    }
        //}

        private static bool IsDirectoryAlghDir(string directory)
        {
            string movingAverageDir = ConfigurationManager.AppSettings["MovingAverageDir"],
                linearRegressionDir = ConfigurationManager.AppSettings["LinearRegressionDir"],
                kNearestDir = ConfigurationManager.AppSettings["KNearestDir"],
                lstmDir = ConfigurationManager.AppSettings["LstmDir"];

            List<string> algorithmDirectories = new List<string>() {
                movingAverageDir, linearRegressionDir, kNearestDir, lstmDir
            };

            foreach (string algorithmDirectory in algorithmDirectories)
            {
                if (directory == algorithmDirectory)
                    return true;
            }

            return false;
        }

        private static string GetStockDir(int stockId)
        {
            string desiredStockDir, stocksDir = ConfigurationManager.AppSettings["defaultStocksDir"];

            IEnumerable<string> stocks = Directory.EnumerateDirectories(stocksDir);

            desiredStockDir = stocks.ElementAt(stockId);

            return desiredStockDir;
        }

        private static bool IsAlgorithmHasPredictions(string algorithmDir)
        {
            bool result = false;

            IEnumerable<string> dirFiles = Directory.EnumerateFiles(algorithmDir);

            foreach (string file in dirFiles)
            {
                if (file == ConfigurationManager.AppSettings["defaultPredictedFile"])
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
