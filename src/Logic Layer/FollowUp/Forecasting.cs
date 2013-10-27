using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.FollowUp
{
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

        //public Forecasting(Product product, int budget, int outcomeMoth, int outcomeAcc, int reprocessed, int trend, int formerPrognosis,
        //    int prognosis, int prognosisBudget, DateTime date)
        //{
        //    Product = product;
        //    Budget = budget;
        //    OutcomeMonth = outcomeMoth;
        //    OutcomeAcc = outcomeAcc;
        //    Reprocessed = reprocessed;
        //    Trend = trend;
        //    FormerPrognosis = formerPrognosis;
        //    Prognosis = prognosis;
        //    PrognosisBudget = prognosisBudget;
        //    Date = date;
        //}

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
