CREATE TABLE [dbo].[TB_NPR340Save]
(
    [ArrearId]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [CUSTID]       VARCHAR (50)   DEFAULT ('') NOT NULL,
    [ORDNO]        VARCHAR (20)   DEFAULT ('') NOT NULL,
    [CNTRNO]       VARCHAR (30)   DEFAULT ('') NOT NULL,
    [PAYMENTTYPE]  INT            DEFAULT ((0)) NOT NULL,
    [CARNO]        VARCHAR (20)   DEFAULT ('') NOT NULL,
    [NORDNO]       VARCHAR (50)   DEFAULT ('') NOT NULL,
    [PAYDATE]      DATETIME      NULL,
    [AUTH_CODE]    VARCHAR (10)   DEFAULT ('') NOT NULL,
    [AMOUNT]       INT            DEFAULT ((0)) NOT NULL,
    [CDTMAN]       NVARCHAR (10)  DEFAULT (N'') NOT NULL,
    [CARDNO]       VARCHAR (20)   DEFAULT ('') NOT NULL,
    [POLNO]        VARCHAR (20)   DEFAULT ('') NOT NULL,
    [isRetry]      TINYINT       DEFAULT ((0)) NOT NULL,
    [MKTime]       DATETIME      DEFAULT (getdate()) NOT NULL,
    [UTime]        DATETIME      NULL,
    CONSTRAINT [PK_TB_NPR340Save] PRIMARY KEY CLUSTERED ([ArrearId] ASC)
);


GO



EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'客戶代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340Save',
    @level2type = N'COLUMN',
    @level2name = N'CUSTID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預約編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340Save',
    @level2type = N'COLUMN',
    @level2name = N'ORDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'合約編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340Save',
    @level2type = N'COLUMN',
    @level2name = N'CNTRNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'欠費類型 1.租金 2.罰單 3.停車費 4. ETAG',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340Save',
    @level2type = N'COLUMN',
    @level2name = N'PAYMENTTYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340Save',
    @level2type = N'COLUMN',
    @level2name = N'CARNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'網路刷卡訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340Save',
    @level2type = N'COLUMN',
    @level2name = N'NORDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'刷卡日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340Save',
    @level2type = N'COLUMN',
    @level2name = N'PAYDATE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'刷卡授權碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340Save',
    @level2type = N'COLUMN',
    @level2name = N'AUTH_CODE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'沖銷金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_NPR340Save',
    @level2type = N'COLUMN',
    @level2name = N'AMOUNT'