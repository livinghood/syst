using System.Windows;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductManager.xaml
    /// </summary>
    public partial class ProductManager : Window
    {
        public ProductManager()
        {
            InitializeComponent();
        }

        private void btnProductGroup_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupRegister pgr = new ProductGroupRegister();
            pgr.ShowDialog();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ProductManagement.Instance.CreateProduct(tbProductID.Text, tbProductName.Text, depa)
        }
    }
}
