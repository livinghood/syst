using System.Collections.ObjectModel;
using System.Windows;

namespace BUPSystem.Kund
{
    /// <summary>
    /// Interaction logic for Kundregister.xaml
    /// </summary>
    public partial class Kundregister : Window
    {
        public ObservableCollection<Logic_Layer.CustomerNamespace.Customer> CustomerList
        {
            get
            {
                return Logic_Layer.LogicController.Instance.CustomerList;
            }
        }

        public Kundregister()
        {
            InitializeComponent();

            // Kod som läser in användares behörighet
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Button used to create a new customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Kundhantering kundhantering = new Kundhantering();
            kundhantering.ShowDialog();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            Kundhantering kundhantering = new Kundhantering();

            // Kod som lägger in vald kunds uppgifter i kundhanteringsfönstret

            kundhantering.ShowDialog();
        }
    }
}
