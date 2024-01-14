-- Departments Table
CREATE TABLE Department (
    DepartmentId INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName VARCHAR(255),
    Location VARCHAR(100)
);

-- Projects Table
CREATE TABLE Project (
    ProjectId INT PRIMARY KEY IDENTITY(1,1),
    ProjectName VARCHAR(255),
    StartDate DATE
);

-- Tasks Table
CREATE TABLE Task (
    TaskId INT PRIMARY KEY IDENTITY(1,1),
    ProjectId INT NOT NULL,
    TaskName VARCHAR(255),
    Priority VARCHAR(50),
    TaskStatus VARCHAR(50),
    FOREIGN KEY (ProjectId) REFERENCES Project(ProjectId)
);

-- Employees Table
CREATE TABLE Employee (
    EmployeeId INT PRIMARY KEY IDENTITY(1,1),
    EmployeeName VARCHAR(255),
    DepartmentId INT NOT NULL,
    Salary DECIMAL(10, 2),
    EmployeeType VARCHAR(50),
    EmployeeStatus VARCHAR(50),
    LeaveStartDate DATE,
    LeaveEndDate DATE,
    FOREIGN KEY (DepartmentId) REFERENCES Department(DepartmentId)
);

-- Assignments Table
CREATE TABLE Assignment (
    AssignmentId INT PRIMARY KEY IDENTITY(1,1),
    EmployeeId INT NOT NULL,
    TaskId INT NOT NULL,
    FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId),
    FOREIGN KEY (TaskId) REFERENCES Task(TaskId)
);
