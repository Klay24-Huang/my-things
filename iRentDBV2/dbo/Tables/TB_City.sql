CREATE TABLE [dbo].[TB_City]
(
	[CityID] [tinyint] NOT NULL,
	[CityName] [nvarchar](10) NOT NULL DEFAULT N'', 
    CONSTRAINT [PK_TB_City] PRIMARY KEY ([CityID]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'縣市',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_City',
    @level2type = N'COLUMN',
    @level2name = N'CityName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'台灣縣市',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_City',
    @level2type = NULL,
    @level2name = NULL