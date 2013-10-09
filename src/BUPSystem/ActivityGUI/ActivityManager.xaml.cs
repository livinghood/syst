using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.ActivityGUI
{
    /// <summary>
    ///     Interaction logic for ActivityManagement.xaml
    /// </summary>
    public partial class ActivityManager : Window
    {
        private readonly IEnumerable<string> departmentsList = ActivityManagement.Instance.GetDepartmentNames();
        private bool changeExistingActivity;

        ObservableCollection<string> list = new ObservableCollection<string>();

        public ActivityManager()
        {
         

            foreach (var item in departmentsList)
            {
                list.Add(item);
            }


            InitializeComponent();

            changeExistingActivity = false;

            cbActivityDepartment.ItemsSource = list;
        }

        public ActivityManager(Activity activity)
        {           
            foreach (var item in departmentsList)
            {
                list.Add(item);
            }


            InitializeComponent();

            changeExistingActivity = true;

            cbActivityDepartment.ItemsSource = list;

            DataContext = activity;

            cbActivityDepartment.SelectedIndex = 
                activity.DepartmentID.EndsWith("AO") ? 0 : 1;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!changeExistingActivity)
            {
                ActivityManagement.Instance.CreateActivity(tbID.Text, tbName.Text,
                    list[cbActivityDepartment.SelectedIndex]);
            }
            else
            {
                CustomerManagement.Instance.UpdateCustomer();
            }  
        }
    }
}