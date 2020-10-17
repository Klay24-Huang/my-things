CREATE TABLE [dbo].[TB_Mobile]
(
 [MobileID]  INT          IDENTITY (1, 1) NOT NULL,
    [MobileNum] VARCHAR (50) DEFAULT ('') NOT NULL,
    [SIMCardNo] VARCHAR(128) DEFAULT('') NOT NULL,
    [last_Opt] [NVARCHAR](10) NOT NULL DEFAULT 'SYS',
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_Mobile] PRIMARY KEY ([MobileID]) 
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手機號碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Mobile', @level2type = N'COLUMN', @level2name = N'MobileNum';
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Mobile', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Mobile', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO


EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'門號列表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Mobile',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'遠傳sim卡編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Mobile',
    @level2type = N'COLUMN',
    @level2name = N'SIMCardNo'