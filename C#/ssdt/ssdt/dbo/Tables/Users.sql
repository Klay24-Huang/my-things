CREATE TABLE [dbo].[Users] (
    [UserId]      INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (50)  NOT NULL,
    [LastName]    NVARCHAR (50)  NOT NULL,
    [DateOfBirth] DATE           NULL,
    [Email]       NVARCHAR (100) NOT NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NULL,
    [IsActive]    BIT            DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);

