CREATE PROCEDURE UpdateEmployeeSalary
    @EmployeeID INT,
    @NewSalary INT
AS
BEGIN
    UPDATE dbo.Employee
    SET Salary = @NewSalary
    WHERE EmployeeID = @EmployeeID
END