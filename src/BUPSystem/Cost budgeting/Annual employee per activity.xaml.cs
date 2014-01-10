using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;
using BUPSystem.ActivityGUI;
using System.Windows.Data;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace BUPSystem.Kostnadsbudgetering
{
    /// <summary>
    /// Interaction logic for AnnualEmployeePerActivity.xaml
    /// </summary>
    public partial class AnnualEmployeePerActivity : Window
    {
        private ObservableCollection<Employee> m_EmployeeList = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> EmployeeList { get { return m_EmployeeList; } }

        public ObservableCollection<Activity> SelectedActivities { get; set; }

        public ObservableCollection<ActivityPlacement> ActivityPlacementList { get; set; }

        public ObservableCollection<DataItemActivity> MyList { get; set; }

        public ObservableCollection<Department> Departments { get { return EmployeeManagement.Instance.AFFODepartments;} }

        private string DepartmentID;

        public AnnualEmployeePerActivity()
        {
            InitializeComponent();
            DataContext = this;
            Logic_Layer.Cost_Budgeting_Logic.ExpenseBudgetManagement.Instance.DoesExpenseBudgetExist();

            MyList = new ObservableCollection<DataItemActivity>();
            ActivityPlacementList = new ObservableCollection<ActivityPlacement>();
            SelectedActivities = new ObservableCollection<Activity>();
            //EmployeeList = new ObservableCollection<Employee>();

            Logic_Layer.Cost_Budgeting_Logic.ExpenseBudgetManagement.Instance.DoesExpenseBudgetExist();

            UserAccount userAccount = UserManagement.Instance.GetUserAccountByUsername(System.Threading.Thread.CurrentPrincipal.Identity.Name);

            switch (userAccount.PermissionLevel)
            {
                //Administrativ Chef
                case 0:
                    DepartmentID = "AO";
                    cbDepartments.Visibility = Visibility.Collapsed;
                    lblChooseDepartment.Visibility = Visibility.Collapsed;
                    LoadEmployees();
                    break;
                //Försäljning Chef
                case 2:
                    DepartmentID = "FO";
                    cbDepartments.Visibility = Visibility.Collapsed;
                    lblChooseDepartment.Visibility = Visibility.Collapsed;
                    LoadEmployees();
                    break;
                //System Admin
                case 5:
                    DepartmentID = "AO";
                    break;
                //Ekonomichef
                case 1:
                    DepartmentID = "AO";
                    btnLock.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;
                    dgActivityPlacements.IsReadOnly = true;
                    btnChooseActivity.IsEnabled = false;
                    break;
            }
            LockedSettings();
        }

        private void LoadEmployees()
        {
            dgActivityPlacements.Columns.Clear();
            EmployeeList.Clear();
            MyList.Clear();
            ActivityPlacementList.Clear();
            SelectedActivities.Clear();

            foreach (Employee e in EmployeeManagement.Instance.GetEmployeeAtributes(DepartmentID))
            {
                m_EmployeeList.Add(e);
            }

            CreateRow();
            LoadExistingPlacements();
        }

        /// <summary>
        /// Fyller listan med redan existerande placeringar
        /// </summary>
        private void LoadExistingPlacements()
        {
            foreach (Employee e in EmployeeList)
            {
                foreach (ActivityPlacement ap in ActivityManagement.Instance.GetActivityPlacementsByEmployeeAndDepartment(e, DepartmentID))
                {
                    foreach (DataItemActivity DIA in MyList)
                    {
                        if (DIA.EmployeeID.Equals(e.EmployeeID))
                        {
                            DIA.DataList.Add(ap);

                            //Create column if it doesn't exist
                            if (!ColumnExists(ap.Activity.ActivityName))
                            {
                                DataGridTextColumn activityColumn = new DataGridTextColumn { Header = ap.Activity.ActivityName };
                                activityColumn.Binding = new Binding("DataList[" + dgActivityPlacements.Columns.Count + "].ActivityAllocate");
                                dgActivityPlacements.Columns.Add(activityColumn);
                                SelectedActivities.Add(ap.Activity);
                            }

                        }
                    }
                }
            }

            foreach (DataItemActivity DIA in MyList)
            {
                foreach (DataGridTextColumn DGTC in dgActivityPlacements.Columns)
                {
                    ActivityPlacement tempAP = DIA.DataList.SingleOrDefault(a => a.Activity.ActivityName.Equals(DGTC.Header.ToString()));

                    if (tempAP == null)
                    {
                        Activity tempAct = ActivityManagement.Instance.GetActivityByName(DGTC.Header.ToString());
                        ActivityPlacement newPlacement = new ActivityPlacement
                        {
                            EmployeeID = DIA.EmployeeID,
                            Activity = tempAct,
                            ActivityID = tempAct.ActivityID,
                            ExpenseBudgetID = DateTime.Now.Year,
                            ActivityAllocate = 0
                        };
                        DIA.DataList.Add(newPlacement);
                        ActivityPlacementList.Add(newPlacement);
                    }
                }
            }


        }

        private bool ColumnExists(string activityName)
        {
            foreach (DataGridColumn DGC in dgActivityPlacements.Columns)
            {
                if (DGC.Header.Equals(activityName))
                    return true;
            }
            return false;
        }

        private void CreateColumn(Activity a)
        {
            DataGridTextColumn activityColumn = new DataGridTextColumn();
            activityColumn.Header = a.ActivityName;
            foreach (DataItemActivity di in MyList)
            {
                ActivityPlacement ap = new ActivityPlacement() { EmployeeID = di.EmployeeID, ActivityID = a.ActivityID, ActivityAllocate = 0 };
                di.DataList.Add(ap);
                ActivityPlacementList.Add(ap);
            }
            activityColumn.Binding = new Binding("DataList[" + dgActivityPlacements.Columns.Count + "].ActivityAllocate");
            dgActivityPlacements.Columns.Add(activityColumn);
        }

        private void CreateRow()
        {
            foreach (Employee e in EmployeeList)
            {
                var m = new DataItemActivity() { EmployeeID = e.EmployeeID };
                MyList.Add(m);
            }
        }

        private void cbDepartments_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            DepartmentID = Departments[cbDepartments.SelectedIndex].DepartmentID;
            LoadEmployees();
            LockedSettings();
        }

        private void btnChooseAktivity_Click(object sender, RoutedEventArgs e)
        {
            ActivityRegister activityRegister = new ActivityRegister(true, DepartmentID);

            if (activityRegister.ShowDialog() == true)
            {
                if (SelectedActivities.Contains(activityRegister.Activity))
                {
                    MessageBox.Show("Du kan inte välja samma produkt flera gånger");
                }
                else
                {
                    SelectedActivities.Add(activityRegister.Activity);
                    CreateColumn(activityRegister.Activity);
                }
            }
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Vill du verkligen låsa denna budgeten?", "Låsa årsarbetare", MessageBoxButton.YesNo);
            if (mbr == MessageBoxResult.Yes)
            {
                if (Logic_Layer.Cost_Budgeting_Logic.ExpenseBudgetManagement.Instance.LockAnnualExpenseBudget(DepartmentID))
                {
                    LockedSettings();
                    MessageBox.Show("Årsarbetare per aktivitet är nu låst");
                }
                else
                    MessageBox.Show("Låsningen misslyckades");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (ActivityPlacement ap in ActivityPlacementList)
            {
                ActivityManagement.Instance.AddActivityPlacement(ap);
            }
            MessageBox.Show("Data är sparad");
        }

        private void LockedSettings()
        {
            if (Logic_Layer.Cost_Budgeting_Logic.ExpenseBudgetManagement.Instance.IsAnnualExpenseBudgetLocked(DepartmentID))
            {
                btnLock.IsEnabled = false;
                btnSave.IsEnabled = false;
                dgActivityPlacements.IsReadOnly = true;
                btnChooseActivity.IsEnabled = false;
            }
            else
            {
                btnLock.IsEnabled = true;
                btnSave.IsEnabled = true;
                dgActivityPlacements.IsReadOnly = false;
                btnChooseActivity.IsEnabled = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // För varje productplacement i listan: reset changes
            foreach (DataItemActivity di in MyList)
            {
                foreach (ActivityPlacement a in di.DataList)
                {
                    // Reset changes
                    ActivityManagement.Instance.ResetActivityPlacement(a);
                }
            }
        }

    }

    public class DataItemActivity
    {   //KLASS FÖR ATT LÄGGA TILL EGNA RADER
        public long EmployeeID { get; set; }
        public ObservableCollection<ActivityPlacement> DataList { get; set; }
        public DataItemActivity()
        {
            this.DataList = new ObservableCollection<ActivityPlacement>();
        }
    }

}
