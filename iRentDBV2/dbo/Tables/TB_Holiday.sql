CREATE TABLE [dbo].[TB_Holiday](
    [HolidayID]        INT          IDENTITY (1, 1) NOT NULL,
    [HolidayYearMonth] VARCHAR (6)  DEFAULT ('') NOT NULL,
    [HolidayDate]      VARCHAR (10) DEFAULT ('') NOT NULL,
  [use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL, 
    CONSTRAINT [PK_TB_Holiday] PRIMARY KEY ([HolidayDate], [HolidayYearMonth])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'年月日(yyyyMMdd)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Holiday', @level2type = N'COLUMN', @level2name = N'HolidayDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'年月(yyyyMM)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Holiday', @level2type = N'COLUMN', @level2name = N'HolidayYearMonth';

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'假日列表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Holiday',
    @level2type = NULL,
    @level2name = NULL
		GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用(0:否;1:是;2:待上線)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Holiday', @level2type=N'COLUMN',@level2name=N'use_flag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Holiday', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Holiday', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

CREATE INDEX [IX_TB_Holiday_Search] ON [dbo].[TB_Holiday] ([HolidayDate], [use_flag])
