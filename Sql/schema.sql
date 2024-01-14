-- 部門表（departments）
CREATE TABLE departments (
    dept_id INT PRIMARY KEY,
    dept_name VARCHAR(255),
    -- 其他部門相關的列
);

-- 職稱表（titles）
CREATE TABLE titles (
    title_id INT PRIMARY KEY,
    title VARCHAR(255),
    -- 其他職稱相關的列
);

-- 員工表（employees）
CREATE TABLE employees (
    emp_id INT PRIMARY KEY,
    emp_name VARCHAR(255),
    hire_date DATE,
    dept_id INT,
	title_id INT,
    FOREIGN KEY (dept_id) REFERENCES departments(dept_id),
    FOREIGN KEY (title_id) REFERENCES titles(title_id)
    -- 其他員工相關的列
);


-- 經理表（managers）
CREATE TABLE managers (
    manager_id INT PRIMARY KEY,
    dept_id INT,
	emp_id INT,
    FOREIGN KEY (dept_id) REFERENCES departments(dept_id),
	FOREIGN KEY (emp_id) REFERENCES employees(emp_id)
    -- 其他經理相關的列
);

-- 薪水表（salaries）
CREATE TABLE salaries (
    salary_id INT PRIMARY KEY,
    emp_id INT,
    salary INT,
    FOREIGN KEY (emp_id) REFERENCES employees(emp_id)
    -- 其他薪水相關的列
);


-- 插入部門資料
INSERT INTO departments (dept_id, dept_name)
VALUES 
    (1, 'Engineering');

-- 插入職稱資料
INSERT INTO titles (title_id, title)
VALUES 
    (1, 'Manager'),
    (2, 'Engineer');

	-- 插入員工資料
INSERT INTO employees (emp_id, emp_name, hire_date, dept_id, title_id)
VALUES 
    (1, 'John Doe', '2022-01-01', 1, 1), -- 符合條件的經理
    (2, 'Jane Smith', '2022-02-01', 1, 2),
    (3, 'Bob Johnson', '2022-03-01', 1, 1), -- 符合條件的經理
    (4, 'Alice Williams', '2022-04-01', 1, 2),
    (5, 'Charlie Brown', '2022-05-01', 1, 2),
    (6, 'David Lee', '2022-06-01', 1, 1), -- 符合條件的經理
    (7, 'Eva Chen', '2022-07-01', 1, 2),
    (8, 'Frank Wilson', '2022-08-01', 1, 1), -- 符合條件的經理
    (9, 'Grace Davis', '2022-09-01', 1, 2),
    (10, 'Henry Wang', '2022-10-01', 1, 2);


-- 插入經理資料
INSERT INTO managers (manager_id, dept_id, emp_id)
VALUES 
    (1, 1, 1),
    (2, 1, 3),
    (3, 1, 6),
    (4, 1, 8);

-- 插入薪水資料
INSERT INTO salaries (salary_id, emp_id, salary)
VALUES 
    (1, 1, 75000),
    (2, 2, 70000),
    (3, 3, 85000),
    (4, 4, 72000),
    (5, 5, 78000),
    (6, 6, 90000),
    (7, 7, 68000),
    (8, 8, 82000),
    (9, 9, 76000),
    (10, 10, 74000);
