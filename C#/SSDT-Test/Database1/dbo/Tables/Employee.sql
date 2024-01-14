CREATE TABLE [dbo].[Employee] (
    [EmployeeId]     INT             IDENTITY (1, 1) NOT NULL,
    [EmployeeName]   VARCHAR (255)   NULL,
    [DepartmentId]   INT             NOT NULL,
    [Salary]         DECIMAL (10, 2) NULL,
    [EmployeeType]   VARCHAR (50)    NULL,
    [EmployeeStatus] VARCHAR (50)    NULL,
    [LeaveStartDate] DATE            NULL,
    [LeaveEndDate]   DATE            NULL,
    PRIMARY KEY CLUSTERED ([EmployeeId] ASC),
    FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Department] ([DepartmentId])
);

