using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.DataHandlers.DateTable
{
    class DateTableHandler
    {
        public DataTable SortTable(DataTable table, string sortOrder)
        {
            DataView dtview = new DataView(table)
            {
                Sort = sortOrder
            };

            return dtview.ToTable();
        }

        public void FillColumn(DataTable table, DataColumn column, IEnumerable<double> values)
        {
            int rowIndex = 0;

            foreach (DataRow row in table.Rows)
            {
                row[column.ColumnName] = values.ElementAt(rowIndex);

                ++rowIndex;
            }
        }

        public void FillColumnRange(DataTable table, List<DataColumn> columns, IEnumerable<double[]> values)
        {
            int columnIndex = 0;

            foreach(DataColumn column in columns)
            {
                IEnumerable<double> currentColValues = values.ElementAt(columnIndex).ToList();

                FillColumn(table, column, currentColValues);

                ++columnIndex;
            }
        }

        public void AddColumn(DataTable table, DataColumn column)
        {
            table.Columns.Add(column);
        }
        public void AddColumnRange(DataTable table, IEnumerable<DataColumn> columns)
        {
            foreach (DataColumn column in columns)
                table.Columns.Add(column);
        }

        public void RemoveColumn(DataTable table, string header)
        {
            try
            {
                table.Columns.Remove(header);
            }
            catch (ArgumentException)
            {
                return;
            }
        }

        public void RemoveColumnRange(DataTable table, string[] headers)
        {
            foreach (string header in headers)
                RemoveColumn(table, header);
        }
    }
}
