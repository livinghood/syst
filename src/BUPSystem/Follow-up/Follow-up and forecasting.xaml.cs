using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using Logic_Layer.FollowUp;
using Microsoft.Win32;

namespace BUPSystem.Uppföljning
{
    /// <summary>
    /// Interaction logic for FollowUpAndForecasting.xaml
    /// </summary>
    public partial class FollowUpAndForecasting : Window
    {
        private bool saved;
        private bool isManual;
        private int cbIndex;

        public ObservableCollection<Forecasting> Forecasts { get; set; }

        public ObservableCollection<Months> Months { get { return ForecastingManagement.Instance.GetMonths(); } }

        public FollowUpAndForecasting()
        {
            InitializeComponent();
            DataContext = this;
            saved = true;
            cbIndex = 0;

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
            ForecastingManagement.Instance.FillForecastsFromDB(cbMonth.SelectedIndex);
            var list = ForecastingManagement.Instance.GetForecastFromMonth(cbMonth.SelectedIndex);

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
                if (!saved)
                {
                    var result = MessageBox.Show("Du har inte sparat ändringar gjorda i den valda månaden. " +
                                    "Fortsätta ändå?", "Ändringar ej sparade",MessageBoxButton.YesNo);
                    
                    if (result == MessageBoxResult.No)
                    {
                        cbMonth.SelectedIndex = cbIndex;
                        return;
                    }
                }
                UpdateForecasts();
                ForecastingManagement.Instance.UpdateForecast();
                // Prevent user from editing when all months option is selected 
                dgForecasts.IsEnabled = cbMonth.SelectedIndex != 0;
            }
        }

        private void dgForecasts_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!isManual)
            {
                isManual = true;
                DataGrid dg = (DataGrid)sender;
                dg.CommitEdit(DataGridEditingUnit.Row, true);
                isManual = false;

            }
            ForecastingManagement.Instance.CalculateTrend(dgForecasts.SelectedItem as Forecasting, cbMonth.SelectedIndex);
            saved = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbMonth.SelectedIndex > 0)
            {
                ForecastingManagement.Instance.AddForecast(cbMonth.SelectedIndex);
                saved = true;
            }           
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            // Prevent locking when all months option is selected 
            if (cbMonth.SelectedIndex > 0)
            {
                // First, save any unsaved changes 
                ForecastingManagement.Instance.AddForecast(cbMonth.SelectedIndex);

                // Call method to lock forecast for the selected month
                ForecastingManagement.Instance.LockForecast(cbMonth.SelectedIndex);
            }
            else
                MessageBox.Show("Du måste välja en enskild månad för låsning", "Välj en månad att låsa");
        }
    }
}
