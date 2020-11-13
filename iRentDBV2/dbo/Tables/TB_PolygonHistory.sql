﻿CREATE TABLE [dbo].[TB_PolygonHistory]
(
	[PolygonHistoryID] INT NOT NULL IDENTITY, 
    [BLOCK_ID]    INT            NOT NULL,
	[StationID]   VARCHAR(10)    DEFAULT ('') NOT NULL,
    [BlockName]   NVARCHAR (50)  DEFAULT ('') NOT NULL,
    [BlockType]   TINYINT        DEFAULT ((0)) NOT NULL,
    [PolygonMode] TINYINT        DEFAULT ((0)) NOT NULL,
    [Longitude]   VARCHAR (MAX)  DEFAULT ('') NOT NULL,
    [Latitude]    VARCHAR (MAX)  DEFAULT ('') NOT NULL,
    [MAPColor]    CHAR (6)       DEFAULT ('FF0000') NOT NULL,
    [StartDate]   DATETIME       DEFAULT  (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [EndDate]     DATETIME       DEFAULT  '2099-12-31 23:59:59' NOT NULL,
    [use_flag]    TINYINT        DEFAULT ((0)) NOT NULL,
    [MKTime]      DATETIME       DEFAULT  (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]      DATETIME       DEFAULT  (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [ADD_User] VARCHAR(50) NOT NULL DEFAULT '', 
    [UPD_User] VARCHAR(50) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_TB_PolygonHistory] PRIMARY KEY ([PolygonHistoryID]) 
)
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'BlockName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'電子欄柵類型：0:可還車;1:可還車中的不可還車;2:優惠',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'BlockType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'電子柵欄模式，0:優惠的取車;1:優惠的還車',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'PolygonMode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'經度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'Longitude'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'緯度',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'Latitude'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'顏色',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'MAPColor'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'1:啟用;0:停用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'use_flag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'此筆於TB_Polygon建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'寫入記錄的時間（TB_Polygon更新時間)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'此筆於TB_Polygon的建立者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'ADD_User'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'此筆於TB_Polygon的修改者',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'UPD_User'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'電子柵欄記錄檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = NULL,
    @level2name = NULL
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'有效日（起）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'StartDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'有效日期（迄）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'EndDate'

GO

CREATE INDEX [IX_TB_PolygonHistory_Search] ON [dbo].[TB_PolygonHistory] ([StationID],[MKTime],[BLOCK_ID])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'於TB_Polygon的pk',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PolygonHistory',
    @level2type = N'COLUMN',
    @level2name = N'BLOCK_ID'