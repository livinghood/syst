using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductGroupCategory.xaml
    /// </summary>
    public partial class ProductGroupCategory : Window
    {
        public ProductCategory ProductCategory { get; set; }
        public ProductGroup ProductGroup { get; set; }

        /// <summary>
        /// Constructor called when creating a new product category or 
        /// product group
        /// </summary>
        public ProductGroupCategory(bool isGroup)
        {
            InitializeComponent();

            // Ledsen för detta, riktigt skitigt
            if (isGroup)
            {
                lblTitle.Content = "Grupphantering";
                this.Title = "Grupphantering";
                ProductGroup ProductGroup = new ProductGroup();
                this.ProductGroup = ProductGroup;
                this.DataContext = ProductGroup;
                gbCategory.Visibility = Visibility.Collapsed;
                //Clear product category validation
                Binding CategoryIDBinding = BindingOperations.GetBinding(tbCategoryID, TextBox.TextProperty);
                Binding CategoryNameBinding = BindingOperations.GetBinding(tbCateogryName, TextBox.TextProperty);
                CategoryIDBinding.ValidationRules.Clear();
                CategoryNameBinding.ValidationRules.Clear();
            }
            else
            {
                this.Title = "Kategorihantering";
                lblTitle.Content = "Kategorihantering";
                ProductCategory ProductCategory = new ProductCategory();
                this.ProductCategory = ProductCategory;
                this.DataContext = ProductCategory;
                gbGroup.Visibility = Visibility.Collapsed;
                //Clear product group validation
                Binding GroupIDBinding = BindingOperations.GetBinding(tbGroupID, TextBox.TextProperty);
                Binding GroupNameBinding = BindingOperations.GetBinding(tbGroupName, TextBox.TextProperty);
                GroupIDBinding.ValidationRules.Clear();
                GroupNameBinding.ValidationRules.Clear();
            }

        }

        /// <summary>
        /// Constructor called when editing a product category
        /// </summary>
        /// <param name="category"></param>
        public ProductGroupCategory(ProductCategory category)
        {
            InitializeComponent();
            ProductCategory = category;
            this.DataContext = ProductCategory;
            gbGroup.Visibility = Visibility.Collapsed;

            //Clear product group validation
            Binding GroupIDBinding = BindingOperations.GetBinding(tbGroupID, TextBox.TextProperty);
            Binding GroupNameBinding = BindingOperations.GetBinding(tbGroupName, TextBox.TextProperty);
            GroupIDBinding.ValidationRules.Clear();
            GroupNameBinding.ValidationRules.Clear();

            // Disable CategoryID textbox and clear validation
            tbCategoryID.IsEnabled = false;
            Binding CategoryIDBinding = BindingOperations.GetBinding(tbCategoryID, TextBox.TextProperty);
            CategoryIDBinding.ValidationRules.Clear();
           

        }

        /// <summary>
        /// Constructor called when editing a product group
        /// </summary>
        /// <param name="group"></param>
        public ProductGroupCategory(ProductGroup group)
        {
            InitializeComponent();
            ProductGroup = group;
            this.DataContext = ProductGroup;
            gbCategory.Visibility = Visibility.Collapsed;
            
            //Clear product category validation
            Binding CategoryIDBinding = BindingOperations.GetBinding(tbCategoryID, TextBox.TextProperty);
            Binding CategoryNameBinding = BindingOperations.GetBinding(tbCateogryName, TextBox.TextProperty);
            CategoryIDBinding.ValidationRules.Clear();
            CategoryNameBinding.ValidationRules.Clear();
            
            // Disable GroupID textbox and clear validation
            tbGroupID.IsEnabled = false;
            Binding GroupIDBinding = BindingOperations.GetBinding(tbGroupID, TextBox.TextProperty);
            GroupIDBinding.ValidationRules.Clear();
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(tbGroupID))
                return;
            if (Validation.GetHasError(tbCategoryID))
                return;
            DialogResult = true;
            Close();
        }
    }
}
