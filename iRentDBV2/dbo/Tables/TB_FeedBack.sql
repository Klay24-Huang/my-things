CREATE TABLE [dbo].[TB_FeedBack]
(
	  [FeedBackID]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [IDNO]           VARCHAR (20)    DEFAULT ('') NOT NULL,
    [OrderNo]   BIGINT          DEFAULT ((0)) NOT NULL,
    [mode]           TINYINT         DEFAULT ((0)) NOT NULL,
    [FeedBackKind]   VARCHAR(1024)   DEFAULT ('') NOT NULL,
    [descript]       NVARCHAR (500)  DEFAULT (N'') NOT NULL,
    [star]           TINYINT         DEFAULT ((0)) NOT NULL,
    [PIC1]           VARCHAR (100)   DEFAULT ('') NOT NULL,
    [PIC2]           VARCHAR (100)   DEFAULT ('') NOT NULL,
    [PIC3]           VARCHAR (100)   DEFAULT ('') NOT NULL,
    [PIC4]           VARCHAR (100)   DEFAULT ('') NOT NULL,
    [handleDescript] NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [opt]            NVARCHAR (20)   DEFAULT (N'') NOT NULL,
    [isHandle]       TINYINT         DEFAULT ((0)) NOT NULL,
    [MKTime]         DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]        DATETIME       NULL,
    CONSTRAINT [PK_TB_FeedBack] PRIMARY KEY CLUSTERED (FeedBackID ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_FeedBackSearch]
    ON [dbo].[TB_FeedBack]([mode] ASC, [OrderNo] ASC, [star] ASC);
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_FeedBack', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'回饋時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_FeedBack', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'0:未處理;1:已處理', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_FeedBack', @level2type = N'COLUMN', @level2name = N'isHandle';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'星星數，當mode=1時才有意義', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_FeedBack', @level2type = N'COLUMN', @level2name = N'star';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'回饋內容', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_FeedBack', @level2type = N'COLUMN', @level2name = N'descript';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'來源：0:取車;1:還車;2:關於iRent', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_FeedBack', @level2type = N'COLUMN', @level2name = N'mode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_FeedBack', @level2type = N'COLUMN', @level2name = N'OrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'回饋主檔', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_FeedBack';
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'回饋類別',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FeedBack',
    @level2type = N'COLUMN',
    @level2name = N'FeedBackKind'