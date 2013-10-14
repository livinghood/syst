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
        // Property to hold a product group when it's to be selected from product manager
        public ProductGroup ProductGroup { get; set; }

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
            ProductGroup = GroupsList[lvProductGroups.SelectedIndex];
            
            DialogResult = true;
            Close();
        }
    }
}
