using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.FollowUp
{
    /// <summary>
    /// This class makes the basis for ForecastMonitor and ForecastMonth
    /// </summary>
    public class Forecasting
    {
        //public Product Product { get; set; }
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerID { get; set; }
        public int Amount { get; set; }
        public int Budget { get; set; }
        public int OutcomeMonth { get; set; }
        public int OutcomeAcc { get; set; }
        public int Reprocessed { get; set; }
        public int Trend { get; set; }
        public int FormerPrognosis { get; set; }
        public int Prognosis { get; set; }
        public int PrognosisBudget { get; set; }
        public DateTime Date { get; set; }

        /// <summary>
        /// Constructor used to create a new forecasting from data read in file 'IntaktProduktKund.txt'
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productName"></param>
        /// <param name="customerID"></param>
        /// <param name="customerName"></param>
        /// <param name="date"></param>
        /// <param name="amount"></param>
        public Forecasting(string productId, string productName, string customerID, string customerName, string date, string amount)
        {
            ProductID = productId;
            ProductName = productName;
            CustomerID = customerID;
            CustomerName = customerName;
            Date = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            Amount = int.Parse(amount);
        }
    }
}
