using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace Logic_Layer
{
    public class EmployeeManagement
    {
        /// <summary>
        /// Lazy Instance of EmployeeManager singelton
        /// </summary>
        private static readonly Lazy<EmployeeManagement> instance = new Lazy<EmployeeManagement>(() => new EmployeeManagement());

        public ObservableCollection<Employee> EmployeeList { get; set; }

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static EmployeeManagement Instance
        {
            get { return instance.Value; }
        }

        EmployeeManagement()
        {
            EmployeeList = new ObservableCollection<Employee>(GetEmployee());
        }

        /// <summary>
        /// Get a list of all employees
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Employee> GetEmployee()
        {
            return db.Employee.OrderBy(e => e.EmployeeName);
        }

        /// <summary>
        /// Hämtar Employees via Employeeplacement med DepartmedID
        /// </summary>
        /// <param name="departmentID"></param>
        /// <returns></returns>
        public IEnumerable<Employee> GetEmployeeByDepartment(string departmentID)
        {
            IEnumerable<Employee> employees = from f in db.Employee
                                              orderby f.EmployeeID
                                              join ep in db.EmployeePlacement on departmentID equals ep.DepartmentID
                                              where f.EmployeeID == ep.EmployeeID 
                                              select f;
            return employees;
        }

        public ObservableCollection<Employee> CalculateEmployeeAtributes(ObservableCollection<Employee> employees)
        {
            foreach (Employee e in employees)
            {
                e.AnnualRate = (e.EmployeementRate - (Convert.ToInt32(e.VacancyDeduction * 100)));

                int i = e.AnnualRate;

                ObservableCollection<EmployeePlacement> ePlacements = new ObservableCollection<EmployeePlacement>(GetEmployeePlacements(e));

                foreach (EmployeePlacement ep in ePlacements)
                    i = i - Convert.ToInt32(ep.EmployeeAllocate);

                e.Diff = i.ToString();

                e.Total = e.AnnualRate - Convert.ToInt32(e.Diff);
            }

            return employees;
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="sallary"></param>
        /// <param name="employrate"></param>
        /// <param name="vacancy"></param>
        public Employee CreateEmployee(long id, string name, int sallary, int employrate, decimal vacancy)
        {
            Employee newEmployee = new Employee { EmployeeID = id, EmployeeName = name, MonthSallary = sallary, EmployeementRate = employrate, VacancyDeduction = vacancy };
            EmployeeList.Add(newEmployee);
            db.Employee.Add(newEmployee);
            db.SaveChanges();

            return newEmployee;
        }

        /// <summary>
        /// Delete a employee
        /// </summary>
        /// <param name="employee"></param>
        public void DeleteEmployee(Employee employee)
        {
            EmployeeList.Remove(employee);
            db.Employee.Remove(employee);
            db.SaveChanges();
        }

        /// <summary>
        /// Update a employee
        /// </summary>
        public void UpdateEmployee()
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Check if an employee is connected to an ActivityPlacement
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToActivityPlacement(Employee employee)
        {
            var query = db.ActivityPlacement.Where(d => d.EmployeeID.Equals(employee.EmployeeID));
            return query.Any();
        }

        /// <summary>
        /// Check if an employee is connected to a ActivityPlacement
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToProductPlacement(Employee employee)
        {
            var query = db.ProductPlacement.Where(d => d.EmployeeID.Equals(employee.EmployeeID));
            return query.Any();
        }

        /// <summary>
        /// Check if an employee is connected to an UserAccount
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToUserAccount(Employee employee)
        {
            var query = db.UserAccount.Where(d => d.EmployeeID == employee.EmployeeID);
            return query.Any();
        }

        /// <summary>
        /// Check if a specific customer exists
        /// </summary>
        public bool EmployeeExist(long id)
        {
            return db.Employee.Any(e => e.EmployeeID == id);
        }

        public void ResetEmployee(Employee EmployeeObj)
        {
            db.Entry(EmployeeObj).State = EntityState.Unchanged;
        }
        //-----------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Get a list of all employeeplacements
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EmployeePlacement> GetEmployeePlacements(Employee employee)
        {
            return db.EmployeePlacement.Where(c => c.EmployeeID == employee.EmployeeID);
        }

        /// <summary>
        /// Create a new EmployeePlacement
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="departmentId"></param>
        /// <param name="allocate"></param>
        public void CreateEmployeePlacement(long employeeId, string departmentId, decimal allocate)
        {
            EmployeePlacement newEmployeePlacement = new EmployeePlacement { EmployeeID = employeeId, DepartmentID = departmentId, EmployeeAllocate = allocate};
            db.EmployeePlacement.Add(newEmployeePlacement);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete an EmployeePlacement
        /// </summary>
        public void DeleteEmployeePlacements(Employee employee)
        {
            var tempPlacementList = employee.EmployeePlacement.ToList();

            foreach (var tempPlacement in tempPlacementList)
            {
                //Tar bort från databasen via den temporära listan
                db.EmployeePlacement.Remove(tempPlacement);
            }
            db.SaveChanges();
        }

        public void DeleteEmployeePlacement(EmployeePlacement employeePlacement)
        {
            db.EmployeePlacement.Remove(employeePlacement);
            db.SaveChanges();
        }

        /// <summary>
        /// Update a EmployeePlacement
        /// </summary>
        public void UpdateEmployeePlacement()
        {
            db.SaveChanges();
        }
    }
}
