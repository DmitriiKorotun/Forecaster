using Accord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecaster.DataHandlers.DatePart
{
    public class DatePart
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int DayOfYear { get; set; }
        public int DayOfMonth { get; set; }
        public int DayOfWeek { get; set; }
        public int IsMonthStart { get; set; }
        public int IsMonthEnd { get; set; }
        public int IsYearStart { get; set; }
        public int IsYearEnd { get; set; }
        public int IsMonOrFri { get; set; }

        public DatePart(DateTime date)
        {
            InitValues(date);
        }

        public static IEnumerable<DatePart> CreateFromDates(IEnumerable<DateTime> dates)
        {
            List<DatePart> dateParts = new List<DatePart>(dates.Count());

            int dateIndex = 0;

            foreach (DateTime date in dates) 
            {
                dateParts.Add(new DatePart(dates.ElementAt(dateIndex)));

                ++dateIndex;
            }

            return dateParts;
        }

        public int[] ToArray()
        {
            return new int[] { Year, Month, DayOfYear, DayOfMonth,
            DayOfWeek, IsMonthStart, IsMonthEnd, IsYearStart, IsYearEnd, IsMonOrFri };
        }

        public double[] ToDoubleArray()
        {
            return new double[] { Year, Month, DayOfYear, DayOfMonth,
            DayOfWeek, IsMonthStart, IsMonthEnd, IsYearStart, IsYearEnd, IsMonOrFri };
        }

        private void InitValues(DateTime date)
        {
            Year = date.Year;
            Month = date.Month;
            DayOfYear = date.DayOfYear;
            DayOfMonth = date.Day;
            DayOfWeek = (int)date.DayOfWeek;
            IsMonthStart = IsValueLess(DayOfMonth, 4);
            IsMonthEnd = IsValueMore(DayOfMonth, 26);
            IsYearStart = IsValueLess(DayOfYear, 20);
            IsYearEnd = IsValueMore(DayOfYear, 345);
            IsMonOrFri = IsDayOneOfNeeded(DayOfWeek, (int)System.DayOfWeek.Friday, (int)System.DayOfWeek.Monday);
        }

        private int IsValueMore(int value, int comparable)
        {
            return value > comparable ? 1 : 0;
        }

        private int IsValueLess(int value, int comparable)
        {
            return value < comparable ? 1 : 0;
        }

        private int IsDayOneOfNeeded(int dayOfWeek, params int[] neededDaysOfWeek)
        {
            return neededDaysOfWeek.Contains(dayOfWeek) ? 1 : 0;
        }
    }
}
