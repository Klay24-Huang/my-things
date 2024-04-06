CREATE TABLE [dbo].[Employee] (
    [EmployeeID] INT           NOT NULL,
    [FirstName]  NVARCHAR (50) NULL,
    [LastName]   NVARCHAR (50) NULL,
    [Salary]     INT           NULL,
    PRIMARY KEY CLUSTERED ([EmployeeID] ASC)
);

