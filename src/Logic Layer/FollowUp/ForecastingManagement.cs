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
        Välj = 0,
        Januari = 01,
        Februari = 02,
        Mars = 03,
        April = 04,
        Maj = 05,
        Juni = 06,
        Juli = 07,
        Augusti = 08,
        September = 09,
        Oktober = 10,
        November = 11,
        December = 12
    }

    public class ForecastingManagement
    {
        public ObservableCollection<Forecasting> Forecasts { get; set; }

        public ObservableCollection<IncomeProductCustomer> ICPs { get; set; }

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

        /// <summary>
        /// Add forecast to database or save changes made to a forecast that is already added to the database
        /// </summary>
        public ForecastMonth AddForecastMonth(DateTime month)
        {
            string monthID = month.ToString("yyyyMM");
            //Check if forecastmonth exists
            ForecastMonth fcm = db.ForecastMonth.SingleOrDefault(f => f.ForecastMonitorMonthID == monthID);
            if (fcm == null)
            {
                // Create forecastmonth object
                fcm = new ForecastMonth
                {
                    ForecastLock = false,
                    ForecastMonitorMonthID = monthID
                };
                // Add object to db
                db.ForecastMonth.Add(fcm);

                db.SaveChanges();
            }
            return fcm;
        }

        /// <summary>
        /// Add forecast to database or save changes made to a forecast that is already added to the database
        /// </summary>
        public void AddForecastMonitor(ForecastMonitor obj)
        {
            db.ForecastMonitor.Add(obj);
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
            // First, delete all forecast related items in db
            db.ForecastMonitor.RemoveRange(db.ForecastMonitor);
            db.ForecastMonth.RemoveRange(db.ForecastMonth);
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
        /// Get all IPC
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IncomeProductCustomer> GetIncomeProductCustomers()
        {
            return db.IncomeProductCustomer.OrderBy(f => f.IeProductID);
        }

        /// <summary>
        /// Get IPCs by month
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public IEnumerable<IncomeProductCustomer> GetIPCsByMonth(DateTime month)
        {
            return from i in db.IncomeProductCustomer
                   where i.IeIncomeDate.Month == month.Month
                   where i.IeIncomeDate.Year == month.Year
                   select i;
        }

        /// <summary>
        /// Get IPCs by month
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public IEnumerable<IncomeProductCustomer> GetAllIPCs()
        {
            return from i in db.IncomeProductCustomer
                   select i;
        }

        /// <summary>
        /// Get IPCs by month
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProducts()
        {
            return from i in db.Product
                   select i;
        }

        public void GetForecastsFromMonth(DateTime month,bool getAll)
        {
            List<IncomeProductCustomer> IPCs;
            if (getAll)
            {
                // Hämta inte från månad utan från alla
                IPCs = new List<IncomeProductCustomer>(GetAllIPCs());

                // Lägg till produkter från vårat system
                var AllProducts = GetAllProducts();

                // Lägg till alla produkter från systemet genom att fejka tomma ICPs
                // (Fulhack)
                foreach (var productObj in AllProducts)
                {


                    IncomeProductCustomer newIPC = new IncomeProductCustomer
                    {
                        IeProductID = productObj.ProductID,
                        IeCustomerID = "NULL",
                        IeCustomerName = "NULL",
                        IeIncomeDate = new DateTime(1973, 10, 24),
                        IeProductName = productObj.ProductName
                    };

                    IPCs.Add(newIPC);
                }

                // Order by product name
                IPCs = IPCs.OrderBy(p => p.IeProductName).ToList();
            }
            else
            {
                IPCs = new List<IncomeProductCustomer>(GetIPCsByMonth(month));
            }
            


            // Aj, inte så fin lista på redan tillagda produkter (då ska inte IPCn läggas in igen)
            List<string> tempProdukter = new List<string>();


            foreach (var IPC in IPCs)
            {
                if (!tempProdukter.Contains(IPC.IeProductID))
                {
                    Forecasting fc = new Forecasting
                    {
                        IeProductID = IPC.IeProductID,
                        IeProductName = IPC.IeProductName,
                        OutcomeAcc = CalculateUtfallAcc(month, IPC.IeProductID),
                        Trend = ((CalculateUtfallAcc(month, IPC.IeProductID) + GetReprocessedValue(month, IPC.IeProductID)) / month.Month) * 12,
                        FormerPrognosis = GetFormerForecastValue(month, IPC.IeProductID),
                        Forecast = GetForecastValue(month, IPC.IeProductID),
                        OutcomeMonth = CalculateOutcomeMonth(month.Month, IPC.IeProductID),
                        Budget = GetBudgetFromFinancialIncome(IPC.IeProductID),
                        Reprocessed = GetReprocessedValue(month, IPC.IeProductID)
                    };

               

                    // Add to the returning list
                    Forecasts.Add(fc);

                    // Vi ska inte använda den här igen
                    tempProdukter.Add(IPC.IeProductID);
                }
            }
        }


        public ForecastMonitor ForecastMonitorExist(string id, DateTime month)
        {
            string monthID = month.ToString("yyyyMM");

            return db.ForecastMonitor.SingleOrDefault(f => f.IeProductID == id && f.ForecastMonitorMonthID == monthID);
        }

        private int? GetBudgetFromFinancialIncome(string productID)
        {
            // Hämta alla financial incomes
            var financialIncomes = from f in db.FinancialIncome
                                   where f.ProductID.Equals(productID)
                                   select f;

            int budget = 0;

            //Summerize the budgets
            foreach (FinancialIncome fi in financialIncomes)
            {
                budget = +(int)fi.Budget;
            }

            return budget;
        }

        // Calculate utfall acc
        private int CalculateUtfallAcc(DateTime month, string productId)
        {
            int outcomeAcc = 0;

            // Get all incomes
            var icps = from d in db.IncomeProductCustomer
                       where d.IeProductID.Equals(productId)
                       where d.IeIncomeDate.Month <= month.Month
                       where d.IeIncomeDate.Year <= month.Year
                       orderby d.IeIncomeDate.Month descending
                       select d;

            List<string> tempCustomer = new List<string>();

            foreach (var icp in icps)
            {
                if (!tempCustomer.Contains(icp.IeCustomerID))
                {
                    tempCustomer.Add(icp.IeCustomerID);
                    outcomeAcc += ~icp.IeAmount + 1;
                }
                
            }

            return outcomeAcc;
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

                    if (tempAmount.Count() == 2)
                        y = (~tempAmount[0].IeAmount + 1) - (~tempAmount[1].IeAmount + 1);
                    else
                        y = ~tempAmount[0].IeAmount + 1;
                }
            }
            return y;
        }

        /// <summary>
        /// Get saved "Prognos" values
        /// </summary>
        /// <param name="month"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        private int? GetForecastValue(DateTime month, string productId)
        {
            string monthID = month.ToString("yyyyMM");
            //Return forecastmonitor (saved row values)
            var fm = db.ForecastMonitor.FirstOrDefault(f => f.IeProductID == productId && f.ForecastMonitorMonthID == monthID);

            if (fm != null)
            {
                return fm.Forecast;
            }
            else
            {
                return 0;
            }
            
        }

        /// <summary>
        /// Get forecast value from last month
        /// </summary>
        /// <param name="month"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        private int? GetFormerForecastValue(DateTime month, string productId)
        {
            string monthID = month.AddMonths(-1).ToString("yyyyMM");
            //Return forecastmonitor (saved row values)
            var fm = db.ForecastMonitor.FirstOrDefault(f => f.IeProductID == productId && f.ForecastMonitorMonthID == monthID);

            if (fm != null)
            {
                return fm.Forecast;
            }
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// Get saved "Upparbetat" values
        /// </summary>
        /// <param name="month"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        private int? GetReprocessedValue(DateTime month, string productId)
        {
            string monthID = month.ToString("yyyyMM");

            //Return forecastmonitor (saved row values)
            ForecastMonitor test = db.ForecastMonitor.FirstOrDefault(f => f.IeProductID == productId && f.ForecastMonitorMonthID == monthID);

            return test != null ? test.Reprocessed : 0;
        }

        /// <summary>
        /// Calculates the trend in a specific forecast object
        /// </summary>
        /// <param name="forecast"></param>
        /// <param name="passedMonths"></param>
        public void CalculateTrend(Forecasting forecast, int passedMonths)
        {
            forecast.Trend = ((forecast.OutcomeAcc + forecast.Reprocessed) / passedMonths) * 12;
            Forecasts.Single(f => f.Equals(forecast)).Trend = forecast.Trend;
        }

        /// <summary>
        /// Locks a forecast
        /// </summary>
        /// <param name="month"></param>
        public void LockForecast(DateTime month)
        {
            string monthID = month.ToString("yyyyMM");

            var Forecast = db.ForecastMonth.FirstOrDefault(f => f.ForecastMonitorMonthID.Equals(monthID));

            Forecast.ForecastLock = true;

            db.SaveChanges();

        }

        public bool CheckIfLocked(DateTime month)
        {
            string monthID = month.ToString("yyyyMM");

            return db.ForecastMonitor.Any() && Enumerable.FirstOrDefault((
                from item in db.ForecastMonitor
                where item.ForecastMonitorMonthID.Equals(monthID)
                select item.ForecastMonth.ForecastLock));
        }

        public int GetIncomeByProduct(string productID)
        {
            int outcomeAcc = 0;

            var icps = from d in db.IncomeProductCustomer
                       where d.IeProductID.Equals(productID)
                       orderby d.IeIncomeDate.Month descending
                       select d;

            List<string> tempCustomer = new List<string>();

            foreach (var icp in icps.Where(icp => !tempCustomer.Contains(icp.IeCustomerID)))
            {
                tempCustomer.Add(icp.IeCustomerID);
                outcomeAcc += ~icp.IeAmount + 1;
            }

            return outcomeAcc;
        }

    }
}
