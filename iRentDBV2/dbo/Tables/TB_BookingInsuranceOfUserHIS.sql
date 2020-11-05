CREATE TABLE [dbo].[TB_BookingInsuranceOfUserHIS]
(
	[HisID] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [IDNO] VARCHAR(10) NOT NULL, 
    [InsuranceLevel] TINYINT NOT NULL DEFAULT 3, 
    [RentHoursCount] FLOAT NOT NULL DEFAULT 0, 
    [CriticalEventCount] TINYINT NOT NULL DEFAULT 0, 
    [MKTime] DATETIME NOT NULL, 
    [UPDTime] DATETIME NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'歷程流水號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingInsuranceOfUserHIS',
    @level2type = N'COLUMN',
    @level2name = N'HisID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'會員編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingInsuranceOfUserHIS',
    @level2type = N'COLUMN',
    @level2name = N'IDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'安心服務等級',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingInsuranceOfUserHIS',
    @level2type = N'COLUMN',
    @level2name = N'InsuranceLevel'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'累計租車時數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingInsuranceOfUserHIS',
    @level2type = N'COLUMN',
    @level2name = N'RentHoursCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'出險記錄統計',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingInsuranceOfUserHIS',
    @level2type = N'COLUMN',
    @level2name = N'CriticalEventCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'產生時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingInsuranceOfUserHIS',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingInsuranceOfUserHIS',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'