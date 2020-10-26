CREATE TABLE [dbo].[TB_Credentials]
(
	[IDNO] VARCHAR(20) NOT NULL DEFAULT '', 
    [ID_1] SMALLINT NOT NULL DEFAULT 0, 
    [ID_2] SMALLINT NOT NULL DEFAULT 0, 
    [CarDriver_1] SMALLINT NOT NULL DEFAULT 0, 
    [CarDriver_2] SMALLINT NOT NULL DEFAULT 0, 
	[MotorDriver_1] SMALLINT NOT NULL DEFAULT 0, 
    [MotorDriver_2] SMALLINT NOT NULL DEFAULT 0, 
	[Self_1] SMALLINT NOT NULL DEFAULT 0, 
	[Law_Agent] SMALLINT NOT NULL DEFAULT 0, 
    [Other_1] SMALLINT NOT NULL DEFAULT 0, 
	[Business_1] SMALLINT NOT NULL DEFAULT 0, 
	[Signture]   SMALLINT NOT NULL DEFAULT 0,
    [MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [UPDTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    CONSTRAINT [PK_TB_Credentials] PRIMARY KEY ([IDNO]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'身份證號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'身份證正面',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'ID_1'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'身份證反面',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'ID_2'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汽車駕證正面',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'CarDriver_1'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'汽車駕照反面',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'CarDriver_2'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車駕照正面',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'MotorDriver_1'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'機車駕照反面',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'MotorDriver_2'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'自拍照',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'Self_1'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'法定代理人',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'Law_Agent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'特殊身份',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'Other_1'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'最後一次修改時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'證件照',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'企業用戶',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'Business_1'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'簽名檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Credentials',
    @level2type = N'COLUMN',
    @level2name = N'Signture'