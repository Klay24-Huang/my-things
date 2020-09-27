CREATE TABLE [dbo].[TB_BAT_Station]
(
	[BAT_Station_Id] INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [Station]        VARCHAR (75)    DEFAULT ('') NOT NULL,
    [Addr]           NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [lon]            DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [lat]            DECIMAL (9, 6)  DEFAULT ((0.0)) NOT NULL,
    [TotalCnt]       INT             DEFAULT ((0)) NOT NULL,
    [FullCnt]        INT             DEFAULT ((0)) NOT NULL,
    [EmptyCnt]       INT             DEFAULT ((0)) NOT NULL,
    [use_flag]       TINYINT         DEFAULT ((1)) NOT NULL,
    [MKTime]         DATETIME        DEFAULT  (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]        DATETIME       NULL,
    CONSTRAINT [PK_TB_BAT_Station] PRIMARY KEY CLUSTERED ([BAT_Station_Id] ASC)
)

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'空槽數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station', @level2type = N'COLUMN', @level2name = N'EmptyCnt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'95%滿電電池數量', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station', @level2type = N'COLUMN', @level2name = N'FullCnt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'總單位數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station', @level2type = N'COLUMN', @level2name = N'TotalCnt';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'緯度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station', @level2type = N'COLUMN', @level2name = N'lat';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'經度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station', @level2type = N'COLUMN', @level2name = N'lon';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station', @level2type = N'COLUMN', @level2name = N'Addr';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交換站編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station', @level2type = N'COLUMN', @level2name = N'Station';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'站點名稱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station', @level2type = N'COLUMN', @level2name = N'Name';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'pk', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station', @level2type = N'COLUMN', @level2name = N'BAT_Station_Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'電池交換站', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BAT_Station';

