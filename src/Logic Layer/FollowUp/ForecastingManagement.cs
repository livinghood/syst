﻿using System;
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

        /// <summary>
        /// Add forecast to database
        /// </summary>
        /// <param name="forecast"></param>
        public void AddForecast(Forecasting forecast)
        {
            ForecastMonth forecastMonth = new ForecastMonth
            {
                ForecastLock = false,
                ForecastMonitorID = GetNewForecastMonthID()
            };

            ForecastMonitor forecastMonitor = new ForecastMonitor
            {
                Forecast = forecast.Prognosis,
                // TODO Är forecastBudget datatyp i databasen felaktig? + Är namnet ForecastMonitorID i ForecastMonth-tabellen fel? 
                ForecastBudget = forecast.PrognosisBudget.ToString(CultureInfo.InvariantCulture),
                ForecastMonitorID = GetNewForecastMonitorID(),
                IeProductID = forecast.ProductID,
                IeProductName = forecast.ProductName,
                OutcomeAcc = forecast.OutcomeAcc,
                Reprocessed = forecast.Reprocessed,
                ForecastMonth = forecastMonth
            };

            var fList = db.ForecastMonitor.Select(f => f.IeProductID);
           
            // Add the forecastMonitor to database if not already added
            if (!fList.Contains(forecastMonitor.IeProductID))
            {
                db.ForecastMonitor.Add(forecastMonitor);
            }

            var mList = db.ForecastMonth.Select(m => m.ForecastMonitorID);
           
            // Add the forecastMonitor to database if not already added
            if (!mList.Contains(forecastMonitor.ForecastMonitorID))
            {
                db.ForecastMonth.Add(forecastMonth);
            }
             
            db.SaveChanges();
        }

        /// <summary>
        /// Returns a new ForecastMonitorID
        /// </summary>
        /// <returns></returns>
        private int GetNewForecastMonitorID()
        {
            var ids = from f in db.ForecastMonitor
                      orderby f.ForecastMonitorID
                      select f.ForecastMonitorID;

            List<int> list = new List<int>(ids);

            // If the database doesn't contain anything in forecastMontitor return 100 as a new id, else return the id of last element + 1. 
            int id = 100;

            if (ids.Any())
            {
                id = list.Last();
                id++;
            }
            return id;
        }

        /// <summary>
        /// Returns a new Returns a new ForecastMonthID
        /// </summary>
        /// <returns></returns>
        private int GetNewForecastMonthID()
        {
            var ids = from f in db.ForecastMonth
                      orderby f.ForecastMonitorID
                      select f.ForecastMonitorID;

            List<int> list = new List<int>(ids);

            // If the database doesn't contain anything in forecastMonth return 100 as a new id, else return the id of last element + 1. 
            int id = 100;

            if (ids.Any())
            {
                id = list.Last();
                id++;
            }
            return id;
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

                    string productID = field[0];
                    string productName = field[1];
                    string customerID = field[2];
                    string customerName = field[3];
                    string date = field[4];
                    string amount = field[5];

                    Forecasting forecast = new Forecasting(productID, productName, customerID, customerName, date, amount);

                    // Add the created forecast to the list of forecasts
                    Forecasts.Add(forecast);

                    // Add the created forecast to database
                    AddForecast(forecast);
                }
            }
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

        /// <summary>
        /// Calculates the trend in a specific forecast object
        /// </summary>
        /// <param name="forecast"></param>
        /// <param name="passedMonths"></param>
        public void CalculateTrend(Forecasting forecast, int passedMonths)
        {
            forecast.Trend = (forecast.OutcomeAcc + forecast.Reprocessed) / passedMonths * 12;
        }
    }
}