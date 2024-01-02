using System;
using System.Collections.Generic;

namespace spTest.Models
{
    public partial class Task
    {
        public Task()
        {
            Assignments = new HashSet<Assignment>();
        }

        public int TaskId { get; set; }
        public string? TaskName { get; set; }
        public string? Priority { get; set; }
        public string? TaskStatus { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
