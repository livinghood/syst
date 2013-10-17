using System.Collections.Generic;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.Kostnadsbudgetering
{
    /// <summary>
    /// Interaction logic for DirectCostsPerProductDepartment.xaml
    /// </summary>
    public partial class DirectCostsPerProductDepartment : Window
    {


        public DirectCostsPerProductDepartment()
        {
            InitializeComponent();
        }

        private void btnSelectAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductGUI.
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            BUPSystem.BUPDataSet bUPDataSet = ((BUPSystem.BUPDataSet)(this.FindResource("bUPDataSet")));
            // Load data into the table DirectProductCost. You can modify this code as needed.
            BUPSystem.BUPDataSetTableAdapters.DirectProductCostTableAdapter bUPDataSetDirectProductCostTableAdapter = new BUPSystem.BUPDataSetTableAdapters.DirectProductCostTableAdapter();
            bUPDataSetDirectProductCostTableAdapter.Fill(bUPDataSet.DirectProductCost);
            System.Windows.Data.CollectionViewSource directProductCostViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("directProductCostViewSource")));
            directProductCostViewSource.View.MoveCurrentToFirst();
            // Load data into the table Account. You can modify this code as needed.
            BUPSystem.BUPDataSetTableAdapters.AccountTableAdapter bUPDataSetAccountTableAdapter = new BUPSystem.BUPDataSetTableAdapters.AccountTableAdapter();
            bUPDataSetAccountTableAdapter.Fill(bUPDataSet.Account);
            System.Windows.Data.CollectionViewSource accountViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("accountViewSource")));
            accountViewSource.View.MoveCurrentToFirst();
        }
    }
}
