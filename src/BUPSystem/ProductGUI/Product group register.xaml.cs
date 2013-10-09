using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductGroupRegister.xaml
    /// </summary>
    public partial class ProductGroupRegister : Window
    {
        public ObservableCollection<ProductGroup> GroupsList
        {
            get
            {
                return new ObservableCollection<ProductGroup>(ProductGroupManagement.Instance.GetProductGroups());
            }
        }

        public ProductGroupRegister()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupCategory pgc = new ProductGroupCategory();
            pgc.ShowDialog();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupCategory pgc = new ProductGroupCategory(GroupsList[lvProductGroups.SelectedIndex]);
            pgc.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupManagement.Instance.DeleteProductGroup(GroupsList[lvProductGroups.SelectedIndex]);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ProductManagement.Instance.ProductGroup = GroupsList[lvProductGroups.SelectedIndex];
        }
    }
}
