using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer.FollowUp;

namespace BUPSystem.Uppföljning
{
    /// <summary>
    /// Interaction logic for BudgetedResult.xaml
    /// </summary>
    public partial class BudgetedResult : Window
    {
        public GeneralFollowUp GFU { get; set; }

        public ObservableCollection<GeneralFollowUp> GeneralFollowUps
        {
            get { return BudgetedResultManagement.Instance.GeneralFollowUps; }
        }

        private CostProductOption cpo;

        public BudgetedResult()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void rbDepartment_Checked(object sender, RoutedEventArgs e)
        {
            BudgetedResultManagement.Instance.FillGeneralFollowUpsWithDepartments();
            cpo = CostProductOption.Department;
        }

        private void rbProductGroup_Checked(object sender, RoutedEventArgs e)
        {
            BudgetedResultManagement.Instance.FillGeneralFollowUpsWithProductGroups();
            cpo = CostProductOption.Productgroup;
        }

        private void rbProduct_Checked(object sender, RoutedEventArgs e)
        {
            BudgetedResultManagement.Instance.FillGeneralFollowUpsWithProducts();
            cpo = CostProductOption.Product;
        }

        private void rbCompany_Checked(object sender, RoutedEventArgs e)
        {
            BudgetedResultManagement.Instance.FillGeneralFollowUpsWithCompany();
            cpo = CostProductOption.Company;
        }

        private void UpdateLabels(GeneralFollowUp gfu)
        {
            lblCostsSum.Content = gfu.Costs;
            lblRevenuesSum.Content = gfu.Revenues;
            lblResultSum.Content = gfu.Result;
            lblObject.Content = gfu.ObjectName;
        }

        private void dgItems_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgItems.SelectedItem != null)
            {
                GeneralFollowUp gfu = BudgetedResultManagement.Instance.GetResults(cpo, GFU.ObjectID);
                UpdateLabels(gfu);
            }
        }

        
    }
}
