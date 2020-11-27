﻿CREATE TABLE [dbo].[TB_ReturnCarControl]
(
	[PROCD]      CHAR (1)      DEFAULT ('A') NOT NULL,
    [ORDNO]      VARCHAR (30)  DEFAULT ('') NOT NULL,
    [CNTRNO]     VARCHAR(30) NOT NULL DEFAULT (''), 
    [IRENTORDNO] BIGINT        DEFAULT ((0)) NOT NULL,
    [CUSTID]     VARCHAR (10)  DEFAULT ('') NOT NULL,
    [CUSTNM]     NVARCHAR (10) DEFAULT (N'') NOT NULL,
    [BIRTH]      VARCHAR (10)  DEFAULT ('') NOT NULL,
    [CUSTTYPE]   TINYINT       DEFAULT ((1)) NOT NULL,
    [ODCUSTID]   VARCHAR (1)   DEFAULT ('') NOT NULL,
    [CARTYPE]    VARCHAR (10)  DEFAULT ('') NOT NULL,
    [CARNO]      VARCHAR (10)  DEFAULT ('') NOT NULL,
    [TSEQNO]     VARCHAR (10)  DEFAULT ('') NOT NULL,
    [GIVEDATE]   VARCHAR (8)   DEFAULT ('') NOT NULL,
    [GIVETIME]   VARCHAR (4)   DEFAULT ('') NOT NULL,
    [RENTDAYS]   FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [GIVEKM]     FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [OUTBRNHCD]  VARCHAR (8)   DEFAULT ('') NOT NULL,
    [RNTDATE]    VARCHAR (8)   DEFAULT ('') NOT NULL,
    [RNTTIME]    VARCHAR (4)   DEFAULT ('') NOT NULL,
    [RNTKM]      FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [INBRNHCD]   VARCHAR (8)   DEFAULT ('') NOT NULL,
    [RPRICE]     INT           DEFAULT ((0)) NOT NULL,
    [RINSU]      INT           DEFAULT ((0)) NOT NULL,
    [DISRATE]    FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [OVERHOURS]  FLOAT (53)    DEFAULT ((0)) NOT NULL,
    [OVERAMT2]   INT           DEFAULT ((0)) NOT NULL,
    [RNTAMT]     INT           DEFAULT ((0)) NOT NULL,
    [RENTAMT]    INT           DEFAULT ((0)) NOT NULL,
    [LOSSAMT2]   INT           DEFAULT ((0)) NOT NULL,
    [PROJID]     VARCHAR (10)  DEFAULT ('') NOT NULL,
    [REMARK]     VARCHAR (30)  DEFAULT ('') NOT NULL,
    [INVKIND]    TINYINT       DEFAULT ((0)) NOT NULL,
    [UNIMNO]     VARCHAR (20)  DEFAULT ('') NOT NULL,
    [INVTITLE]   NVARCHAR (50) DEFAULT ('') NOT NULL,
    [INVADDR]    NVARCHAR (80) DEFAULT ('') NOT NULL,
    [GIFT]       INT           DEFAULT ((0)) NOT NULL,
    [GIFT_MOTO]  INT           DEFAULT (0) NOT NULL,
    [CARDNO]     VARCHAR (20)  DEFAULT ('') NOT NULL,
    [PAYAMT]     INT           DEFAULT (0) NOT NULL,
    [AUTHCODE]   VARCHAR (30)  DEFAULT ('') NOT NULL,
    [isRetry]    TINYINT       DEFAULT ((0)) NOT NULL,
    [RetryTimes] TINYINT       DEFAULT ((0)) NOT NULL,
    [eTag]       INT           DEFAULT (0) NOT NULL,
    [CARRIERID]  VARCHAR (100) NULL,
    [NPOBAN]     VARCHAR (100) NULL,
    [NOCAMT]     INT NOT NULL DEFAULT 0, 
    [PARKINGAMT2] INT          DEFAULT (0) NOT NULL,
    [MKTime]     DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]    DATETIME      NULL,
    
    CONSTRAINT [PK_TB_ReturnCarControl] PRIMARY KEY CLUSTERED ([IRENTORDNO] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_ReturnCarControl_ReturnIndex]
    ON [dbo].[TB_ReturnCarControl]([isRetry] ASC, [RetryTimes] ASC);

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租130',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'代收停車費',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'PARKINGAMT2'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租合約編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'CNTRNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租預約編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'ORDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'生日',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'BIRTH'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'客戶類型',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'CUSTTYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'簽約客戶',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'ODCUSTID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車型',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'CARTYPE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'CARNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'車輛流水號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'TSEQNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'出車日期',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'GIVEDATE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'出車時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'GIVETIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租用天數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'RENTDAYS'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'出車里程',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'GIVEKM'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'出車據點',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'OUTBRNHCD'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'還車日期',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'RNTDATE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'還車時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'RNTTIME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'還車里程',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'RNTKM'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'還車據點',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'INBRNHCD'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日租金',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'RPRICE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'日保費',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'RINSU'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'折扣',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'DISRATE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'逾時時數',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'OVERHOURS'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'逾時費用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'OVERAMT2'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'還車費用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'RNTAMT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'租金小計',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'RENTAMT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'油料補貼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'LOSSAMT2'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案代碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'PROJID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'網刷訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'REMARK'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'發票寄送方式1:捐贈 2:EMAIL 3:郵寄二聯 4:郵寄三聯 5:手機載具(會員設定)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'INVKIND'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'發票統編',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'UNIMNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'發票抬頭',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'INVTITLE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'發票地址',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'INVADDR'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'折抵時數-汽車',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'GIFT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'折抵時數-機車',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'GIFT_MOTO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'信用卡號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'CARDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'實際付款金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'PAYAMT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'授權碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'AUTHCODE'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'etag費用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'eTag'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'載具條碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'CARRIERID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'愛心碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'NPOBAN'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'安心服務費用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'NOCAMT'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'產生時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'MKTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'更新時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'irent訂單編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'IRENTORDNO'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'客戶編號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'CUSTID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'客戶名稱',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_ReturnCarControl',
    @level2type = N'COLUMN',
    @level2name = N'CUSTNM'