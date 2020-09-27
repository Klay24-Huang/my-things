CREATE TABLE [dbo].[TB_KeyStore]
(
	[KeyId]         INT           IDENTITY (1, 1) NOT NULL,
    [EffectiveDate] DATE           DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [EncryedKey]    VARCHAR (256)  DEFAULT ('') NOT NULL,
    [use_flag]      TINYINT        DEFAULT ((0)) NOT NULL,
    [MKTime]        DATETIME       DEFAULT  (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]       DATETIME      NULL, 
    CONSTRAINT [PK_TB_KeyStore] PRIMARY KEY ([KeyId]),
)
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後一次更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_KeyStore', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_KeyStore', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用(0:停用;1:啟用)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_KeyStore', @level2type = N'COLUMN', @level2name = N'use_flag';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'金鑰', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_KeyStore', @level2type = N'COLUMN', @level2name = N'EncryedKey';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'有效日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_KeyStore', @level2type = N'COLUMN', @level2name = N'EffectiveDate';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'PK', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_KeyStore', @level2type = N'COLUMN', @level2name = N'KeyId';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'驗證碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_KeyStore';


