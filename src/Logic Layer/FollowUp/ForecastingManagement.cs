using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

        /// <summary>
        /// Create a new account
        /// </summary>
        public void CreateForecast(Forecasting forecast)
        {
            //Accounts.Add(account);
            //db.Account.Add(account);
            //db.SaveChanges();
        }       

        /// <summary>
        /// Update the database
        /// </summary>
        public void UpdateAccount()
        {
            db.SaveChanges();
        }

        public void CreateForecastFromFile(string fileName)
        {     
            using (var reader = new StreamReader(fileName))
            {
                reader.ReadLine();
                string row;
                while ((row = reader.ReadLine()) != null)
                {
                    string str = row;

                    /* IntaktProduktKund.txt is formatted in such a way that there are up to three tabs separating each
                     * 'column' in the text file. If a row contains multiple tabs they are replaced with one. */                   
                    if (row.Contains("\t\t\t"))
                    {
                        str = str.Replace("\t\t\t", "\t");
                    }

                    if (str.Contains("\t\t"))
                    {
                        str = str.Replace("\t\t", "\t");
                    }

                    // At this point each column is only separated by one tab, making it easy to read the file
                    string[] field = str.Split('\t');     

                    string productID = field[0];
                    string productName = field[1];
                    string customerID = field[2];
                    string customerName = field[3];
                    string date = field[4];
                    string amount = field[5];

                    // Create a new forecast
                    Forecasting forecast = new Forecasting(productID, productName,customerID, customerName, date, amount);

                    // Add the created forecast to the list of forecasts
                    Forecasts.Add(forecast);
                }
            }
        }

        public ObservableCollection<Months> GetMonths()
        {
            return new ObservableCollection<Months>(Enum.GetValues(typeof(Months)).Cast<Months>());
        }

        public IEnumerable<Forecasting> GetForecastFromMonth(Months month)
        {
            switch (month)
            {
                case Months.Alla:
                    return Forecasts;
                case Months.Januari:
                    return Forecasts.Where(m => m.Date.Month == 1);
                case Months.Februari:
                    return Forecasts.Where(m => m.Date.Month == 2);
                case Months.Mars:
                    return Forecasts.Where(m => m.Date.Month == 3);
                case Months.April:
                    return Forecasts.Where(m => m.Date.Month == 4);
                case Months.Maj:
                    return Forecasts.Where(m => m.Date.Month == 5);
                case Months.Juni:
                    return Forecasts.Where(m => m.Date.Month == 6);
                case Months.Juli:
                    return Forecasts.Where(m => m.Date.Month == 7);
                case Months.Augusti:
                    return Forecasts.Where(m => m.Date.Month == 8);
                case Months.September:
                    return Forecasts.Where(m => m.Date.Month == 9);
                case Months.Oktober:
                    return Forecasts.Where(m => m.Date.Month == 10);
                case Months.November:
                    return Forecasts.Where(m => m.Date.Month == 11);
                case Months.December:
                    return Forecasts.Where(m => m.Date.Month == 12);
                default: 
                    return Forecasts;
            }
        }

        public void Calculate(Forecasting forecast, int passedMonths)
        {
            forecast.Trend = (forecast.OutcomeAcc + forecast.Reprocessed)/passedMonths*12;
        }
    }
}
