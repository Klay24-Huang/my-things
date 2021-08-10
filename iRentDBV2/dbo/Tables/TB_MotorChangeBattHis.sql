CREATE TABLE [dbo].[TB_MotorChangeBattHis]
(
	[SEQNO] [int] IDENTITY(1,1) NOT NULL,
	[order_number] [bigint] NOT NULL,
	[ChgTimes] [int] NOT NULL,
	[RSOC_S] [float] NOT NULL,
	[RSOC_E] [float] NOT NULL,
	[ChgGift] [int] NOT NULL,
	[RewardGift] [int] NOT NULL,
	[TotalGift] [int] NOT NULL,
	[MKTime] [datetime] NULL,
)





GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'流水號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MotorChangeBattHis',
    @level2type = N'COLUMN',
    @level2name = N'SEQNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MotorChangeBattHis',
    @level2type = N'COLUMN',
    @level2name = N'order_number'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'換電次數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MotorChangeBattHis',
    @level2type = N'COLUMN',
    @level2name = N'ChgTimes'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'取車電量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MotorChangeBattHis',
    @level2type = N'COLUMN',
    @level2name = N'RSOC_S'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'還車電量',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MotorChangeBattHis',
    @level2type = N'COLUMN',
    @level2name = N'RSOC_E'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'換電時數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MotorChangeBattHis',
    @level2type = N'COLUMN',
    @level2name = N'ChgGift'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'獎勵時數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MotorChangeBattHis',
    @level2type = N'COLUMN',
    @level2name = N'RewardGift'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'總時數=換電+獎勵',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MotorChangeBattHis',
    @level2type = N'COLUMN',
    @level2name = N'TotalGift'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'產生時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MotorChangeBattHis',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'