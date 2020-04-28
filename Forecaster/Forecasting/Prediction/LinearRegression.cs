//using Forecaster.Forecasting.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Forecaster.Forecasting.Prediction
//{
//    class LinearRegression
//    {
//        public void test(IEnumerable<BasicDataset> average, int predictionCount)
//        {
//            var lol = average.ToList();

//            foreach (BasicDataset dataset in average)
//            {
//                if (dataset.Date.DayOfWeek == DayOfWeek.Friday || dataset.Date.DayOfWeek == DayOfWeek.Monday)
//                    ;
//            }
//        }
//        static double[] Solve(double[][] design)
//        {
//            int rows = design.Length;
//            int cols = data[0].Length;
//            double[][] X = MatrixCreate(rows, cols - 1);
//            double[][] Y = MatrixCreate(rows, 1);

//            int j;
//            for (int i = 0; i < rows; ++i)
//            {
//                for (j = 0; j < cols - 1; ++j)
//                {
//                    X[i][j] = design[i][j];
//                }
//                Y[i][0] = design[i][j]; // Last column
//            }

//            double[][] Xt = MatrixTranspose(X);
//            double[][] XtX = MatrixProduct(Xt, X);
//            double[][] inv = MatrixInverse(XtX);
//            double[][] invXt = MatrixProduct(inv, Xt);
//            double[][] mResult = MatrixProduct(invXt, Y);
//            double[] result = MatrixToVector(mResult);
//            return result;
//        }
//    }
//}
