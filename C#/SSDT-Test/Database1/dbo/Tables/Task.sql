CREATE TABLE [dbo].[Task] (
    [TaskId]     INT           IDENTITY (1, 1) NOT NULL,
    [ProjectId]  INT           NOT NULL,
    [TaskName]   VARCHAR (255) NULL,
    [Priority]   VARCHAR (50)  NULL,
    [TaskStatus] VARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([TaskId] ASC),
    FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId])
);

