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

    public partial class Account : INotifyPropertyChanged
    {
        private int m_AccountID;
        private string m_AccountName;
        private int? m_AccountCost;


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

        public Account()
        {
            this.DirectActivityCost = new HashSet<DirectActivityCost>();
            this.DirectProductCost = new HashSet<DirectProductCost>();
        }

        public int AccountID
        {
            get { return m_AccountID; }
            set { SetField(ref m_AccountID, value, "AccountID"); }
        }
        public string AccountName
        {
            get { return m_AccountName; }
            set { SetField(ref m_AccountName, value, "AccountName"); }
        }
        public Nullable<int> AccountCost
        {
            get { return m_AccountCost; }
            set { SetField(ref m_AccountCost, value, "AccountCost"); }
        }
    
        public virtual ICollection<DirectActivityCost> DirectActivityCost { get; set; }
        public virtual ICollection<DirectProductCost> DirectProductCost { get; set; }
    }
}
