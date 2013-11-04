using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.FollowUp
{
    /// <summary>
    /// This class makes the basis for ForecastMonitor and ForecastMonth
    /// </summary>
    public class Forecasting : INotifyPropertyChanged
    {
        private int? m_trend;
        private int? m_Forecast;

        public string IeProductName { get; set; }
        public string IeProductID { get; set; }
        public int? Budget { get; set; }
        public int OutcomeMonth { get; set; }
        public int OutcomeAcc { get; set; }
        public int? Reprocessed { get; set; }
        public int? FormerPrognosis { get; set; }
        

        public event PropertyChangedEventHandler PropertyChanged;


        public int? Trend
        {
            get { return m_trend;}
            set { SetField(ref m_trend, value, "Trend"); }
        }

        public int? Forecast
        {
            get { return m_Forecast; }
            set { SetField(ref m_Forecast, value, "Forecast");
            OnPropertyChanged("ForecastBudget");
            }
        }

        public int? ForecastBudget
        {
            get { return m_Forecast - Budget; }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
