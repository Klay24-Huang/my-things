CREATE TABLE [dbo].[TB_Project]
(
	[PROJID] [varchar](10) NOT NULL DEFAULT ('') ,
	[PRONAME] [nvarchar](50) NOT NULL DEFAULT (N'') ,
	[PRODESC] [nvarchar](1000) NOT NULL DEFAULT (N'') ,
	[PROHOUR] [float] NOT NULL DEFAULT ((0.00)) ,
	[PROHOUR_MAX] [float] NOT NULL DEFAULT ((0.00)) ,
	[PROTYPE_N] [char](1) NOT NULL DEFAULT (''),
	[PROTYPE_H] [char](1) NOT NULL DEFAULT (''),
	[PROPRICE_N] [float] NOT NULL  DEFAULT ((0.00)),
	[PROPRICE_H] [float] NOT NULL  DEFAULT ((0.00)),
	[ShowStart] [datetime] NULL,
	[ShowEnd] [datetime] NULL,
	[PRSTDT] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[PRENDT] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[SPCLOCK] [char](1) NOT NULL DEFAULT (''),
	[RNTDTLOCK] [char](1) NOT NULL DEFAULT (''),
	[SPECProj] [tinyint] NOT NULL DEFAULT ((0)),
	[use_flag] [tinyint] NOT NULL DEFAULT (1),
	[MKTime] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[UPDTime] [datetime] NULL,
	[PROJTYPE] [int] NULL  DEFAULT ((0)) ,
	[PayMode] [tinyint] NOT NULL  DEFAULT ((0)) ,
	[SORT] [int] NOT NULL  DEFAULT ((99)) , 
	[IsMonthRent] [TINYINT] NOT NULL DEFAULT(0),
	[IsRegional] [int] NOT NULL  DEFAULT ((0)) ,
    CONSTRAINT [PK_TB_Project] PRIMARY KEY ([PROJID]),
)


GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠專案代碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PROJID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠專案名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PRONAME'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠專案描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PRODESC'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'專案時數下限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PROHOUR'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'專案時數上限' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PROHOUR_MAX'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'計費方式(平日) A : 專案價格 B : 折扣方式(抵金額) C : 折扣方式(純折扣)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PROTYPE_N'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'計費方式(假日) A : 專案價格 B : 折扣方式(抵金額) C : 折扣方式(純折扣)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PROTYPE_H'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'專案價格/折扣/折價(平日)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PROPRICE_N'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'專案價格/折扣/折價(假日)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PROPRICE_H'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'露出時間（起）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'ShowStart'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'露出時間(迄）' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'ShowEnd'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'專案起日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PRSTDT'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'專案迄日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PRENDT'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'特殊身分鎖定 (Y/N)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'SPCLOCK'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'還車時間不得超過專案迄日(Y:可以;N:不行)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'RNTDTLOCK'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'甲租乙還專案(0:否;1:是)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'SPECProj'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'建立時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最近一次修改時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'計價模式(0:以時計費;1:以分計費)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_Project', @level2type=N'COLUMN',@level2name=N'PayMode'

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否為月租專案(0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Project',
    @level2type = N'COLUMN',
    @level2name = N'IsMonthRent'
GO

CREATE INDEX [IX_TB_Project_Search] ON [dbo].[TB_Project] ([SPCLOCK], [ShowEnd], [ShowStart], [PROJTYPE], [IsMonthRent])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案列表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Project',
    @level2type = NULL,
    @level2name = NULL