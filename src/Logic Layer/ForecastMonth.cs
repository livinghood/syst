//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Logic_Layer
{
    using System;
    using System.Collections.Generic;
    
    public partial class ForecastMonth
    {
        public ForecastMonth()
        {
            this.ForecastMonitor = new HashSet<ForecastMonitor>();
        }
    
        public string ForecastMonitorMonthID { get; set; }
        public bool ForecastLock { get; set; }
    
        public virtual ICollection<ForecastMonitor> ForecastMonitor { get; set; }
    }
}
