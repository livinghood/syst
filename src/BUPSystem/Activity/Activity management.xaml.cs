using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic_Layer.ActivityNamespace;
using Logic_Layer.CustomerNamespace;

namespace BUPSystem.Activity
{
    /// <summary>
    ///     Interaction logic for ActivityManagement.xaml
    /// </summary>
    public partial class ActivityManagement : Window
    {
        readonly List<ActivityDepartments> list = new List<ActivityDepartments>(Enum.GetValues(typeof(ActivityDepartments)).Cast<ActivityDepartments>());

        public ActivityManagement()
        {
            InitializeComponent();

            cbActivityDepartment.ItemsSource = list;
        }

        public ActivityManagement(Logic_Layer.ActivityNamespace.Activity activity)
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