using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using Logic_Layer.Cost_Budgeting_Logic;

namespace BUPSystem.Kostnadsbudgetering
{
    /// <summary>
    /// Interaction logic for DirectCostsPerActivityDepartment.xaml
    /// </summary>
    public partial class DirectCostsPerActivityDepartment : Window
    {
        private DirectActivityCost objToAdd;

        //public ObservableCollection<DirectActivityCost> itemList { get; set; }

        private Activity activity { get; set; }

        private Account account { get; set; }

        public ObservableCollection<DirectActivityCost> DirectActivityCosts
        {
            get { return DCPADManagement.Instance.DirectActivityCosts; }
            set { DCPADManagement.Instance.DirectActivityCosts = value; }
        }

        public ObservableCollection<Account> Accounts
        {
            get { return AccountManagement.Instance.Accounts; }
        }

        public string DepartmentID { get; set; }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public DirectCostsPerActivityDepartment()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Method used to make some initial preparations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void winDCPAD_Loaded(object sender, RoutedEventArgs e)
        {
            dgAccounts.ItemsSource = Accounts;

            //brnLock är alltid låst från början. Enablas när produktionschefen loggar in, 
            //utifall den är låst i databasen(100, 101) så diseables knappen för honom också.

            if (ExpenseBudgetManagement.Instance.IsDirectExpenseBudgetLocked(DepartmentID))
            {
                btnLock.IsEnabled = false;
            }
        }

        /// <summary>
        /// Change selected account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DirectActivityCosts.Clear();
            account = dgAccounts.SelectedItem as Account;
            DirectActivityCosts = new ObservableCollection<DirectActivityCost>(DCPADManagement.Instance.GetAccounts(account));
            dgDCPAD.ItemsSource = DirectActivityCosts;
            lblSum.Content = "Summa: " + DCPADManagement.Instance.CalculateSum(account);
            btnAddActivity.IsEnabled = true;
        }

        /// <summary>
        /// Allows editing of cells
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDCPAD_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement elementProductID = dgDCPAD.Columns[0].GetCellContent(e.Row);
                if (elementProductID.GetType() == typeof(TextBox))
                {
                    var column1 = ((TextBox)elementProductID).Text;
                    objToAdd.ActivityID = column1;

                }

                FrameworkElement elementProductCost = dgDCPAD.Columns[1].GetCellContent(e.Row);
                if (elementProductCost.GetType() == typeof(TextBox))
                {
                    var column2 = ((TextBox)elementProductCost).Text;
                    objToAdd.ActivityCost = Convert.ToInt32(column2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fel");
            }
        }

        /// <summary>
        /// Saves edited row changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDCPAD_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            DCPADManagement.Instance.SaveEditing(objToAdd, dgAccounts.SelectedItem as Account);
            lblSum.Content = "Summa: " + DCPADManagement.Instance.CalculateSum(account);
        }

        /// <summary>
        /// Change selected DirectActivityCost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDCPAD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDCPAD.SelectedItem != null)
            {
                objToAdd = dgDCPAD.SelectedItem as DirectActivityCost;
            }
        }

        /// <summary>
        /// Lock the expense budget
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Vill du verkligen låsa kostnadsbudgeten?", "Låsa kostnadsbudget?", MessageBoxButton.YesNo);

            if (mbr == MessageBoxResult.Yes)
            {

                UserPermissionLevels upl = UserPermissionLevels.Driftschef;

                if (upl == UserPermissionLevels.Driftschef)
                {
                    if (ExpenseBudgetManagement.Instance.LockDirectExpenseBudget(DepartmentID))
                    {
                        MessageBox.Show("Kostnadsbudgeten har låsts", "Låsning lyckades");
                    }
                    else
                        MessageBox.Show("Kunde inte låsa kostnadsbudgeten", "Låsning misslyckades");
                }
            }
        }

        /// <summary>
        /// Connect a new activity to the selected account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddActivity_Click(object sender, RoutedEventArgs e)
        {
            ActivityGUI.ActivityRegister ar = new ActivityGUI.ActivityRegister();
            ar.ShowDialog();

            if (ar.DialogResult == true)
            {
                activity = ar.Activity;

                // Check if user attempts to add a product that is already connected to the selected account
                bool activityConnected = DCPADManagement.Instance.CheckIfActivityConnected(activity.ActivityID);

                if (activityConnected)
                {
                    MessageBox.Show(String.Format
                        ("Du försöker lägga till en aktivitet som redan är kopplad till konto {0}.",
                        account.AccountName), "Aktivitet redan kopplad");
                    return;
                }

                objToAdd = new DirectActivityCost
                {
                    AccountID = account.AccountID,
                    ActivityID = activity.ActivityID
                };
                DCPADManagement.Instance.SaveNewActivity(objToAdd, dgAccounts.SelectedItem as Account);
            }
            lblSum.Content = "Summa: " + DCPADManagement.Instance.CalculateSum(account);
        }
    }
}
