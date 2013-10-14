using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BUPSystem.Account;
using BUPSystem.ActivityGUI;
using BUPSystem.CustomerGUI;
using BUPSystem.EmployeeGUI;
using BUPSystem.ProductGUI;
using BUPSystem.User;

namespace BUPSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Login login = new Login();

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            login.ShowDialog();

            if (login.Authenticated)
            {
                this.Show();
            }
        }

        private void btnKundhantering_Click(object sender, RoutedEventArgs e)
        {
            CustomerRegister customerRegister = new CustomerRegister();
            customerRegister.ShowDialog();
        }

        private void btnProdukthantering_Click(object sender, RoutedEventArgs e)
        {
            ProductRegister productRegister = new ProductRegister();
            productRegister.ShowDialog();
        }

        private void btnKontohantering_Click(object sender, RoutedEventArgs e)
        {
            AccountRegister accountRegister = new AccountRegister();
            accountRegister.ShowDialog();
        }

        private void btnPersonalhantering_Click(object sender, RoutedEventArgs e)
        {
            EmployeeRegister employeeRegister = new EmployeeRegister();
            employeeRegister.ShowDialog();
        }

        private void btnAnvändarhantering_Click(object sender, RoutedEventArgs e)
        {
            UserRegister userRegister = new UserRegister();
            userRegister.ShowDialog();
        }

        private void btnUppföljning_Click(object sender, RoutedEventArgs e)
        {
            Uppföljning.FollowUpAndForecasting uppföljningOchPrognos =
                new Uppföljning.FollowUpAndForecasting();
            uppföljningOchPrognos.ShowDialog();
        }

        private void btnÅrsarbetarePerProdukt_Click(object sender, RoutedEventArgs e)
        {
            Kostnadsbudgetering.AnnualEmployeeViaProduct årsArbetarePerProduct =
                new Kostnadsbudgetering.AnnualEmployeeViaProduct();
            årsArbetarePerProduct.ShowDialog();
        }

        private void btnÅrsarbetarePerAktivitet_Click(object sender, RoutedEventArgs e)
        {
            Kostnadsbudgetering.AnnualEmployeePerActivity årsarbetarePerAktivitet =
                new Kostnadsbudgetering.AnnualEmployeePerActivity();
            årsarbetarePerAktivitet.ShowDialog();
        }

        private void btnAktivitetshantering_Click(object sender, RoutedEventArgs e)
        {
            ActivityRegister activityRegister = new ActivityRegister();
            activityRegister.ShowDialog();
        }

        private void btnDKPAA_Click(object sender, RoutedEventArgs e)
        {
            Kostnadsbudgetering.DirectCostsPerActivityDepartment dkpaa =
                new Kostnadsbudgetering.DirectCostsPerActivityDepartment();
            dkpaa.ShowDialog();
        }

        private void btnDKPPA_Click(object sender, RoutedEventArgs e)
        {
            Kostnadsbudgetering.DirectCostsPerProductDepartment dkppa =
                new Kostnadsbudgetering.DirectCostsPerProductDepartment();
            dkppa.ShowDialog();
        }

        private void btnIVK_Click(object sender, RoutedEventArgs e)
        {
            Revenue_budgeting.RevenueBudgetingViaCustomer ivk = new Revenue_budgeting.RevenueBudgetingViaCustomer();
            ivk.ShowDialog();
        }

        private void btnIVP_Click(object sender, RoutedEventArgs e)
        {
            Revenue_budgeting.RevenueBudgetingViaProduct ivp = new Revenue_budgeting.RevenueBudgetingViaProduct();
            ivp.ShowDialog();
        }

        private void btnProduktgrupp_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupRegister pgr = new ProductGroupRegister();
            pgr.ShowDialog();
        }

        private void btnBudgeteratResultat_Click(object sender, RoutedEventArgs e)
        {
            Uppföljning.BudgetedResult budgetedResult = new Uppföljning.BudgetedResult();
            budgetedResult.ShowDialog();
        }

        private void btnProduktKategori_Click(object sender, RoutedEventArgs e)
        {
            ProductCategoryRegister pkr = new ProductCategoryRegister();
            pkr.ShowDialog();
        }

        private void btnUppföljningAvIntäkter_Click(object sender, RoutedEventArgs e)
        {
            Uppföljning.RevenueFollowUp uai = new Uppföljning.RevenueFollowUp();
            uai.ShowDialog();
        }
    }
}
