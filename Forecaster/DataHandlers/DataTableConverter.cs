using Forecaster.Forecasting.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.DataConverters
{
    public static class DataTableConverter
    {
        public static DataTable ConvertToDateTable(IEnumerable<StockDataset> datasets)
        {
            DataTable datasetsTable = new DataTable();

            InitColumns(datasetsTable);

            AddRows(datasetsTable, datasets);

            return datasetsTable;
        }

        private static void InitColumns(DataTable table)
        {
            string[] headers = new string[] { "Date", "Close" };

            foreach (string header in headers)
                table.Columns.Add(header);
        }

        private static void AddRows(DataTable table, IEnumerable<StockDataset> datasets)
        {
            foreach (StockDataset dataset in datasets)
                AddRow(table, dataset);
        }

        private static void AddRow(DataTable table, StockDataset dataset)
        {
            DataRow row = table.NewRow();

            row["Close"] = dataset.Close;
            row["Date"] = dataset.Date.ToString("yyyy-MM-dd");

            table.Rows.Add(row);
        }
    }
}
