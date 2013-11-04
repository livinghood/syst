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
    /// Interaction logic for ProductGroupRegister.xaml
    /// </summary>
    public partial class ProductGroupRegister : Window
    {
        // Property to hold a product group when it's to be selected from product manager
        public ProductGroup SelectedProductGroup { get; set; }

        public ObservableCollection<ProductGroup> GroupsList
        {
            get { return ProductGroupManagement.Instance.ProductGroups; }
            set { ProductGroupManagement.Instance.ProductGroups = value; }
        }
        /// <summary>
        /// Standard constructor
        /// </summary>
        public ProductGroupRegister(bool SelectingProductGroup = false)
        {
            InitializeComponent();
            DataContext = this;

            if (!SelectingProductGroup)
                btnSelect.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Add a new product group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Initilize a new window for adding a new account
            ProductGroupCategory pgc = new ProductGroupCategory(true);

            // Show the window
            pgc.ShowDialog();

            // If the users presses OK, add the new user
            if (pgc.DialogResult.Equals(true))
            {
                // Add the category to the database
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
            // Make sure the sure the user has selected an item in the listview
            if (lvProductGroups.SelectedItem == null)
            {
                MessageBox.Show("Markera en produktgrupp att ändra", "Ingen produktgrupp vald");
                return;
            }

            // Initilize a new window for editing a group
            ProductGroupCategory pgc = new ProductGroupCategory(SelectedProductGroup);

            // Show the window
            pgc.ShowDialog();

            // If the users presses OK, update the item
            if (pgc.DialogResult == true)
            {
                // Update the database context
                ProductGroupManagement.Instance.UpdateProductGroup();
            }
            else
            {
                // The user pressed cancel, revert changes
                ProductGroupManagement.Instance.ResetProductGroup(SelectedProductGroup);
            }
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
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här produktgruppen?", "Ta bort produktgrupp", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    if (ProductGroupManagement.Instance.IsProductGroupEmpty(SelectedProductGroup))
                    {
                        ProductGroupManagement.Instance.DeleteProductGroup(SelectedProductGroup);
                    }
                    else
                    {
                        MessageBox.Show("Det finns kopplade produkter i den här gruppen, går ej ta bort", "Finns kopplade produkter"); 
                    }                   
                }
            }
            else
                MessageBox.Show("Markera en produktgrupp att ta bort", "Ingen produktgrupp vald"); 
        }
        /// <summary>
        /// Select a product group for a product from productmanager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (lvProductGroups.SelectedItem == null)
            {
                MessageBox.Show("Markera en produktgrupp att välja", "Ingen vald produktgrupp");
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
            ICollectionView view = CollectionViewSource.GetDefaultView(lvProductGroups.ItemsSource);

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
            ProductGroup item = obj as ProductGroup;
            if (item == null) return false;

            string textFilter = tbSearch.Text;

            if (textFilter.Trim().Length == 0) return true; // the filter is empty - pass all items

            // apply the filter
            return item.ProductGroupName.ToLower().Contains(textFilter.ToLower()) || item.ProductGroupID.ToLower().Contains(textFilter.ToLower());
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvProductGroups.ItemsSource);

            view.Filter = null;
            view.Filter = FilterCustomerItem;
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvProductGroups.ItemsSource);

            view.Filter = null;
        }
    }
}
