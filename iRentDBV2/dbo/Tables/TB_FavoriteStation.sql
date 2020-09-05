CREATE TABLE [dbo].[TB_FavoriteStation]
(
    [StationID] VARCHAR(10) NOT NULL DEFAULT '', 
    [IDNO] VARCHAR(20) NOT NULL DEFAULT '', 
		[MKTime] [DATETIME] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    CONSTRAINT [PK_TB_FavoriteStation] PRIMARY KEY ([StationID], [IDNO])
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'據點代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FavoriteStation',
    @level2type = N'COLUMN',
    @level2name = N'StationID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'身份證',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FavoriteStation',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_FavoriteStation',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
