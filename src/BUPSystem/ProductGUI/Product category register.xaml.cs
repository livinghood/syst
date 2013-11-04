using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System;
using Logic_Layer;
using Logic_Layer.General_Logic;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductCategoryRegister.xaml
    /// </summary>
    public partial class ProductCategoryRegister : Window
    {
        public ObservableCollection<ProductCategory> CategoriesList
        {
            get { return ProductCategoryManagement.Instance.ProductCategories; }
            set { ProductCategoryManagement.Instance.ProductCategories = value; }
        }

        // Containing the selected product
        public ProductCategory SelectedProductCategory { get; set; }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ProductCategoryRegister(bool SelectingAccount = false)
        {
            InitializeComponent();
            DataContext = this;

            if (!SelectingAccount)
                btnSelect.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Add a new product category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Initilize a new window for adding a new account
            ProductGroupCategory pgc = new ProductGroupCategory(false);

            // Show the window
            pgc.ShowDialog();

            // If the users presses OK, add the new user
            if (pgc.DialogResult.Equals(true))
            {
                // Add the category to the database
                ProductCategoryManagement.Instance.AddProductCategory(pgc.ProductCategory);
            }

        }

        /// <summary>
        /// Change a product category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvProductCategories.SelectedItem == null)
            {
                MessageBox.Show("Markera en produktkategori att ändra", "Ingen produktkategori vald");
                return;
            }

            // Initilize a new window for editing a category
            ProductGroupCategory pgc = new ProductGroupCategory(SelectedProductCategory);

            // Show the window
            pgc.ShowDialog();

            // If the users presses OK, update the item
            if (pgc.DialogResult == true)
            {
                // Update the database context
                ProductCategoryManagement.Instance.UpdateProductCategory();
            }
            else
            {
                // The user pressed cancel, revert changes
                ProductCategoryManagement.Instance.ResetProductCategory(SelectedProductCategory);
            }
        }

        /// <summary>
        /// Delete a product category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            
            // Make sure the sure the user has selected an item in the listview
            if (lvProductCategories.SelectedItem != null)
            {
                // Confirm that the user wishes to delete 
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här produktkategorin?", "Ta bort produktkategori", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    if (ProductCategoryManagement.Instance.IsProductCategoryEmpty(SelectedProductCategory))
                    {
                        ProductCategoryManagement.Instance.DeleteProductCategory(SelectedProductCategory);
                    }
                    else
                        MessageBox.Show("Det finns produktgrupper som är kopplade till den här produktkategorin." +
                                        "Ta bort dessa produktgrupper först", "Kan inte ta bort");
                }
            }
            else
                MessageBox.Show("Markera en produktkategori att ta bort", "Ingen produktkategori vald"); 
        }

        /// <summary>
        /// Selects a product category when creating a product group which the product group
        /// will be associated to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvProductCategories.SelectedItem == null)
            {
                MessageBox.Show("Markera en produktkateogri att välja", "Ingen vald produktkateogri");
                return;
            }

            DialogResult = true;
            Close();
        }
        // Sorting function
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsInitialized) return;    // get out of here if the window is not initialized

            string propertyName = (sender as GridViewColumnHeader).Tag.ToString();

            // Get the default view from the listview
            ICollectionView view = CollectionViewSource.GetDefaultView(lvProductCategories.ItemsSource);

            // figure out what is the new direction
            ListSortDirection direction = ListSortDirection.Ascending;

            // if already sorted by this column, reverse the direction
            if (view.SortDescriptions.Count > 0 && view.SortDescriptions[0].PropertyName == propertyName)
            {
                direction = view.SortDescriptions[0].Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
        }

        public bool FilterCustomerItem(object obj)
        {
            ProductCategory item = obj as ProductCategory;
            if (item == null) return false;

            string textFilter = tbSearch.Text;

            if (textFilter.Trim().Length == 0) return true; // the filter is empty - pass all items

            // apply the filter
            return item.ProductCategoryName.ToLower().Contains(textFilter.ToLower()) || item.ProductCategoryID.ToLower().Contains(textFilter.ToLower());
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvProductCategories.ItemsSource);

            view.Filter = null;
            view.Filter = FilterCustomerItem; 
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvProductCategories.ItemsSource);

            view.Filter = null;
        }
    }
}
