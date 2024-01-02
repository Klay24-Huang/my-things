using spTest.Models;
using System.Linq;
using Task = spTest.Models.Task;

class Program
{
    static void Main()
    {
        using (var context = new spTestContext())
        {
            context.Database.EnsureCreated(); // 确保数据库已创建

            var departments = GenerateDepartment(context);
            var tasts = GenerateTasks(context);

            for (int i = 1; i <= 100; i++)
            {

                var project = new Project
                {
                    ProjectName = $"Project {i % 5 + 1}",
                    StartDate = DateTime.Now.AddDays(i)
                };

                var taskCount = i % 3 + 1; // 生成 1 到 3 个 Task
                var tasks = GenerateTasks(taskCount);

                var employee = new Employee
                {
                    EmployeeName = $"Employee {i}",
                    Department = department,
                    Salary = 50000.0m + i,
                    EmployeeType = i % 2 == 0 ? "Full-Time" : "Part-Time",
                    EmployeeStatus = i % 3 == 0 ? "Inactive" : "Active",
                    LeaveStartDate = DateTime.Now.AddDays(i),
                    LeaveEndDate = DateTime.Now.AddDays(i + 5),
                    Assignments = GenerateAssignments(tasks)
                };

                context.Employees.Add(employee);
            }

            context.SaveChanges();
        }
    }

    static List<Department> GenerateDepartment(spTestContext context)
    {
        var departmentNames = new List<string> {
            "IT",
            "HR",
            "Marketing",
            "Finance",
            "Operations"
        };

        var departments = departmentNames.Select(x => new Department
        {
            DepartmentName = x,
            Location = $"Location of {x}"
        }).ToList();

        context.AddRange(departments);
        return departments;
    }

    static List<Project> GenerateProject(spTestContext context)
    {
        var projects = new List<Project>();
        for (int i = 0; i < 5; i++)
        {
            projects.Add(new Project
            {
                ProjectName = $"project Name {i}",
                StartDate = DateTime.Now.AddDays(i),
            });
        }
        context.AddRange(projects);
        return projects;
    }

    static List<Task> GenerateTasks(spTestContext context)
    {
        var tasks = new List<Task>();
        for (int i = 1; i <= 10; i++)
        {
            tasks.Add(new Task
            {
                TaskName = $"Task {i}",
                Priority = i % 2 == 0 ? "High" : "Low",
                TaskStatus = i % 3 == 0 ? "Completed" : "In Progress"
            });
        }
        context.AddRange(tasks);
        return tasks;
    }

    static List<Assignment> GenerateAssignments(List<Task> tasks)
    {
        var assignments = new List<Assignment>();
        foreach (var task in tasks)
        {
            assignments.Add(new Assignment
            {
                ProjectId = task.TaskId % 5 + 1,
                Task = task
            });
        }
        return assignments;
    }
}
