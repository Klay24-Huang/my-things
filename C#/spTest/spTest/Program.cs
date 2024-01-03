using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using spTest.Models;
using Task = spTest.Models.Task;
using System.Diagnostics;

class Program
{
    // Scaffold-DbContext "Server=H-HICEIT6-20\NEWSERVER;Database=spTest;MultipleActiveResultSets=true;User ID=sa;Password=1qaz@WSX" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force
    // reference: https://ithelp.ithome.com.tw/articles/10262110
    static void Main()
    {
        Console.WriteLine("App started.");
        using var context = new spTestContext();
        context.Database.EnsureCreated(); // 确保数据库已创建
        //SeedData(context, 3000);
        RunTest(context, 100);
    }

    static void RunTest(spTestContext context, int count)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < count; i++)
        {
            QueryEmployees(context);
        }
        stopwatch.Stop();
        Console.WriteLine($"ef 查詢時間：{stopwatch.Elapsed.TotalMilliseconds} 毫秒");

        stopwatch.Restart();
        for (int i = 0; i < count; i++)
        {
            QueryEmployeesWithDapper();
        }
        stopwatch.Stop();
        Console.WriteLine($"sp 查詢時間：{stopwatch.Elapsed.TotalMilliseconds} 毫秒");
    }

    static void QueryEmployeesWithDapper()
    {
        using var dbConnection = new SqlConnection("Server=H-HICEIT6-20\\NEWSERVER;Database=spTest;MultipleActiveResultSets=true;User ID=sa;Password=1qaz@WSX");
        dbConnection.Open();
        var result = new Dictionary<int, Employee>(); // 用于跟踪已映射的 Employee

        dbConnection.Query<Employee, Department, Assignment, Task, Employee>(
            "SP_GetEmployees",
            (employee, department, assignment, task) =>
            {
                // 检查是否已经存在该 Employee
                if (!result.TryGetValue(employee.EmployeeId, out var existingEmployee))
                {
                    // 如果不存在，则将该 Employee 添加到字典中
                    existingEmployee = employee;
                    existingEmployee.Department = department;
                    existingEmployee.Assignments = new List<Assignment>();
                    result.Add(existingEmployee.EmployeeId, existingEmployee);
                }

                // 处理关联关系
                assignment.Task = task;
                existingEmployee.Assignments.Add(assignment);

                return existingEmployee; // 返回已处理过关联关系的 Employee
            },
            splitOn: "DepartmentId, AssignmentId, TaskId",
            commandType: CommandType.StoredProcedure
        );

        var finalResult = result.Values.ToList();
    }

    static void QueryEmployees(spTestContext context)
    {
        var departmentsFilter = new List<string> {
            "IT",
            //"HR",
            "Marketing",
            "Finance",
            //"Operations"
        };
        var employeesWithComplexConditions = context.Employees
            .Where(e => departmentsFilter.Contains(e.Department.DepartmentName) &&
                e.EmployeeType == "Full-Time" &&
                e.Salary >= 60000m && e.Salary <= 120000m &&
                e.Assignments.Any(a => a.Task.Priority == "High"))
            .Include(e => e.Department)
            .Include(e => e.Assignments.Where(a => a.Task.Priority == "High"))
            .ThenInclude(a => a.Task).ToList();

        //var sql = context.Employees
        //    .Where(e => departmentsFilter.Contains(e.Department.DepartmentName) &&
        //        e.EmployeeType == "Full-Time" &&
        //        e.Salary >= 60000m && e.Salary <= 120000m &&
        //        e.Assignments.Any(a => a.Task.Priority == "High"))
        //    .Include(e => e.Department)
        //    .Include(e => e.Assignments.Where(a => a.Task.Priority == "High"))
        //    .ThenInclude(a => a.Task).ToQueryString();

    }

    static void SeedData(spTestContext context, int employeeCounts)
    {
        try
        {
            using var transaction = context.Database.BeginTransaction();
            var projects = GenerateProjects(context);
            var tasks = GenerateTasks(context, projects);
            var departments = GenerateDepartments(context);
            var employees = GenerateEmployees(context, departments, employeeCounts);
            GenerateAssignments(context, tasks, employees);
            transaction.Commit();
            Console.WriteLine("Seed data completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] SeedData: {ex}");
            throw;
        }
    }

    static List<Department> GenerateDepartments(spTestContext context)
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

        context.Departments.AddRange(departments);
        context.SaveChanges();
        return departments;
    }

    static List<Project> GenerateProjects(spTestContext context)
    {
        var projects = new List<Project>();
        for (int i = 0; i < 10; i++)
        {
            projects.Add(new Project
            {
                ProjectName = $"project Name {i}",
                StartDate = DateTime.Now.AddDays(i),
            });
        }
        context.Projects.AddRange(projects);
        context.SaveChanges();
        return projects;
    }

    static List<Task> GenerateTasks(spTestContext context, List<Project> projects)
    {
        var tasks = new List<Task>();
        for (int i = 1; i <= 100; i++)
        {
            tasks.Add(new Task
            {
                TaskName = $"Task {i}",
                ProjectId = projects[GetRandomIndex(projects.Count)].ProjectId,
                Priority = i % 2 == 0 ? "High" : "Low",
                TaskStatus = i % 3 == 0 ? "Completed" : "In Progress"
            });
        }
        context.Tasks.AddRange(tasks);
        context.SaveChanges();
        return tasks;
    }

    static List<Employee> GenerateEmployees(spTestContext context, List<Department> departments, int count)
    {
        var employees = new List<Employee>();
        var salarys = new List<decimal> {
            50000.0m,
            60000.0m,
            80000.0m,
            120000.0m,
            150000.0m,
        };
        for (int i = 0; i < count; i++)
        {
            var employee = new Employee
            {
                EmployeeName = $"Employee {i}",
                DepartmentId = departments[GetRandomIndex(departments.Count)].DepartmentId,
                Salary = salarys[GetRandomIndex(salarys.Count)],
                EmployeeType = i % 2 == 0 ? "Full-Time" : "Part-Time",
                EmployeeStatus = i % 3 == 0 ? "Inactive" : "Active",
                LeaveStartDate = DateTime.Now.AddDays(i),
                LeaveEndDate = DateTime.Now.AddDays(i + 5),
            };

            employees.Add(employee);
        }
        context.Employees.AddRange(employees);
        context.SaveChanges();
        return employees;
    }

    static List<Assignment> GenerateAssignments(spTestContext context, List<Task> tasks, List<Employee> employees)
    {
        var assignments = new List<Assignment>();
        for (int i = 0; i < employees.Count; i++)
        {
            // 從100個tasks中 隨機取1~5個
            var randomNumbers = GenerateRandomNumbers(tasks.Count, GetRandomIndex(5));
            foreach (var index in randomNumbers)
            {
                var assignment = new Assignment
                {
                    EmployeeId = employees[GetRandomIndex(employees.Count)].EmployeeId,
                    TaskId = tasks[index].TaskId,
                };
                assignments.Add(assignment);
            }
        }
        context.Assignments.AddRange(assignments);
        context.SaveChanges();
        return assignments;
    }

    static int GetRandomIndex(int count)
    {
        var random = new Random();
        return random.Next(count);
    }

    static List<int> GenerateRandomNumbers(int N, int count)
    {
        List<int> randomNumbers = new List<int>();
        Random random = new Random();

        while (randomNumbers.Count < count)
        {
            int randomNumber = random.Next(N);

            // 確保數字不重複
            if (!randomNumbers.Contains(randomNumber))
            {
                randomNumbers.Add(randomNumber);
            }
        }

        return randomNumbers;
    }
}
