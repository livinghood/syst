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
    /// Interaction logic for ProductRegister.xaml
    /// </summary>
    public partial class ProductRegister : Window
    {
        public ObservableCollection<Product> Products { get { return ProductManagement.Instance.Products; } }
        /// <summary>
        /// Standard constructor
        /// </summary>
        public ProductRegister(bool SelectingProduct = false, string department = "All")
        {
            InitializeComponent();
            DataContext = this;

            if (!SelectingProduct)
                btnSelect.Visibility = Visibility.Collapsed;

            if (System.Threading.Thread.CurrentPrincipal.IsInRole("99"))
            {
                btn_Change.Visibility = Visibility.Collapsed;
                btnAdd.Visibility = Visibility.Collapsed;
                btnRemove.Visibility = Visibility.Collapsed;

            }
            else
            {
                UserAccount userAccount = UserManagement.Instance.GetUserAccountByUsername(System.Threading.Thread.CurrentPrincipal.Identity.Name);

                if (userAccount == null)
                    Application.Current.Shutdown();


                switch (userAccount.PermissionLevel)
                {
                    // Ekonomichef
                    case 1:
                        btn_Change.Visibility = Visibility.Collapsed;
                        btnAdd.Visibility = Visibility.Collapsed;
                        btnRemove.Visibility = Visibility.Collapsed;
                        break;

                    // Driftschef
                    case 4:
                        btn_Change.Visibility = Visibility.Collapsed;
                        btnRemove.Visibility = Visibility.Collapsed;
                        break;

                    // Systemadministratör
                    case 5:
                        // Kan göra allt
                        break;
                    // Utvecklingschef
                    case 7:
                        btn_Change.Visibility = Visibility.Collapsed;
                        btnRemove.Visibility = Visibility.Collapsed;
                        break;
                }
            }

            ProductManagement.Instance.FillProductList(department);
        }

        // Containing the selected customer
        public Product SelectedProduct { get; set; }

        /// <summary>
        /// Adds a product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Initilize a new window for adding a new customer
            ProductManager pm = new ProductManager();

            // Show the window
            pm.ShowDialog();

            // If the users presses OK, add the new customer
            if (pm.DialogResult.Equals(true))
            {
                ProductManagement.Instance.AddProduct(pm.Product);
            }
        }
        /// <summary>
        /// Edit a product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvProducts.SelectedItem == null)
            {
                MessageBox.Show("Markera en produkt att ändra", "Ingen vald produkt");
                return;
            }

            // Initilize a new window for editing a product
            ProductManager pm = new ProductManager(SelectedProduct);

            // Show the window
            pm.ShowDialog();

            // If the users presses OK, update the item
            if (pm.DialogResult.Equals(true))
            {
                // Update the database context
                ProductManagement.Instance.Update();
            }
            else
            {
                // The user pressed cancel, revert changes
                ProductManagement.Instance.ResetProduct(SelectedProduct);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvProducts.SelectedItem == null)
            {
                MessageBox.Show("Markera en produkt att välja", "Ingen vald produkt");
                return;
            }

            DialogResult = true;
            Close();
        }
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvProducts.SelectedItem != null)
            {
                // Confirm that the user wishes to delete 
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här produkten?", "Ta bort produkt", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    if (ProductManagement.Instance.IsConnectedToDirectProductCost(SelectedProduct))
                    {
                        MessageBox.Show("Produkten är kopplad till en direktkostnad, går ej ta bort", "Finns kopplade produkter");
                        return; 
                    }

                    if (ProductManagement.Instance.IsConnectedToFinancialIncome(SelectedProduct))
                    {
                        MessageBox.Show("Produkten är kopplad till en intäktsbudgetering, går ej ta bort", "Finns kopplade produkter");
                        return;
                    }

                    if (ProductManagement.Instance.IsConnectedToEmployee(SelectedProduct))
                    {
                        MessageBox.Show("Produkten är kopplad till en anställd, går ej ta bort", "Finns kopplade produkter");
                        return;
                    }

                    // Delete the customer from the database
                    ProductManagement.Instance.DeleteProduct(SelectedProduct);
                }
            }
            else
                MessageBox.Show("Markera en produkt att ta bort", "Ingen vald produkt");       

        }

        public bool FilterCustomerItem(object obj)
        {
            Product item = obj as Product;
            if (item == null) return false;

            string textFilter = tbSearch.Text;

            if (textFilter.Trim().Length == 0) return true; // the filter is empty - pass all items

            // apply the filter
            return item.ProductID.ToLower().Contains(textFilter.ToLower()) || item.ProductName.ToLower().Contains(textFilter.ToLower());
        }

        // Sort listview
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsInitialized) return;    // get out of here if the window is not initialized

            string propertyName = (sender as GridViewColumnHeader).Tag.ToString();

            // Get the default view from the listview
            ICollectionView view = CollectionViewSource.GetDefaultView(lvProducts.ItemsSource);

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

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvProducts.ItemsSource);

            view.Filter = null;
            view.Filter = FilterCustomerItem; 
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvProducts.ItemsSource);

            view.Filter = null;
        }
    }
}
