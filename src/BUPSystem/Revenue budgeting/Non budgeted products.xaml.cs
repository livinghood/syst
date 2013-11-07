using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Logic_Layer;
using Logic_Layer.General_Logic;

namespace BUPSystem.Revenue_budgeting
{
    /// <summary>
    /// Interaction logic for NonBudgetedProducts.xaml 
    /// </summary>
    /// 
    public partial class NonBudgetedProducts : Window
    {
        public ObservableCollection<Product> NonBudgetedProductsList { get; set; }

        /// <summary>z
        /// Standard constructor
        /// </summary>
        public NonBudgetedProducts()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Selects products that belongs to the drift department
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbDriftDepartment_Checked(object sender, RoutedEventArgs e)
        {
            NonBudgetedProductsList = new ObservableCollection<Product>(ProductManagement.Instance.GetNonBudgetedProducts("DA"));
            dgProducts.ItemsSource = NonBudgetedProductsList;
        }

        /// <summary>
        /// Selects products that belongs to the development department
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbDevelopmentDeparment_Checked(object sender, RoutedEventArgs e)
        {
            NonBudgetedProductsList = new ObservableCollection<Product>(ProductManagement.Instance.GetNonBudgetedProducts("UF"));
            dgProducts.ItemsSource = NonBudgetedProductsList;
        }

        /// <summary>
        /// Export selected list of products to text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            string department;
            if (rbDevelopmentDeparment.IsChecked.HasValue && rbDevelopmentDeparment.IsChecked.Value) department = "DA";
            else if (rbDriftDepartment.IsChecked.HasValue && rbDriftDepartment.IsChecked.Value) department = "UF";
            else
            {
                MessageBox.Show("Välj en avdelning att exportera till textfil.", "Ingen vald avdelning");
                return;
            }

            string message = 
                String.Format("Du har valt att exportera ej budgeterade produkter från avdelning {0}. Vill du fortsätta?", department);

            MessageBoxResult mbr = MessageBox.Show(message, "Exportera till textfil", MessageBoxButton.YesNo);

            if (mbr == MessageBoxResult.Yes)
            {
                PrintLogic pl = new PrintLogic();
                string filename = String.Format("Ej Budgeterade Produkter - {0}.txt", department);
                bool success = pl.ExportNonBudgetedProductsToTextFile(filename, NonBudgetedProductsList);

                string finalMessage = success ? "Exportering till fil är klar" : "Misslyckades att exportera till fil";

                MessageBox.Show(finalMessage, "Exportering till fil");
            }        
        }
    }
}
