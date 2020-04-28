using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.Server.Local
{
    public static class Initializer
    {
        public static void Initialize()
        {
            string defaultStocksDir = ConfigurationManager.AppSettings["defaultStocksDir"];

            try
            {
                Directory.CreateDirectory(defaultStocksDir);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
