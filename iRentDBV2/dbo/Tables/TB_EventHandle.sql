CREATE TABLE [dbo].[TB_EventHandle]
(
	    [EventID]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [EventType]      TINYINT        DEFAULT ((0)) NOT NULL,
    [NowOrder]       BIGINT         DEFAULT ((0)) NOT NULL,
    [NextOrder]      BIGINT         DEFAULT ((0)) NOT NULL,
    [MachineNo]      VARCHAR (10)   DEFAULT ('') NOT NULL,
    [CarNo]          VARCHAR (10)   DEFAULT ('') NOT NULL,
    [Processor]      NVARCHAR (20)  DEFAULT (N'') NOT NULL,
    [isHandle]       TINYINT        DEFAULT ((0)) NOT NULL,
    [HandleDescript] NVARCHAR (300) DEFAULT (N'') NOT NULL,
    [UPDTime]        DATETIME       NULL,
    [MKTime]         DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [Remark]         NVARCHAR (300) NULL,
    CONSTRAINT [PK_TB_EventHandle] PRIMARY KEY CLUSTERED ([EventID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_INSEventHandle_New]
    ON [dbo].[TB_EventHandle]([EventType] ASC, [isHandle] ASC, [Remark] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_EventHandle_201612_Search]
    ON [dbo].[TB_EventHandle]([EventType] ASC, [isHandle] ASC, [MKTime] ASC, [CarNo] ASC, [MachineNo] ASC);


GO

