using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;
using Logic_Layer.ActivityNamespace;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductManager.xaml
    /// </summary>
    public partial class ProductManager : Window
    {

        private readonly IEnumerable<Department> departmentsList = ProductManagement.Instance.GetDepartments();
        private bool changeExistingActivity;

        ObservableCollection<Department> list = new ObservableCollection<Department>();

        public ProductManager()
        {
            InitializeComponent();

            // Items from IEnumerable must be placed in a List in order for the combobox index to work
            foreach (var item in departmentsList)
            {
                list.Add(item);
            }
        }

        private void btnProductGroup_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupRegister pgr = new ProductGroupRegister();
            pgr.ShowDialog();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ProductManagement.Instance.CreateProduct(tbProductID.Text, tbProductName.Text,
                list[cbDepartment.SelectedIndex]);
        }
    }
}
