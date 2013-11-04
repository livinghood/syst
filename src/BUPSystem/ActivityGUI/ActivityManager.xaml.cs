using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Logic_Layer;

namespace BUPSystem.ActivityGUI
{
    /// <summary>
    ///     Interaction logic for ActivityManagement.xaml
    /// </summary>
    public partial class ActivityManager : Window
    {
        public Logic_Layer.Activity Activity { get; set; }

        readonly ObservableCollection<string> departmentNames = new ObservableCollection<string>(ActivityManagement.Instance.GetDepartmentNames());

        private string m_partActivityID;

        public string PartActivityID
        {
            get { return m_partActivityID; }
            set
            {
                m_partActivityID = value;
                // Update the main product ID
                updateActivityID();
            }
        }

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

            PartActivityIDGrid.DataContext = this;
        }

        /// <summary>
        /// Constructor called when editing an existing activity
        /// </summary>
        /// <param name="activity"></param>
        public ActivityManager(Activity activity)
        {   
            InitializeComponent();
            cbActivityDepartment.IsEnabled = false;
            tbPartID.IsEnabled = false;
            tbId.IsEnabled = false;
            // Hide the partial ID
            PartActivityIDGrid.Visibility = Visibility.Collapsed;

            cbActivityDepartment.ItemsSource = departmentNames;
            DataContext = activity;
            Activity = activity;

            // Disable validation for product id and part-id
            Binding binding = BindingOperations.GetBinding(tbId, TextBox.TextProperty);
            Binding partIDbinding = BindingOperations.GetBinding(tbPartID, TextBox.TextProperty);
            binding.ValidationRules.Clear();
            partIDbinding.ValidationRules.Clear();

        }

        /// <summary>
        /// Saves a new activity or changes made to an existing activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Update the product ID
        /// </summary>
        private void updateActivityID()
        {
            // If no part-id or productgroup is set, set to null
            if (!String.IsNullOrEmpty(m_partActivityID) && !String.IsNullOrEmpty(Activity.DepartmentID))
            {
                Activity.ActivityID = m_partActivityID.ToUpper() + Activity.DepartmentID.ToUpper();
            }
            else
            {
                Activity.ActivityID = null;
            }
        }

        private void cbActivityDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            updateActivityID();
        }
    }
}