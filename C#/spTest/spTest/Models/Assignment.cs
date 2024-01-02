using System;
using System.Collections.Generic;

namespace spTest.Models
{
    public partial class Assignment
    {
        public int AssignmentId { get; set; }
        public int? EmployeeId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }

        public virtual Employee? Employee { get; set; }
        public virtual Project? Project { get; set; }
        public virtual Task? Task { get; set; }
    }
}
