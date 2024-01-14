DECLARE @StartTime DATETIME;
DECLARE @EndTime DATETIME;

-- 記錄開始時間
SET @StartTime = GETDATE();

DECLARE @counter INT = 0;

WHILE @counter < 10
BEGIN
    -- 您的複雜查詢
    UPDATE salaries
SET salary = 90000
FROM employees
JOIN titles ON employees.title_id = titles.title_id
JOIN departments ON employees.dept_id = departments.dept_id
JOIN managers ON employees.emp_id = managers.emp_id
JOIN salaries AS s ON employees.emp_id = s.emp_id
WHERE
    departments.dept_name = 'Engineering'
    AND titles.title = 'Manager'
    AND s.salary > 75000;

    SET @counter = @counter + 1;
END

-- 記錄結束時間
SET @EndTime = GETDATE();

-- 計算執行時間差異
SELECT DATEDIFF(MILLISECOND, @StartTime, @EndTime) AS 'TotalExecutionTimeInMilliseconds';
