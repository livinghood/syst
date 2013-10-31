using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Logic_Layer.FollowUp
{
    public enum Months
    {
        Alla,
        Januari,
        Februari,
        Mars,
        April,
        Maj,
        Juni,
        Juli,
        Augusti,
        September,
        Oktober,
        November,
        December
    }

    public class ForecastingManagement
    {
        public ObservableCollection<Forecasting> Forecasts { get; set; }

        /// <summary>
        /// Lazy Instance of ForecastingManagement singelton
        /// </summary>
        private static readonly Lazy<ForecastingManagement> instance = new Lazy<ForecastingManagement>(() => new ForecastingManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static ForecastingManagement Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Constructor with initialization of forecasts list
        /// </summary>
        private ForecastingManagement()
        {
            Forecasts = new ObservableCollection<Forecasting>();
            FillForecastsFromDB();
        }

        public void FillForecastsFromDB()
        {
            Forecasts.Clear();
            var icps = GetIncomeProductCustomers();

            foreach (var icp in icps)
            {
                CreateForecasting(icp);
            }
        }

        /// <summary>
        /// Add forecast to database or save changes made to a forecast that is already added to the database
        /// </summary>
        public void AddForecast(int month)
        {
            ForecastMonth forecastMonth = new ForecastMonth
            {
                ForecastLock = false,
                ForecastMonitorMonthID = month.ToString(CultureInfo.InvariantCulture)
            };

            var mList = db.ForecastMonth.Select(m => m.ForecastMonitorMonthID);

            // Add the forecastMonitor to database if not already added
            if (!mList.Contains(forecastMonth.ForecastMonitorMonthID))
            {
                db.ForecastMonth.Add(forecastMonth);
            }


            foreach (var forecast in Forecasts)
            {
                if (forecast.Date.Month == month)
                {
                    ForecastMonitor fm = new ForecastMonitor();
                    fm.Reprocessed = forecast.Reprocessed;
                    fm.OutcomeAcc = forecast.OutcomeAcc;
                    fm.IeProductName = forecast.IeProductName;
                    fm.IeProductID = forecast.IeProductID;
                    fm.ForecastMonth = forecastMonth;
                    fm.ForecastMonitorMonthID = month.ToString(CultureInfo.InvariantCulture);
                    fm.ForecastBudget = forecast.ForecastBudget.ToString(CultureInfo.InvariantCulture);
                    fm.Forecast = forecast.Forecast;

                    var flist = db.ForecastMonitor.Select(f => f.IeProductID);

                    // If database already contains fm, update the attributes that a user can enter
                    if (flist.Contains(fm.IeProductID))
                    {
                        db.ForecastMonitor.Single(s => s.IeProductID.Equals(fm.IeProductID)).Reprocessed += fm.Reprocessed;
                        db.ForecastMonitor.Single(s => s.IeProductID.Equals(fm.IeProductID)).Forecast += fm.Forecast;
                    }
                    else
                        db.ForecastMonitor.Add(fm);
                }
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Update the database
        /// </summary>
        public void UpdateForecast()
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Creates a forecasting from the imported file IntaktProduktKund.txt
        /// </summary>
        /// <param name="fileName"></param>
        public void CreateForecastFromFile(string fileName)
        {
            // First, delete all items in IncomeProductCustomer
            db.IncomeProductCustomer.RemoveRange(db.IncomeProductCustomer);
            db.SaveChanges();
            Forecasts.Clear();

            using (var reader = new StreamReader(fileName))
            {
                // Ignore first row since it's a header
                reader.ReadLine();
                string row;
                while ((row = reader.ReadLine()) != null)
                {
                    /* IntaktProduktKund.txt is formatted in such a way that there are up to three tabs separating each
                     * 'column' in the text file. If a row contains multiple tabs they are replaced with one. */
                    if (row.Contains("\t\t\t"))
                    {
                        row = row.Replace("\t\t\t", "\t");
                    }

                    if (row.Contains("\t\t"))
                    {
                        row = row.Replace("\t\t", "\t");
                    }

                    // At this point each column is only separated by one tab which makes it easy to read the file
                    string[] field = row.Split('\t');

                    IncomeProductCustomer ipc = new IncomeProductCustomer
                    {
                        IeProductID = field[0],
                        IeProductName = field[1],
                        IeCustomerID = field[2],
                        IeCustomerName = field[3],
                        IeAmount = int.Parse(field[5]),
                        IeIncomeDate = DateTime.ParseExact(field[4], "yyyyMMdd", CultureInfo.InvariantCulture)
                    };

                    // Add icp to database
                    AddIncomeProductCustomer(ipc);
                    CreateForecasting(ipc);
                }
            }
        }

        private void AddIncomeProductCustomer(IncomeProductCustomer ipc)
        {
            db.IncomeProductCustomer.Add(ipc);
            db.SaveChanges();
        }

        /// <summary>
        /// Returns a list of months
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Months> GetMonths()
        {
            return new ObservableCollection<Months>(Enum.GetValues(typeof(Months)).Cast<Months>());
        }

        /// <summary>
        /// Allows for selection of forecasts from a specific month
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public IEnumerable<Forecasting> GetForecastFromMonth(int month)
        {
            return month == 0 ? Forecasts : Forecasts.Where(m => m.Date.Month == month);
        }

        public IEnumerable<ForecastMonitor> GetForecastMonitor()
        {
            return db.ForecastMonitor.OrderBy(f => f.IeProductID);
        }
        public IEnumerable<IncomeProductCustomer> GetIncomeProductCustomers()
        {
            return db.IncomeProductCustomer.OrderBy(f => f.IeProductID);
        }

        public void CreateForecasting(IncomeProductCustomer ipc)
        {
            Forecasting fc = new Forecasting();
            // fc.Forecast =                            Måste finnas med
            fc.IeProductID = ipc.IeProductID;
            fc.IeProductName = ipc.IeProductName;
            fc.CustomerID = ipc.IeCustomerID;
            fc.CustomerName = ipc.IeCustomerName;
            fc.Amount = ipc.IeAmount;
            fc.Date = ipc.IeIncomeDate;
            fc.OutcomeAcc = ~ipc.IeAmount + 1;
            fc.Trend = (~ipc.IeAmount + 1) / ipc.IeIncomeDate.Date.Month * 12;
            fc.FormerPrognosis = CalculateFormerPrognosis(ipc.IeIncomeDate.Date.Month, ipc.IeProductID);

            // In case newly created forecast already exists, just update its properties
            foreach (var item in Forecasts)
            {
                if (item.IeProductID.Equals(fc.IeProductID) && item.Date.Month == fc.Date.Month)
                {
                    item.Amount = fc.Amount;
                    item.OutcomeAcc = ~fc.Amount + 1;
                    item.Trend = (~fc.Amount + 1) / ipc.IeIncomeDate.Date.Month * 12;
                    item.FormerPrognosis = fc.FormerPrognosis;
                    item.Forecast = fc.Forecast;
                    item.Reprocessed = fc.Reprocessed;
                    return;
                }
            }

            Forecasts.Add(fc);
        }

        private int? CalculateFormerPrognosis(int month, string productID)
        {
            int minusMonth = 1;
            if (month == 1)
                minusMonth = 13;

            int? formerPrognosis = 0;

            try
            {
                // Attempt to retrieve the 'former prognosis' value from the forecast value in a an item 
                // with the same product id from the month before current month
                int monthValue = month - minusMonth;
                string str = monthValue.ToString(CultureInfo.InvariantCulture);

                var items = from s in db.ForecastMonitor
                            where s.ForecastMonitorMonthID.Equals(str)
                            select s;

                foreach (var item in items.Where(item => item.IeProductID.Equals(productID)))
                {
                    // Assign formerPrognosis the forecast value 
                    formerPrognosis = item.Forecast;
                }
            }
            catch (Exception)
            {
                formerPrognosis = 0;
            }

            return formerPrognosis;
        }

        /// <summary>
        /// Calculates the trend in a specific forecast object
        /// </summary>
        /// <param name="forecast"></param>
        /// <param name="passedMonths"></param>
        public void CalculateTrend(Forecasting forecast, int passedMonths)
        {
            forecast.Trend = (forecast.OutcomeAcc + forecast.Reprocessed) / passedMonths * 12;
        }

        public void Calculate(Forecasting forecast, int selectedMonth)
        {
            int minusMonth = 1;
            if (forecast.Date.Month == 1)
                minusMonth = 13;

            var itemFromPreviousMonth = from f in Forecasts
                                        where f.IeProductID.Equals(forecast.IeProductID)
                                        && f.Date.Month == forecast.Date.Month - minusMonth
                                        select f;

            //forecast.OutcomeMonth = forecast.OutcomeAcc - 
        }


        public void LockForecast(int month)
        {
            foreach (var forecast in db.ForecastMonitor.Where(forecast =>
                forecast.ForecastMonitorMonthID.Equals(month.ToString(CultureInfo.InvariantCulture)) &&
                forecast.ForecastMonth.ForecastLock == false))
            {
                forecast.ForecastMonth.ForecastLock = true;
            }
        }
    }
}
