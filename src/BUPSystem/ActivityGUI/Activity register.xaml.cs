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
            get
            {
                return new ObservableCollection<Activity>(ActivityManagement.Instance.GetActivities());
            }
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
        }

        /// <summary>
        /// Opens ActivityManagement window and sends selected activity from the listview as parameter so it may be edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            ActivityManager am = new ActivityManager(ActivityList[lvActivityRegister.SelectedIndex]);
            am.ShowDialog();
        }
    }
}
