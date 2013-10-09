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
        readonly ObservableCollection<ActivityDepartments> departmentsList = new ObservableCollection<ActivityDepartments>(Enum.GetValues(typeof(ActivityDepartments)).Cast<ActivityDepartments>());

        public ActivityManager()
        {
            InitializeComponent();

            cbActivityDepartment.ItemsSource = departmentsList;
        }

        public ActivityManager(Activity activity)
        {
            InitializeComponent();

            cbActivityDepartment.ItemsSource = departmentsList;

            DataContext = activity;

            cbActivityDepartment.SelectedIndex = 
                activity.DepartmentID.EndsWith("AO") ? 0 : 1;
        }
    }
}