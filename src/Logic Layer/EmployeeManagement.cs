using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic_Layer
{
    public class EmployeeManagement
    {
        /// <summary>
        /// Lazy Instance of EmployeeManager singelton
        /// </summary>
        private static readonly Lazy<EmployeeManagement> instance = new Lazy<EmployeeManagement>(() => new EmployeeManagement());

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


        /// <summary>
        /// Get a list of all employees
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Employee> GetEmployee()
        {
            IEnumerable<Employee> employees = from c in db.Employee
                                              orderby c.EmployeeName
                                              select c;

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
        public void CreateEmployee(long id, string name, int sallary, int employrate, decimal vacancy)
        {
            Employee newEmployee = new Employee { EmployeeID = id, EmployeeName = name, MonthSallary = sallary, EmployeementRate = employrate, VacancyDeduction = vacancy };
            db.Employee.Add(newEmployee);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete a employee
        /// </summary>
        /// <param name="employee"></param>
        public void DeleteEmployee(Employee employee)
        {
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


        //-----------------------------------------------------------------------------------------------------------


        /// <summary>
        /// Get a list of all employeeplacements
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EmployeePlacement> GetEmployeePlacement()
        {
            IEnumerable<EmployeePlacement> placements = from c in db.EmployeePlacement
                                                        orderby c.EmployeeID
                                                        select c;

            return placements;
        }

        /// <summary>
        /// Create a new EmployeePlacement
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="departmentId"></param>
        /// <param name="allocate"></param>
        public void CreateEmployeePlacement(long employeeId, string departmentId, decimal allocate)
        {
            EmployeePlacement newEmployeePlacement = new EmployeePlacement { EmployeeID = employeeId, DepartmentID = departmentId, EmployeeAllocate = allocate };
            db.EmployeePlacement.Add(newEmployeePlacement);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete a EmployeePlacement
        /// </summary>
        /// <param name="employeePlacement"></param>
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
