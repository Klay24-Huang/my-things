set nocount on;
-- Execute the stored procedure 100 times
DECLARE @counter INT = 1;
DECLARE @loop INT = 10000;

-- 宣告變數來記錄開始和結束時間
DECLARE @StartTime DATETIME, @EndTime DATETIME;
-- 設置開始時間
SET @StartTime = GETDATE();

WHILE @counter <= @loop
BEGIN
    EXEC UpdateEmployeeSalary @EmployeeID = 1, @NewSalary = 55000;
    SET @counter = @counter + 1;
END;

-- 設置結束時間
SET @EndTime = GETDATE();

-- 計算總執行時間
DECLARE @TotalExecutionTime INT;
SET @TotalExecutionTime = DATEDIFF(MILLISECOND, @StartTime, @EndTime);

-- 顯示總執行時間
PRINT 'SP Total Execution Time: ' + CAST(@TotalExecutionTime AS NVARCHAR(20)) + ' milliseconds';


-- Execute the raw SQL query 100 times

set @counter = 1;
SET @StartTime = GETDATE();

WHILE @counter <= @loop
BEGIN
    UPDATE dbo.Employee
    SET Salary = 55000
    WHERE EmployeeID = 1;
    SET @counter = @counter + 1;
END;

SET @EndTime = GETDATE();

SET @TotalExecutionTime = DATEDIFF(MILLISECOND, @StartTime, @EndTime);

-- 顯示總執行時間
PRINT 'Raw sql Total Execution Time: ' + CAST(@TotalExecutionTime AS NVARCHAR(20)) + ' milliseconds';