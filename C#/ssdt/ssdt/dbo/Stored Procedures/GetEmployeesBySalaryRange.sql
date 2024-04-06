CREATE PROCEDURE GetEmployeesBySalaryRange
    @MinSalary INT,
    @MaxSalary INT
AS
BEGIN
    SELECT * FROM Employee
    WHERE Salary BETWEEN @MinSalary AND @MaxSalary
END