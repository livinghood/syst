using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using Logic_Layer.FollowUp;
using Microsoft.Win32;
using System;

namespace BUPSystem.Uppföljning
{
    /// <summary>
    /// Interaction logic for FollowUpAndForecasting.xaml
    /// </summary>
    public partial class FollowUpAndForecasting : Window
    {
        private bool saved;
        private bool isManual;

        public ForecastMonth ForecastMonth;

        public ObservableCollection<Forecasting> Forecasts { get { return ForecastingManagement.Instance.Forecasts; } }

        public ObservableCollection<Months> Months { get { return ForecastingManagement.Instance.GetMonths(); } }

        public FollowUpAndForecasting()
        {
            InitializeComponent();
            DataContext = this;
            saved = true;
        }

        private void btnImportFile_Click(object sender, RoutedEventArgs e)
        {
            var mbresult = MessageBox.Show("Vid import av ny fil kommer befintlig uppföljningsdata att skrivas över. Fortsätta?",
                    "Import av fil", MessageBoxButton.YesNo);

            if (mbresult == MessageBoxResult.Yes)
            {
                try
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
                        Forecasts.Clear();
                        cbMonth_SelectionChanged(sender, e as SelectionChangedEventArgs);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void UpdateForecasts(bool getAll = false)
        {
            getAll = (bool)cbshowAll.IsChecked;

            Forecasts.Clear();

            Months SelectedMonth;
            Enum.TryParse(cbMonth.SelectedValue.ToString(), out SelectedMonth);

            DateTime month = new DateTime(DateTime.Now.Year, (int)SelectedMonth, 1);

            // Adda en month för att kunna låsa (addas bara om den inte finns)
            ForecastMonth = ForecastingManagement.Instance.AddForecastMonth(month);

            ForecastingManagement.Instance.GetForecastsFromMonth(month,getAll);

            UpdateLabels();

            if (Forecasts.Any())
            {
                if (ForecastingManagement.Instance.CheckIfLocked(month))
                {
                    dgForecasts.IsEnabled = false;
                    btnSave.IsEnabled = false;
                    lblInfo.Content = "Uppföljning för denna månad är låst";
                }
                else
                {
                    dgForecasts.IsEnabled = true;
                    btnSave.IsEnabled = true;
                    lblInfo.Content = "";
                }
            }

            
        }

        private void cbMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            if (cbMonth.SelectedValue != null)
            {
                if (cbMonth.SelectedIndex == 0)
                    return;

                UpdateForecasts();
                
            }
        }

        private void UpdateLabels()
        {
            int? budget = 0;
            int? outcomeMonth = 0;
            int? outcomeAcc = 0;
            int? reprocssed = 0;
            int? trend = 0;
            int? formerPrognosis = 0;
            int? prognosis = 0;
            int? prognosisBudget = 0;

            foreach (var item in Forecasts)
            {
                budget += item.Budget;
                outcomeMonth += item.OutcomeMonth;
                outcomeAcc += item.OutcomeAcc;
                reprocssed += item.Reprocessed;
                trend += item.Trend;
                formerPrognosis += item.FormerPrognosis;
                prognosis += item.Forecast;
                prognosisBudget += item.ForecastBudget;
            }

            lblBudget.Content = budget.ToString();
            lblFormerPrognosis.Content = formerPrognosis.ToString();
            lblOutcomeAcc.Content = outcomeAcc.ToString();
            lblOutcomeMonth.Content = outcomeMonth.ToString();
            lblPrognosis.Content = prognosis.ToString();
            lblReprocessed.Content = reprocssed.ToString();
            lblTrend.Content = trend.ToString();
            lblPrognosisBudget.Content = prognosisBudget.ToString();
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
            UpdateLabels();
            saved = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbMonth.SelectedIndex > 0)
            {
                Months SelectedMonth;
                Enum.TryParse(cbMonth.SelectedValue.ToString(), out SelectedMonth);

                DateTime month = new DateTime(DateTime.Now.Year, (int)SelectedMonth, 1);


                foreach (Forecasting fc in Forecasts)
                {
                    ForecastMonitor fm = (ForecastingManagement.Instance.ForecastMonitorExist(fc.IeProductID, month));
                    if (fm == null)
                    {
                        if (fc.Reprocessed > 0 || fc.Forecast > 0)
                        {
                            ForecastMonitor newfm = new ForecastMonitor
                            {
                                Reprocessed = fc.Reprocessed,
                                OutcomeAcc = fc.OutcomeAcc,
                                IeProductName = fc.IeProductName,
                                IeProductID = fc.IeProductID,
                                ForecastMonth = ForecastMonth,
                                ForecastMonitorMonthID = month.ToString("yyyyMM"),
                                ForecastBudget = fc.Budget.ToString(),
                                Forecast = fc.Forecast
                            };

                            ForecastingManagement.Instance.AddForecastMonitor(newfm);
                        }
                    }
                    else
                    {
                        fm.Reprocessed = fc.Reprocessed;
                        fm.OutcomeAcc = fc.OutcomeAcc;
                        fm.ForecastBudget = fc.Budget.ToString();
                        fm.Forecast = fc.Forecast;
                    }
                    ForecastingManagement.Instance.UpdateForecast();
                }
                           
                //ForecastingManagement.Instance.AddForecast(cbMonth.SelectedIndex);
                saved = true;
            }
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            // Prevent locking when all months option is selected 
            if (cbMonth.SelectedIndex > 0)
            {
                Months SelectedMonth;
                Enum.TryParse(cbMonth.SelectedValue.ToString(), out SelectedMonth);

                DateTime month = new DateTime(DateTime.Now.Year, (int)SelectedMonth, 1);

                //Call method to lock forecast for the selected month
                ForecastingManagement.Instance.LockForecast(month);

                MessageBox.Show(string.Format("Uppföljning för månad {0} har låsts", month), "Låst");
            }
            else
                MessageBox.Show("Du måste välja en enskild månad för låsning", "Välj en månad att låsa");
        }

        private void cbMonth_DropDownOpened(object sender, System.EventArgs e)
        {
            if (!saved)
            {
                var result = MessageBox.Show("Du har inte sparat ändringar gjorda i den valda månaden. " +
                                "Fortsätta ändå?", "Ändringar ej sparade", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.No)
                {
                    cbMonth.IsDropDownOpen = false;
                }
                else
                {
                    saved = true;
                    cbMonth.IsDropDownOpen = true;
                }
            }
        }

        private void cbshowAll_Checked(object sender, RoutedEventArgs e)
        {
            if (cbMonth.SelectedValue != null)
            {
                if (cbMonth.SelectedIndex == 0)
                    return;

                UpdateForecasts();
            }
        }
    }
}
