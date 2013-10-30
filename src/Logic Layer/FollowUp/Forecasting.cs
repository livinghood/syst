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
        public string IeProductName { get; set; }
        public string IeProductID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerID { get; set; }
        public int Amount { get; set; }
        public int Budget { get; set; }
        public int OutcomeMonth { get; set; }
        public int OutcomeAcc { get; set; }
        public int Reprocessed { get; set; }
        public int Trend { get; set; }
        public int? FormerPrognosis { get; set; }
        public int Forecast { get; set; }
        public int ForecastBudget { get; set; }
        public DateTime Date { get; set; }
    }
}
