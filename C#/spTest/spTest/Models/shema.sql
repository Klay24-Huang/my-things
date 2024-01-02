
-- Departments Table
CREATE TABLE department (
    department_id INT PRIMARY KEY IDENTITY(1,1),
    department_name VARCHAR(255),
    location VARCHAR(100)
);
-- Projects Table
CREATE TABLE project (
    project_id INT PRIMARY KEY IDENTITY(1,1),
    project_name VARCHAR(255),
    start_date DATE
);

-- Tasks Table
CREATE TABLE task (
    task_id INT PRIMARY KEY IDENTITY(1,1),
    task_name VARCHAR(255),
    priority VARCHAR(50),
    task_status VARCHAR(50)
);

-- Employees Table
CREATE TABLE employee (
    employee_id INT PRIMARY KEY IDENTITY(1,1),
    employee_name VARCHAR(255),
    department_id INT,
    salary DECIMAL(10, 2),
    employee_type VARCHAR(50),
    employee_status VARCHAR(50),
    leave_start_date DATE,
    leave_end_date DATE,
    FOREIGN KEY (department_id) REFERENCES department(department_id)
);


-- Assignments Table
CREATE TABLE assignments (
    assignment_id INT PRIMARY KEY IDENTITY(1,1),
    employee_id INT,
    project_id INT,
    task_id INT,
    FOREIGN KEY (employee_id) REFERENCES employee(employee_id),
    FOREIGN KEY (project_id) REFERENCES project(project_id),
    FOREIGN KEY (task_id) REFERENCES task(task_id)
);
