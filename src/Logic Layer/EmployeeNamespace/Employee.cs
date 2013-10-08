using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.EmployeeNamespace
{
    /// <summary>
    ///  An employee belongs to a category, which is either "AffoAvdelning" or "Produktionsavdelning"
    /// </summary>
    public enum Placements
    {
        AffoAvdelning,
        Produktionsavdelning
    }
    /// <summary>
    /// Class that defines an employee
    /// </summary>
    public class Employee
    {
        public string EmployeeName { get; set; }
        public long EmployeeID { get; set; }
        public int MonthlySalary { get; set; }
        public int EmploymentRate { get; set; }
        public double AnnualEmployee { get; set; }
        public double VacancyDeduction { get; set; }
        public double Diff { get; set; }               // räknas ut?
        public Placements Placement { get; set; }
        public double AdministrativeDepartment { get; set; }
        public double DriftDeparment { get; set; }
        public double SalesMarket { get; set; }
        public double DevelopmentManagement { get; set; }
        public double Drift { get; set; }

        /// <summary>
        /// Constructor to create an employee
        /// </summary>
        /// <param name="employeeName"></param>
        /// <param name="employeeId"></param>
        /// <param name="monthlySalary"></param>
        /// <param name="employmentRate"></param>
        /// <param name="annualEmployee"></param>
        /// <param name="vacancyDeduction"></param>
        /// <param name="diff"></param>
        /// <param name="placement"></param>
        /// <param name="administrativeDepartment"></param>
        /// <param name="driftDepartment"></param>
        /// <param name="salesMarket"></param>
        /// <param name="developmentManagement"></param>
        /// <param name="drift"></param>
        public Employee(string employeeName, long employeeId, int monthlySalary, int employmentRate,
            double annualEmployee, double vacancyDeduction, double diff, Placements placement, double administrativeDepartment,
            double driftDepartment, double salesMarket, double developmentManagement, double drift)
        {
            EmployeeName = employeeName;
            EmployeeID = employeeId;
            MonthlySalary = monthlySalary;
            EmploymentRate = employmentRate;
            AnnualEmployee = annualEmployee;
            VacancyDeduction = vacancyDeduction;
            Diff = diff;
            Placement = placement;
            AdministrativeDepartment = administrativeDepartment;
            DriftDeparment = driftDepartment;
            SalesMarket = salesMarket;
            DevelopmentManagement = developmentManagement;
            Drift = drift;
        }
    }
}
