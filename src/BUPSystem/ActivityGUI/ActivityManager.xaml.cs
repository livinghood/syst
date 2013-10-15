using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;

namespace BUPSystem.ActivityGUI
{
    /// <summary>
    ///     Interaction logic for ActivityManagement.xaml
    /// </summary>
    public partial class ActivityManager : Window
    {
        public Logic_Layer.Activity Activity { get; set; }

        readonly ObservableCollection<string> departmentNames =
            new ObservableCollection<string>(ActivityManagement.Instance.GetDepartmentNames());

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ActivityManager()
        {
            InitializeComponent();
            cbActivityDepartment.ItemsSource = departmentNames;

            Logic_Layer.Activity activity = new Activity();
            DataContext = activity;
            Activity = activity;
        }

        /// <summary>
        /// Constructor called when editing an existing activity
        /// </summary>
        /// <param name="activity"></param>
        public ActivityManager(Activity activity)
        {   
            InitializeComponent();
            tbID.IsEnabled = false;
            cbActivityDepartment.ItemsSource = departmentNames;
            DataContext = activity;
            Activity = activity;
        }

        /// <summary>
        /// Saves a new activity or changes made to an existing activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            tbID.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            cbActivityDepartment.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();

            DialogResult = true;
            Close();
        }
    }
}