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
        }

        public void FillForecastsFromDB(int month)
        {
            Forecasts.Clear();
            var icps = GetIncomeProductCustomers();

            foreach (var icp in icps)
            {
                CreateForecasting(icp, month);
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
                    ForecastMonitor fm = new ForecastMonitor
                    {
                        Reprocessed = forecast.Reprocessed,
                        OutcomeAcc = forecast.OutcomeAcc,
                        IeProductName = forecast.IeProductName,
                        IeProductID = forecast.IeProductID,
                        ForecastMonth = forecastMonth,
                        ForecastMonitorMonthID = month.ToString(CultureInfo.InvariantCulture),
                        ForecastBudget = forecast.ForecastBudget.ToString(),
                        Forecast = forecast.Forecast
                    };

                    var flist = db.ForecastMonitor.Select(f => f.IeProductID);

                    // If database already contains fm, update the attributes that a user can enter
                    if (flist.Contains(fm.IeProductID))
                    {
                        var itemToUpdate = db.ForecastMonitor.Single(s => s.IeProductID.Equals(fm.IeProductID));
                        itemToUpdate.Reprocessed = fm.Reprocessed;
                        itemToUpdate.Forecast = fm.Forecast;
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
                    //CreateForecasting(ipc);
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

        public IEnumerable<IncomeProductCustomer> GetIncomeProductCustomers()
        {
            return db.IncomeProductCustomer.OrderBy(f => f.IeProductID);
        }

        public void CreateForecasting(IncomeProductCustomer ipc, int chosenMonth)
        {
            Forecasting fc = new Forecasting
            {
                IeProductID = ipc.IeProductID,
                IeProductName = ipc.IeProductName,
                CustomerID = ipc.IeCustomerID,
                CustomerName = ipc.IeCustomerName,
                Amount = ipc.IeAmount,
                Date = ipc.IeIncomeDate,
                OutcomeAcc = ~ipc.IeAmount + 1,
                Trend = (~ipc.IeAmount + 1)/ipc.IeIncomeDate.Date.Month*12,
                FormerPrognosis = CalculateFormerPrognosis(ipc.IeIncomeDate.Date.Month, ipc.IeProductID),
                Forecast = GetForecastValue(ipc.IeIncomeDate.Date.Month, ipc.IeProductID),
                OutcomeMonth = CalculateOutcomeMonth(chosenMonth, ipc.IeProductID),
                Budget = GetBudgetFromFinancialIncome(ipc.IeProductID),
                Reprocessed = GetReprocessed(ipc.IeProductID)
            };
            fc.ForecastBudget = fc.Forecast - fc.Budget;

            // In case newly created forecast already exists, just update its properties
            foreach (var item in Forecasts.Where(item => item.IeProductID.Equals(fc.IeProductID)
                && item.Date.Month == fc.Date.Month))
            {
                item.Amount = fc.Amount;
                item.OutcomeAcc = ~fc.Amount + 1;
                item.Trend = (~fc.Amount + 1) / ipc.IeIncomeDate.Date.Month * 12;
                item.FormerPrognosis = fc.FormerPrognosis;
                item.Forecast = fc.Forecast;
                item.Reprocessed = fc.Reprocessed;
                item.Budget = fc.Budget;
                item.ForecastBudget = fc.ForecastBudget;
                item.Reprocessed = fc.Reprocessed;
                return;
            }
            Forecasts.Add(fc);
        }

        private int? GetReprocessed(string ieProductId)
        {
            int? valueToReturn = 0;

            foreach (var fm in db.ForecastMonitor)
            {
                if (fm.IeProductID.Equals(ieProductId))
                {
                    valueToReturn = fm.Reprocessed;
                }
            }
            return valueToReturn;
        }

        private int? GetBudgetFromFinancialIncome(string productID)
        {
            return Enumerable.Aggregate<FinancialIncome,int?>(db.FinancialIncome.Where
                (fi => fi.ProductID.Equals(productID)), 0, (current, fi) => current + fi.Budget);
        }

        private int CalculateOutcomeMonth(int month, string productId)
        {
            int outcomeAccThisMonth = 0;

            int minusMonth = month;
            if (minusMonth == 0)
                minusMonth = 12;

            var icps = from d in db.IncomeProductCustomer
                       where d.IeProductID.Equals(productId)
                       select d;

            List<string> tempCustomer = new List<string>();

            foreach (var icp in icps)
            {
                if (!tempCustomer.Contains(icp.IeCustomerID))
                {
                    int OutcomeMontCustomer = CalculateOutComeMonthCustomer(icp.IeCustomerID, icps, minusMonth);
                    tempCustomer.Add(icp.IeCustomerID);
                    outcomeAccThisMonth += OutcomeMontCustomer;
                }
            }

            return outcomeAccThisMonth;
        }

        private int CalculateOutComeMonthCustomer(string ieCustomerId, IEnumerable<IncomeProductCustomer> icps, int month)
        {
            List<IncomeProductCustomer> tempICP = new List<IncomeProductCustomer>
                (icps.Where(icp => icp.IeCustomerID.Equals(ieCustomerId) 
                    && icp.IeIncomeDate.Month <= month).OrderByDescending(s => s.IeIncomeDate));
      
            int y = 0;

            if (tempICP.Any())
            {
                if (tempICP[0].IeIncomeDate.Month.Equals(month))
                {
                    var tempAmount = new List<IncomeProductCustomer>();

                    if (tempICP.Count() > 2)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            tempAmount.Add(tempICP[i]);
                        }
                    }
                    else
                        tempAmount.AddRange(tempICP);

                    if (tempAmount.Count() >= 2)
                        y = (~tempAmount[0].IeAmount + 1) - (~tempAmount[1].IeAmount + 1);
                    else
                        y = ~tempAmount[0].IeAmount + 1;
                }
            }
            return y;
        }

        private int? GetForecastValue(int month, string productId)
        {
            int? forecast = 0;

            try
            {
                // Attempt to retrieve the 'former prognosis' value from the forecast value in a an item 
                // with the same product id from the month before current month
                string str = month.ToString(CultureInfo.InvariantCulture);

                var items = from s in db.ForecastMonitor
                            where s.ForecastMonitorMonthID.Equals(str)
                            select s;

                foreach (var item in items.Where(item => item.IeProductID.Equals(productId)))
                {
                    // Assign formerPrognosis the forecast value 
                    forecast = item.Forecast;
                }
            }
            catch (Exception)
            {
                forecast = 0;
            }
            return forecast;
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
            Forecasts.Single(f => f.Equals(forecast)).Trend = forecast.Trend;
        }

        /// <summary>
        /// Locks a forecast
        /// </summary>
        /// <param name="month"></param>
        public void LockForecast(int month)
        {
            foreach (var forecast in db.ForecastMonitor.Where(forecast =>
                forecast.ForecastMonitorMonthID.Equals(month.ToString(CultureInfo.InvariantCulture))
                && forecast.ForecastMonth.ForecastLock == false))
            {
                forecast.ForecastMonth.ForecastLock = true;
            }
        }
    }
}
