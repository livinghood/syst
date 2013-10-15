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
            get { return ProductGroupManagement.Instance.ProductGroups; }
            set { ProductGroupManagement.Instance.ProductGroups = value; }
        }
        /// <summary>
        /// Standard constructor
        /// </summary>
        public ProductGroupRegister()
        {
            InitializeComponent();
            DataContext = this;
        }
        /// <summary>
        /// Add a new product group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupCategory pgc = new ProductGroupCategory();
            pgc.ShowDialog();
            if (pgc.DialogResult == true)
            {
                ProductGroupManagement.Instance.AddProductGroup(pgc.ProductGroup);
            }
        }
        /// <summary>
        /// Edit a product group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (lvProductGroups.SelectedItem != null)
            {
                ProductGroupCategory pgc = new ProductGroupCategory(GroupsList[lvProductGroups.SelectedIndex]);
                pgc.ShowDialog();
                if (pgc.DialogResult == true)
                {
                    ProductGroupManagement.Instance.UpdateProductGroup();
                    lvProductGroups.Items.Refresh();
                }
            }
            else
                MessageBox.Show("Markera en produktgrupp att redigera först", "Ingen vald produktgrupp");
        }
        /// <summary>
        /// Delete a product group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvProductGroups.SelectedItem != null)
            {
                // Confirm that the user wishes to delete
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här produktgruppen?",
                    "Ta bort", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    // Check that the product group is not associated to any products before removing
                    bool empty = ProductGroupManagement.Instance.IsProductGroupEmpty(GroupsList[lvProductGroups.SelectedIndex]);

                    if (empty)
                    {
                        ProductGroupManagement.Instance.DeleteProductGroup(GroupsList[lvProductGroups.SelectedIndex]);
                        //lblInfo.Content = "Användaren togs bort";
                    }
                    else
                        MessageBox.Show("Det finns produkter som är kopplade till den här produktgruppen." +
                                        "Ta bort dessa produkter först", "Kan inte ta bort");
                }
            }
            else
                MessageBox.Show("Markera en produktgrupp att ta bort", "Ingen vald produktgrupp");
        }
        /// <summary>
        /// Select a product group for a product from productmanager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (lvProductGroups.SelectedItem != null)
            {
                ProductGroup = GroupsList[lvProductGroups.SelectedIndex];
                DialogResult = true;
                Close();
            }
            else
                MessageBox.Show("Markera en grupp i listan först.", "Välj en grupp");
        }
    }
}
