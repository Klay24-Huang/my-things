CREATE TABLE [dbo].[TB_InsuranceInfo]
(
	[InsuranceLevel] INT NOT NULL , 
    [CarTypeGroupCode] VARCHAR(20) NOT NULL DEFAULT (''), 
    [InsurancePerHours] FLOAT NOT NULL DEFAULT 0, 
    [useflg] VARCHAR NOT NULL DEFAULT ('Y'), 
    [MKTime] DATETIME NOT NULL, 
    [UPDTime] DATETIME NULL, 
    PRIMARY KEY ([InsuranceLevel], [CarTypeGroupCode])

)
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'級距等級' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_InsuranceInfo', @level2type=N'COLUMN',@level2name=N'InsuranceLevel'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'車型對應' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_InsuranceInfo', @level2type=N'COLUMN',@level2name=N'CarTypeGroupCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'每小時價格' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_InsuranceInfo', @level2type=N'COLUMN',@level2name=N'InsurancePerHours'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否啟用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_InsuranceInfo', @level2type=N'COLUMN',@level2name=N'useflg'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'產生時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_InsuranceInfo', @level2type=N'COLUMN',@level2name=N'MKTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'更新時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_InsuranceInfo', @level2type=N'COLUMN',@level2name=N'UPDTime'
GO

