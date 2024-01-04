CREATE TABLE [dbo].[Assignment] (
    [AssignmentId] INT IDENTITY (1, 1) NOT NULL,
    [EmployeeId]   INT NOT NULL,
    [TaskId]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([AssignmentId] ASC),
    FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId]),
    FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Task] ([TaskId])
);

