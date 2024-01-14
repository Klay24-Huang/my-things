set nocount on;
-- Execute the stored procedure 100 times
DECLARE @counter INT = 1;
DECLARE @loop INT = 10000;

-- �ŧi�ܼƨӰO���}�l�M�����ɶ�
DECLARE @StartTime DATETIME, @EndTime DATETIME;
-- �]�m�}�l�ɶ�
SET @StartTime = GETDATE();

WHILE @counter <= @loop
BEGIN
    EXEC UpdateEmployeeSalary @EmployeeID = 1, @NewSalary = 55000;
    SET @counter = @counter + 1;
END;

-- �]�m�����ɶ�
SET @EndTime = GETDATE();

-- �p���`����ɶ�
DECLARE @TotalExecutionTime INT;
SET @TotalExecutionTime = DATEDIFF(MILLISECOND, @StartTime, @EndTime);

-- ����`����ɶ�
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

-- ����`����ɶ�
PRINT 'Raw sql Total Execution Time: ' + CAST(@TotalExecutionTime AS NVARCHAR(20)) + ' milliseconds';