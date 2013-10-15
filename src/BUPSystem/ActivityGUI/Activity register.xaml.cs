using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

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

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ActivityRegister()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Opens ActivityManagement window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddActivity_Click(object sender, RoutedEventArgs e)
        {
            ActivityManager am = new ActivityManager();
            am.ShowDialog();
            if (am.DialogResult == true)
            {
                ActivityManagement.Instance.AddActicity(am.Activity);
                //lblInfo.Content = "Ny användare skapad";
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
            if (lvActivityRegister.SelectedItem != null)
            {
                ActivityManager am = new ActivityManager(ActivityList[lvActivityRegister.SelectedIndex]);
                am.ShowDialog();

                if (am.DialogResult == true)
                {
                    ActivityManagement.Instance.Update();
                    //lblInfo.Content = "Ny användare skapad";
                }
            }
            else
                MessageBox.Show("Markera en aktivitet i listan", "Ingen vald aktivitet");     
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
                    ActivityManagement.Instance.DeleteActivity(ActivityList[lvActivityRegister.SelectedIndex]);
                    //lblInfo.Content = "Användaren togs bort";
                }
            }
            else
                MessageBox.Show("Markera en aktivitet att ta bort", "Ingen vald aktivitet");
        }
    }
}
