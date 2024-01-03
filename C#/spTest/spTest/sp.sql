-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE SP_GetEmployees
AS
BEGIN
SELECT [e].[EmployeeId]
    ,[e].[DepartmentId]
    ,[e].[EmployeeName]
    ,[e].[EmployeeStatus]
    ,[e].[EmployeeType]
    ,[e].[LeaveEndDate]
    ,[e].[LeaveStartDate]
    ,[e].[Salary]
    ,[d].[DepartmentId]
    ,[d].[DepartmentName]
    ,[d].[Location]
    ,[t0].[AssignmentId]
    ,[t0].[EmployeeId]
    ,[t0].[TaskId]
    ,[t0].[TaskId0]
    ,[t0].[Priority]
    ,[t0].[ProjectId]
    ,[t0].[TaskName]
    ,[t0].[TaskStatus]
FROM [Employee] AS [e]
INNER JOIN [Department] AS [d] ON [e].[DepartmentId] = [d].[DepartmentId]
LEFT JOIN (
    SELECT [a0].[AssignmentId]
        ,[a0].[EmployeeId]
        ,[a0].[TaskId]
        ,[t1].[TaskId] AS [TaskId0]
        ,[t1].[Priority]
        ,[t1].[ProjectId]
        ,[t1].[TaskName]
        ,[t1].[TaskStatus]
    FROM [Assignment] AS [a0]
    INNER JOIN [Task] AS [t1] ON [a0].[TaskId] = [t1].[TaskId]
    WHERE [t1].[Priority] = 'High'
    ) AS [t0] ON [e].[EmployeeId] = [t0].[EmployeeId]
WHERE (
        (
            (
                [d].[DepartmentName] IN (
                    'IT'
                    ,'Marketing'
                    ,'Finance'
                    )
                AND ([e].[EmployeeType] = 'Full-Time')
                )
            AND ([e].[Salary] >= 60000.0)
            )
        AND ([e].[Salary] <= 120000.0)
        )
    AND EXISTS (
        SELECT 1
        FROM [Assignment] AS [a]
        INNER JOIN [Task] AS [t] ON [a].[TaskId] = [t].[TaskId]
        WHERE ([e].[EmployeeId] = [a].[EmployeeId])
            AND ([t].[Priority] = 'High')
        )
ORDER BY [e].[EmployeeId]
    ,[d].[DepartmentId]
    ,[t0].[AssignmentId]
END
GO
