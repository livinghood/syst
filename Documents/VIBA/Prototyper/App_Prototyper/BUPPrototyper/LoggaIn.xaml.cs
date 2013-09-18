using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BUPPrototyper
{
    /// <summary>
    /// Interaction logic for LoggaIn.xaml
    /// </summary>
    public partial class LoggaIn : Window
    {
        public LoggaIn()
        {
            InitializeComponent();
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (tbPassword.Equals("nisse") && tbUserName.Equals("Nisse"))
            {
                MessageBox.Show("");
            }
        }
    }
}
