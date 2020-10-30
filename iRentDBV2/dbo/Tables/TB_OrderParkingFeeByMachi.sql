CREATE TABLE [dbo].[TB_OrderParkingFeeByMachi]
(
  [OrderNo] BIGINT   NOT NULL,
    [Amount]   INT      DEFAULT ((0)) NOT NULL,
    [MKTime]   DATETIME DEFAULT(DATEADD(HOUR,8,GETDATE())) NOT NULL, 
    [UPDTime]  DATETIME NULL, 
    CONSTRAINT [PK_TB_OrderParkingFeeByMachi] PRIMARY KEY ([OrderNo]),
)
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingFeeByMachi', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingFeeByMachi', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'停車費總計', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingFeeByMachi', @level2type = N'COLUMN', @level2name = N'Amount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'iRent訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingFeeByMachi', @level2type = N'COLUMN', @level2name = N'OrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車麻吉停車費', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderParkingFeeByMachi';


