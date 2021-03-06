//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace Logic_Layer
{
    using System;
    using System.Collections.Generic;

    public partial class FinancialIncome : INotifyPropertyChanged
    {
        private string m_ProductID;
        private string m_ProductName;
        private string m_CustomerID;
        private string m_CustomerName;
        private int? m_Agreement;
        private bool m_GradeT;
        private bool m_GradeA;
        private int? m_Hours;
        private int? m_Addition;
        private string m_Comments;
        private string m_FinancialIncomeYearID;

        public string ProductID
        {
            get { return m_ProductID; }
            set { SetField(ref m_ProductID, value, "ProductID"); }
        }

        [NotMapped]
        public string ProductName
        {
            get { return m_ProductName; }
            set { SetField(ref m_ProductName, value, "ProductName"); }
        }

        [NotMapped]
        public Nullable<int> Budget { get { return Agreement + Addition; } }

        public string CustomerID
        {
            get { return m_CustomerID; }
            set { SetField(ref m_CustomerID, value, "CustomerID"); }
        }

         [NotMapped]
        public string CustomerName
         {
             get { return m_CustomerName; }
             set { SetField(ref m_CustomerName, value, "CustomerName"); }
         }

        public Nullable<int> Agreement
        {
            get { return m_Agreement; }

            set 
            { 
                SetField(ref m_Agreement, value, "Agreement");
                OnPropertyChanged("Budget");
            }
                
        }

        public bool GradeT
        {
            get { return m_GradeT; }
            set { SetField(ref m_GradeT, value, "GradeT"); }
        }

        public bool GradeA
        {
            get { return m_GradeA; }
            set { SetField(ref m_GradeA, value, "GradeA"); }
        }

        public Nullable<int> Hours
        {
            get { return m_Hours; }
            set { SetField(ref m_Hours, value, "Hours"); }
        }

        public Nullable<int> Addition
        {
            get { return m_Addition; }
            set
            {
                SetField(ref m_Addition, value, "Addition");
                OnPropertyChanged("Budget");
            }
        }

        public string Comments
        {
            get { return m_Comments; }
            set { SetField(ref m_Comments, value, "Comments"); }
        }
        public string FinancialIncomeYearID
        {
            get { return m_FinancialIncomeYearID; }
            set { SetField(ref m_FinancialIncomeYearID, value, "FinancialIncomeYearID"); }
        }

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

        public virtual Customer Customer { get; set; }
        public virtual FinancialIncomeYear FinancialIncomeYear { get; set; }
        public virtual Product Product { get; set; }
    }
}
