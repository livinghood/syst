//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.ComponentModel;

namespace Logic_Layer
{
    using System;
    using System.Collections.Generic;

    public partial class Employee : INotifyPropertyChanged
    {
        private long m_EmployeeID;
        private string m_EmployeeName;
        private int m_MonthSallary;
        private int m_EmployeementRate;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public Employee()
        {
            this.ActivityPlacement = new HashSet<ActivityPlacement>();
            this.EmployeePlacement = new HashSet<EmployeePlacement>();
            this.ProductPlacement = new HashSet<ProductPlacement>();
            this.UserAccount = new HashSet<UserAccount>();
        }

        public long EmployeeID
        {
            get { return m_EmployeeID; }
            set { SetField(ref m_EmployeeID, value, "EmployeeID"); }
        }

        public string EmployeeName
        {
            get { return m_EmployeeName; }
            set { SetField(ref m_EmployeeName, value, "EmployeeName"); }
        }
        public int MonthSallary
        {
            get { return m_MonthSallary; }
            set { SetField(ref m_MonthSallary, value, "MonthSallary"); }
        }

        public int EmployeementRate
        {
            get { return m_EmployeementRate; }
            set { SetField(ref m_EmployeementRate, value, "EmployeementRate"); }
        }

        public Nullable<decimal> VacancyDeduction { get; set; }
    
        public virtual ICollection<ActivityPlacement> ActivityPlacement { get; set; }
        public virtual ICollection<EmployeePlacement> EmployeePlacement { get; set; }
        public virtual ICollection<ProductPlacement> ProductPlacement { get; set; }
        public virtual ICollection<UserAccount> UserAccount { get; set; }
    }
}
