CREATE TABLE [dbo].[Department] (
    [DepartmentId]   INT           IDENTITY (1, 1) NOT NULL,
    [DepartmentName] VARCHAR (255) NULL,
    [Location]       VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([DepartmentId] ASC)
);

