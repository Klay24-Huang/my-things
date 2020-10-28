CREATE TABLE [dbo].[TB_tmpCrentialsPIC]
(
	[IDNO] VARCHAR(20) NOT NULL, 
    [CrentialsType] TINYINT NOT NULL DEFAULT 0, 
    [CrentialsFile] VARCHAR(8000) NOT NULL DEFAULT '', 
	[MKTime] DATETIME NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()), 
    [UPDTime] DATETIME NOT NULL, 
    CONSTRAINT [PK_TB_tmpCrentialsPIC] PRIMARY KEY ([IDNO], [CrentialsType]) 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'圖檔（以Azure檔名存入）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_tmpCrentialsPIC',
    @level2type = N'COLUMN',
    @level2name = N'CrentialsFile'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'證件照類型: 1:身份證正面;2:身份證反面;3:汽車駕照正面;4:汽車駕照反面;5:機車駕證正面;6:機車駕證反面;7:自拍照;8:法定代理人;9:其他（如台大專案）;10:企業用戶;11:簽名檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_tmpCrentialsPIC',
    @level2type = N'COLUMN',
    @level2name = N'CrentialsType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'建立時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_tmpCrentialsPIC',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_tmpCrentialsPIC',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'證件資料表（待審核）',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_tmpCrentialsPIC',
    @level2type = NULL,
    @level2name = NULL