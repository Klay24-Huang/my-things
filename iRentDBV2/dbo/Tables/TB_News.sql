CREATE TABLE [dbo].[TB_News]
(
	  [NewsID]    INT            IDENTITY (1, 1) NOT NULL,
    [Title]     NVARCHAR (50)  DEFAULT (N'') NOT NULL,
    [NewsType]  TINYINT        DEFAULT ((0)) NOT NULL,
    [Content]   NVARCHAR (100) DEFAULT (N'') NOT NULL,
    [URL]       VARCHAR (250)  DEFAULT ('') NOT NULL,
    [SD]        DATETIME       NULL,
    [ED]        DATETIME       NULL,
    [isSend]    TINYINT        DEFAULT ((0)) NOT NULL,
    [NewsClass] TINYINT        DEFAULT ((1)) NOT NULL,
    [isTop]     VARCHAR (10)    DEFAULT ('0') NULL,
    CONSTRAINT [PK_TB_News201612] PRIMARY KEY CLUSTERED ([NewsID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_News_SendDate]
    ON [dbo].[TB_News]([SD] ASC, [ED] ASC, [isSend] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否推播(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_News', @level2type = N'COLUMN', @level2name = N'isSend';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訊息類型(0:一般;1:edm)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_News', @level2type = N'COLUMN', @level2name = N'NewsType';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'活動通知',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_News',
    @level2type = NULL,
    @level2name = NULL