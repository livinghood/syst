using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer.FollowUp;
using Logic_Layer;
using Microsoft.Win32;

namespace BUPSystem.Uppföljning
{
    /// <summary>
    /// Interaction logic for RevenueFollowUp.xaml
    /// </summary>
    public partial class RevenueFollowUp : Window
    {
        private CostProductOption cpo;

        public GeneralFollowUp GFU { get; set; }

        public ObservableCollection<GeneralFollowUp> GeneralFollowUps
        {
            get { return RevenueFollowUpManagement.Instance.GeneralFollowUps; }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public RevenueFollowUp()
        {
            InitializeComponent();
            DataContext = this;
            if (System.Threading.Thread.CurrentPrincipal.IsInRole("99"))
            {
                btnImportFile.Visibility = Visibility.Collapsed;

            }
            else
            {
                UserAccount userAccount = UserManagement.Instance.GetUserAccountByUsername(System.Threading.Thread.CurrentPrincipal.Identity.Name);

                if (userAccount == null)
                    Application.Current.Shutdown();


                switch (userAccount.PermissionLevel)
                {
                    // Administrativchef
                    case 0:
                        btnImportFile.Visibility = Visibility.Collapsed;
                        break;

                    // Ekonomichef
                    case 1:

                        break;

                    // Försäljningschef
                    case 2:
                        btnImportFile.Visibility = Visibility.Collapsed;
                        break;

                    // Personalchef
                    case 3:
                        btnImportFile.Visibility = Visibility.Collapsed;
                        break;

                    // Driftschef
                    case 4:
                        btnImportFile.Visibility = Visibility.Collapsed;
                        break;

                    // Systemadministratör
                    case 5:
                        // Kan göra allt
                        break;

                    // Säljare
                    case 6:
                        btnImportFile.Visibility = Visibility.Collapsed;
                        break;

                    // Utvecklingschef
                    case 7:
                        btnImportFile.Visibility = Visibility.Collapsed;
                        break;
                }
            }

        }

        /// <summary>
        /// Import a textfile with data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportFile_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Filter = "Textfiler (.txt)|*.txt",
                    Title = "Importera KostnadProdukt.txt",
                    Multiselect = false
                };

                var result = ofd.ShowDialog();

                if (result == true)
                {
                    RevenueFollowUpManagement.Instance.CreateCostProductFromFile(ofd.FileName);
                } 
            //}

            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        /// <summary>
        /// Show all departments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbDepartment_Checked(object sender, RoutedEventArgs e)
        {
            RevenueFollowUpManagement.Instance.FillGeneralFollowUpsWithDepartments();
            cpo = CostProductOption.Department;
        }

        /// <summary>
        /// Show all product groups
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbProductGroup_Checked(object sender, RoutedEventArgs e)
        {
            RevenueFollowUpManagement.Instance.FillGeneralFollowUpsWithProductGroups();
            cpo = CostProductOption.Productgroup;
        }

        /// <summary>
        /// Show all products
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbProduct_Checked(object sender, RoutedEventArgs e)
        {
            RevenueFollowUpManagement.Instance.FillGeneralFollowUpsWithProducts();
            cpo = CostProductOption.Product;
        }

        /// <summary>
        /// Show the company
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbCompany_Checked(object sender, RoutedEventArgs e)
        {
            RevenueFollowUpManagement.Instance.FillGeneralFollowUpsWithCompany();
            cpo = CostProductOption.Company;
        }

        /// <summary>
        /// Update labels with data from a selected GeneralFollowUp in the list
        /// of GeneralFollowUps
        /// </summary>
        /// <param name="gfu"></param>
        private void UpdateLabels(GeneralFollowUp gfu)
        {
            lblCosts.Content = gfu.Costs;
            lblProduct.Content = gfu.ObjectName;
            lblRevenues.Content = gfu.Revenues;
            lblResult.Content = gfu.Result;

            switch (gfu.Date.Month)
            {
                case 1:
                    lblMonth.Content = "Januari";
                    break;
                case 2:
                    lblMonth.Content = "Februari";
                    break;
                case 3:
                    lblMonth.Content = "Mars";
                    break;
                case 4:
                    lblMonth.Content = "April";
                    break;
                case 5:
                    lblMonth.Content = "Maj";
                    break;
                case 6:
                    lblMonth.Content = "Juni";
                    break;
                case 7:
                    lblMonth.Content = "Juli";
                    break;
                case 8:
                    lblMonth.Content = "Augusti";
                    break;
                case 9:
                    lblMonth.Content = "September";
                    break;
                case 10:
                    lblMonth.Content = "Oktober";
                    break;
                case 11:
                    lblMonth.Content = "November";
                    break;
                case 12:
                    lblMonth.Content = "December";
                    break;
            }
        }
        /// <summary>
        /// Updates the labels when selection has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgItems.SelectedItem != null)
            {
                GeneralFollowUp gfu = RevenueFollowUpManagement.Instance.GetResults(cpo, GFU);
                UpdateLabels(gfu);
            }
        }
    }
}
