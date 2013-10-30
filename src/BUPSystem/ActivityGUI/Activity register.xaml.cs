using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System;
using Logic_Layer;
using Logic_Layer.General_Logic;

namespace BUPSystem.ActivityGUI
{
    /// <summary>
    /// Interaction logic for ActivityRegister.xaml
    /// </summary>
    public partial class ActivityRegister : Window
    {
        public ObservableCollection<Activity> ActivityList
        {
            get { return ActivityManagement.Instance.Activities; }
            set { ActivityManagement.Instance.Activities = value; }
        }

        // Holds an selected activity
        public Activity Activity { get; set; }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ActivityRegister(bool SelectingActivity = false)
        {
            InitializeComponent();
            DataContext = this;

            if (!SelectingActivity)
                btnSelect.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Opens ActivityManagement window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddActivity_Click(object sender, RoutedEventArgs e)
        {
            // Initilize a new window for adding a new account
            ActivityManager am = new ActivityManager();

            // Show the window
            am.ShowDialog();

            // If the users presses OK, add the new user
            if (am.DialogResult.Equals(true))
            {
                // Add the user to the database
                ActivityManagement.Instance.AddActicity(am.Activity);
            }
        }

        /// <summary>
        /// Opens ActivityManagement window and sends selected activity from the listview as parameter so it may be edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvActivityRegister.SelectedItem == null)
            {
                MessageBox.Show("Markera en produkt att ändra", "Ingen vald produkt");
                return;
            }

            // Initilize a new window for editing an activity
            ActivityManager am = new ActivityManager(Activity);

            // Show the window
            am.ShowDialog();

            // If the users presses OK, update the item
            if (am.DialogResult.Equals(true))
            {
                // Update the database context
                ActivityManagement.Instance.Update();
            }
            else
            {
                // The user pressed cancel, revert changes
                ActivityManagement.Instance.ResetActivity(Activity);
            }   
        }

        /// <summary>
        /// Select an activity to be deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvActivityRegister.SelectedItem != null)
            {
                // Confirm that the user wishes to delete
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här aktiviteten?",
                    "Ta bort aktivitet", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    if (ActivityManagement.Instance.IsConnectedToDirectActivityCost(Activity))
                    {
                        MessageBox.Show("Aktiviteten är kopplad till en direktkostnad, går ej ta bort", "Finns kopplade aktiviteter");
                        return;
                    }

                    if (ActivityManagement.Instance.IsConnectedToEmployee(Activity))
                    {
                        MessageBox.Show("Aktiviteten är kopplad till en anställd, går ej ta bort", "Finns kopplade aktiviteter");
                        return;
                    }

                    ActivityManagement.Instance.DeleteActivity(Activity);
                }
            }
            else
                MessageBox.Show("Markera en aktivitet att ta bort", "Ingen vald aktivitet");
        }

        /// <summary>
        /// Assigns the property 'Activity' a selected activity to be used in other windows, e.g. DCPAD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvActivityRegister.SelectedItem != null)
            {
                Activity = lvActivityRegister.SelectedItem as Activity;
                DialogResult = true;
                Close();
            }
            else
                MessageBox.Show("Markera en aktivitet i listan", "Ingen vald aktivitet");
        }

        //Sorting function
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsInitialized) return;    // get out of here if the window is not initialized

            string propertyName = (sender as GridViewColumnHeader).Tag.ToString();

            // Get the default view from the listview
            ICollectionView view = CollectionViewSource.GetDefaultView(lvActivityRegister.ItemsSource);

            // figure out what is the new direction
            ListSortDirection direction = ListSortDirection.Ascending;

            // if already sorted by this column, reverse the direction
            if (view.SortDescriptions.Count > 0 && view.SortDescriptions[0].PropertyName == propertyName)
            {
                direction = view.SortDescriptions[0].Direction == ListSortDirection.Ascending
                    ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
        }

        private void tbSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvActivityRegister.ItemsSource);

            view.Filter = null;
            view.Filter = FilterCustomerItem;
        }

        public bool FilterCustomerItem(object obj)
        {
            Activity item = obj as Activity;
            if (item == null) return false;

            string textFilter = tbSearch.Text;

            if (textFilter.Trim().Length == 0) return true;

            // apply the filter
            return item.ActivityID.ToLower().Contains(textFilter.ToLower())
                || item.ActivityName.ToString(CultureInfo.InvariantCulture).ToLower().Contains(textFilter.ToLower());
        }
    }
}
