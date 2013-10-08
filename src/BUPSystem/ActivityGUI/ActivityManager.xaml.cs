using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic_Layer.ActivityNamespace;

namespace BUPSystem.ActivityGUI
{
    /// <summary>
    ///     Interaction logic for ActivityManagement.xaml
    /// </summary>
    public partial class ActivityManager : Window
    {
        readonly List<ActivityDepartments> list = new List<ActivityDepartments>(Enum.GetValues(typeof(ActivityDepartments)).Cast<ActivityDepartments>());

        public ActivityManager()
        {
            InitializeComponent();

            cbActivityDepartment.ItemsSource = list;
        }

        public ActivityManager(Logic_Layer.ActivityNamespace.Activity activity)
        {
            InitializeComponent();

            DataContext = activity;

            cbActivityDepartment.ItemsSource = list;

            cbActivityDepartment.SelectedIndex = activity.ActivityDepartment == ActivityDepartments.FO ? 0 : 1;    
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}