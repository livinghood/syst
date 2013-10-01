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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BUPSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoggaIn loggaIn = new LoggaIn();

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            loggaIn.ShowDialog();

            if (loggaIn.Authenticated)
            {
                this.Show();
            }
        }

        private void btnKundhantering_Click(object sender, RoutedEventArgs e)
        {
            Kund.Kundregister kundregister = new Kund.Kundregister();
            kundregister.ShowDialog();
        }

        private void btnProdukthantering_Click(object sender, RoutedEventArgs e)
        {
            Produkt.Produktregister produktregister = new Produkt.Produktregister();
            produktregister.ShowDialog();
        }

        private void btnKontohantering_Click(object sender, RoutedEventArgs e)
        {
            Konto.Kontoregister kontoregister = new Konto.Kontoregister();
            kontoregister.ShowDialog();
        }

        private void btnPersonalhantering_Click(object sender, RoutedEventArgs e)
        {
            Personal.Personalregister personalregister = new Personal.Personalregister();
            personalregister.ShowDialog();
        }

        private void btnAnvändarhantering_Click(object sender, RoutedEventArgs e)
        {
            Användare.Användarregister användarregister = new Användare.Användarregister();
            användarregister.ShowDialog();
        }

        private void btnUppföljning_Click(object sender, RoutedEventArgs e)
        {
            Uppföljning.UppföljningOchPrognostiseringAvIntäkter uppföljningOchPrognos =
                new Uppföljning.UppföljningOchPrognostiseringAvIntäkter();
            uppföljningOchPrognos.ShowDialog();
        }

        private void btnÅrsarbetarePerProdukt_Click(object sender, RoutedEventArgs e)
        {
            Kostnadsbudgetering.ÅrsarbetareViaProdukt årsArbetarePerProdukt =
                new Kostnadsbudgetering.ÅrsarbetareViaProdukt();
            årsArbetarePerProdukt.ShowDialog();
        }

        private void btnÅrsarbetarePerAktivitet_Click(object sender, RoutedEventArgs e)
        {
            Kostnadsbudgetering.ÅrsarbetereViaAktivitet årsarbetarePerAktivitet =
                new Kostnadsbudgetering.ÅrsarbetereViaAktivitet();
            årsarbetarePerAktivitet.ShowDialog();
        }

        private void btnAktivitetshantering_Click(object sender, RoutedEventArgs e)
        {
            Aktivitet.Aktivitetsregister aktivitetsregister = new Aktivitet.Aktivitetsregister();
            aktivitetsregister.ShowDialog();
        }

        private void btn_DKPAA_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
