CREATE TABLE [dbo].[TB_MemberDataBlock]
(
	[MEMIDNO] VARCHAR(10) NOT NULL PRIMARY KEY DEFAULT (''), 
    [STADT] VARCHAR(8) NOT NULL DEFAULT (''), 
    [ENDDT] VARCHAR(8) NOT NULL DEFAULT ('')
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'會員編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberDataBlock',
    @level2type = N'COLUMN',
    @level2name = N'MEMIDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'拒往起始日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberDataBlock',
    @level2type = N'COLUMN',
    @level2name = N'STADT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'拒往結束日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberDataBlock',
    @level2type = N'COLUMN',
    @level2name = N'ENDDT'