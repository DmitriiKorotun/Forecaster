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

        public void FillColumn<T>(DataTable table, DataColumn column, IEnumerable<T> values)
        {
            int rowIndex = 0;

            foreach (DataRow row in table.Rows)
            {
                row[column.ColumnName] = values.ElementAt(rowIndex);

                ++rowIndex;
            }
        }

        public void FillColumn<T>(DataTable table, string columnHeader, IEnumerable<T> values)
        {
            int rowIndex = 0;

            foreach (DataRow row in table.Rows)
            {
                row[columnHeader] = values.ElementAt(rowIndex);

                ++rowIndex;
            }
        }

        public void FillColumnRange<T>(DataTable table, IEnumerable<DataColumn> columns, IEnumerable<T[]> values)
        {
            int columnIndex = 0;

            foreach(DataColumn column in columns)
            {
                IEnumerable<T> currentColValues = values.ElementAt(columnIndex).ToList();

                FillColumn(table, column, currentColValues);

                ++columnIndex;
            }
        }

        public void FillColumnRange<T>(DataTable table, IEnumerable<string> columnHeaders, IEnumerable<T[]> values)
        {
            int columnIndex = 0;

            foreach (string columnHeader in columnHeaders)
            {
                IEnumerable<T> currentColValues = values.ElementAt(columnIndex).ToList();

                FillColumn(table, columnHeader, currentColValues);

                ++columnIndex;
            }
        }

        //public void FillColumnRange(DataTable table, List<DataColumn> columns, IEnumerable<DatePart.DatePart> values)
        //{
        //    int columnIndex = 0;

        //    foreach (DataColumn column in columns)
        //    {
        //        IEnumerable<double> currentColValues = values.ElementAt(columnIndex).ToDoubleArray();

        //        FillColumn(table, column, currentColValues);

        //        ++columnIndex;
        //    }
        //}

        public void FillColumnRange(DataTable table, IEnumerable<DataColumn> columns, IEnumerable<DatePart.DatePart> values)
        {
            IEnumerable<int[]> castedValues = ToColumnOrientation(values);

            FillColumnRange(table, columns, castedValues);
        }

        public void FillColumnRange(DataTable table, IEnumerable<string> columnHeaders, IEnumerable<DatePart.DatePart> values)
        {
            IEnumerable<int[]> castedValues = ToColumnOrientation(values);

            FillColumnRange(table, columnHeaders, castedValues);
        }

        private IEnumerable<int[]> ToColumnOrientation(IEnumerable<DatePart.DatePart> values)
        {
            List<int[]> doubleValues = new List<int[]>
            {
                values.Select(a => a.Year).ToArray(),
                values.Select(a => a.Month).ToArray(),
                values.Select(a => a.DayOfYear).ToArray(),
                values.Select(a => a.DayOfMonth).ToArray(),
                values.Select(a => a.DayOfWeek).ToArray(),
                values.Select(a => a.IsMonthStart).ToArray(),
                values.Select(a => a.IsMonthEnd).ToArray(),
                values.Select(a => a.IsYearStart).ToArray(),
                values.Select(a => a.IsYearEnd).ToArray(),
                values.Select(a => a.IsMonOrFri).ToArray()
            };

            return doubleValues;
        }

        public void AddColumn(DataTable table, DataColumn column)
        {
            table.Columns.Add(column);
        }

        public void AddColumnRange(DataTable table, IEnumerable<DataColumn> columns)
        {
            foreach (DataColumn column in columns)
                AddColumn(table, column);
        }

        public void CreateColumn<T>(DataTable table, string header)
        {
            DataColumn newCol = new DataColumn(header, typeof(T))
            {
                AllowDBNull = true
            };

            AddColumn(table, newCol);
        }

        public void CreateColumnRange<T>(DataTable table, IEnumerable<string> headers)
        {
            foreach (string header in headers)
                CreateColumn<T>(table, header);
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

        public void RemoveColumnRange(DataTable table, IEnumerable<string> headers)
        {
            foreach (string header in headers)
                RemoveColumn(table, header);
        }
    }
}
