using System;
using System.Collections.Generic;

namespace spTest.Models
{
    public partial class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
        }

        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? Location { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
