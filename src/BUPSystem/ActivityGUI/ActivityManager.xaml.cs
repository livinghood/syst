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

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ActivityManager()
        {
            // Items from IEnumerable must be placed in a List in order for the combobox index to work
            foreach (var item in departmentsList)
            {
                list.Add(item);
            }

            InitializeComponent();

            changeExistingActivity = false;

            cbActivityDepartment.ItemsSource = list;
        }

        /// <summary>
        /// Constructor called when editing an existing activity
        /// </summary>
        /// <param name="activity"></param>
        public ActivityManager(Activity activity)
        {   
            // Items from IEnumerable must be placed in a List in order for the combobox index to work
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

        /// <summary>
        /// Saves a new activity or changes made to an existing activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!changeExistingActivity)
            {
                ActivityManagement.Instance.CreateActivity(tbID.Text, tbName.Text,
                    list[cbActivityDepartment.SelectedIndex]);
            }
            else
            {
                ActivityManagement.Instance.Update();
            }  
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}