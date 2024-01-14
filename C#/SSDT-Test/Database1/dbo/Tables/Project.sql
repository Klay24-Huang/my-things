CREATE TABLE [dbo].[Project] (
    [ProjectId]   INT           IDENTITY (1, 1) NOT NULL,
    [ProjectName] VARCHAR (255) NULL,
    [StartDate]   DATE          NULL,
    PRIMARY KEY CLUSTERED ([ProjectId] ASC)
);

