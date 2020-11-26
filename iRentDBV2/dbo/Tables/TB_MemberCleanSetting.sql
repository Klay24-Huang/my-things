CREATE TABLE [dbo].[TB_MemberCleanSetting]
(
	 [Account]      VARCHAR (50)   DEFAULT ('') NOT NULL,
    [StationGroup] VARCHAR (300)  DEFAULT ('') NOT NULL,
    [Lat]          DECIMAL (9, 6) DEFAULT ((25.046891)) NOT NULL,
    [Lng]          DECIMAL (9, 6) DEFAULT ((121.516602)) NOT NULL,
    [MKTime]       DATETIME       DEFAULT (getdate()) NOT NULL,
    [UPDTime]      DATETIME       DEFAULT (getdate()) NOT NULL, 
    CONSTRAINT [PK_TB_MemberCleanSetting] PRIMARY KEY ([Account]),
)
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最近一次更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCleanSetting', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCleanSetting', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目前點選的經度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCleanSetting', @level2type = N'COLUMN', @level2name = N'Lng';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目前點選的緯度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCleanSetting', @level2type = N'COLUMN', @level2name = N'Lat';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'設定的管轄據點', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCleanSetting', @level2type = N'COLUMN', @level2name = N'StationGroup';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCleanSetting', @level2type = N'COLUMN', @level2name = N'Account';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'整備人員設定管轄及目前所在經緯度', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberCleanSetting';

