using System;
using System.Collections.Generic;

namespace spTest.Models
{
    public partial class Project
    {
        public Project()
        {
            Tasks = new HashSet<Task>();
        }

        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public DateTime? StartDate { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }
}
