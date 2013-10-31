using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Logic_Layer;

namespace BUPSystem
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        MainWindow HuvudMeny = new MainWindow();
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            AuthIPrincipal AuthPrincipal = new AuthIPrincipal(tbUserName.Text, tbPassword.Password);

            if ((!AuthPrincipal.Identity.IsAuthenticated))
            {
                // The user is still not validated. 
                MessageBox.Show("Användarnamn eller lösenord är fel");
            }
            else
            {
                // Update the current principal. 
                System.Threading.Thread.CurrentPrincipal = AuthPrincipal;
                HuvudMeny.Show();
                this.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void loggaIn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
