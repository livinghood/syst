using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer.FollowUp;
using Microsoft.Win32;

namespace BUPSystem.Uppföljning
{
    /// <summary>
    /// Interaction logic for FollowUpAndForecasting.xaml
    /// </summary>
    public partial class FollowUpAndForecasting : Window
    {
        public ObservableCollection<Forecasting> Forecasts { get; set; }

        public ObservableCollection<Months> Months { get { return ForecastingManagement.Instance.GetMonths(); } }

        public FollowUpAndForecasting()
        {
            InitializeComponent();
            DataContext = this;

            /* If the file IntaktProduktKund.txt has already been imported earlier during the current session,
             * set the local list of forecasts to the one in ForecastingMangement, else create a new local list. */
            Forecasts = ForecastingManagement.Instance.Forecasts.Count > 0
                ? new ObservableCollection<Forecasting>(ForecastingManagement.Instance.Forecasts)
                : new ObservableCollection<Forecasting>();
        }

        private void btnImportFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Textfiler (.txt)|*.txt", 
                Title = "Importera IntäktProduktKund.txt",
                Multiselect = false
            };

            var result = ofd.ShowDialog();

            if (result == true)
            {
                ForecastingManagement.Instance.CreateForecastFromFile(ofd.FileName);
                cbMonth_SelectionChanged(sender, e as SelectionChangedEventArgs);
            }           
        }

        private void UpdateForecasts()
        {
            Forecasts.Clear();
            var list = ForecastingManagement.Instance.GetForecastFromMonth(Months[cbMonth.SelectedIndex]);

            if (list != null)
            {
                foreach (var forecast in list)
                {
                    Forecasts.Add(forecast);
                }
            }
        }

        private void cbMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbMonth.SelectedItem != null)
            {
                UpdateForecasts();

                // Prevent user from editing when all months option is selected 
                dgForecasts.IsEnabled = cbMonth.SelectedIndex != 0;
            }
        }

        private void dgForecasts_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ForecastingManagement.Instance.CalculateTrend(dgForecasts.SelectedItem as Forecasting, cbMonth.SelectedIndex);
        }

        private void dgForecasts_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        { 
            UpdateForecasts();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ForecastingManagement.Instance.UpdateForecast();
        }
    }
}
