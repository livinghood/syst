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

namespace BUPSystem.Revenue_budgeting
{
    /// <summary>
    /// Interaction logic for NonBudgetedProducts.xaml 
    /// Knappar för skriv ut etc kanske inte behövs.
    /// </summary>
    /// 
    public partial class NonBudgetedProducts : Window
    {
        public ObservableCollection<Product> Products
        {
            get { return ProductManagement.Instance.Products; }
            set { ProductManagement.Instance.Products = value; }
        }

        public ObservableCollection<Product> NonBudgetedProductsList { get; set; }

        public NonBudgetedProducts()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void winNonBudgetedProducts_Loaded(object sender, RoutedEventArgs e)
        {
            NonBudgetedProductsList = new ObservableCollection<Product>(ProductManagement.Instance.GetNonBudgetedProducts());
            dgProducts.ItemsSource = NonBudgetedProductsList;
        }
    }
}
