using System;
using System.Collections.Generic;

namespace spTest.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Assignments = new HashSet<Assignment>();
        }

        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int DepartmentId { get; set; }
        public decimal? Salary { get; set; }
        public string? EmployeeType { get; set; }
        public string? EmployeeStatus { get; set; }
        public DateTime? LeaveStartDate { get; set; }
        public DateTime? LeaveEndDate { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
