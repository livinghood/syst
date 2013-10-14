using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductRegister.xaml
    /// </summary>
    public partial class ProductRegister : Window
    {
        public ObservableCollection<Product> Products { get { return ProductManagement.Instance.Products; } }

        public ProductRegister()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductManager pm = new ProductManager();
            pm.ShowDialog();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            ProductManager pm = new ProductManager(Products[lvProducts.SelectedIndex]);
            pm.ShowDialog();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
