CREATE TABLE [dbo].[TB_Arrear]
(
	 [ArrearId]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [RealOrderNum] VARCHAR (50) DEFAULT ('') NOT NULL,
    [CUSTID]       VARCHAR (50)   DEFAULT ('') NOT NULL,
    [ORDNO]        VARCHAR (20)   DEFAULT ('') NOT NULL,
    [CNTRNO]       VARCHAR (30)   DEFAULT ('') NOT NULL,
    [PAYMENTTYPE]  INT            DEFAULT ((0)) NOT NULL,
    [CARNO]        VARCHAR (20)   DEFAULT ('') NOT NULL,
    [NORDNO]       VARCHAR (50)  DEFAULT ('') NOT NULL,
    [AUTH_CODE]    VARCHAR (10)   DEFAULT ('') NOT NULL,
    [AMOUNT]       INT           DEFAULT ((0)) NOT NULL,
    [CDTMAN]       NVARCHAR (10) DEFAULT (N'') NOT NULL,
    [CARDNO]       VARCHAR (20)   DEFAULT ('') NOT NULL,
    [POLNO]        VARCHAR (20) DEFAULT ('') NOT NULL,
    [PAYDATE]      DATETIME      NULL,
    [isRetry]      TINYINT       DEFAULT ((0)) NOT NULL,
    [MKTime]       DATETIME      DEFAULT  (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UTime]        DATETIME      NULL, 
    CONSTRAINT [PK_TB_Arrear] PRIMARY KEY ([ArrearId]),
)
GO
CREATE NONCLUSTERED INDEX [IX_UPDArrear_Search]
    ON [dbo].[TB_Arrear]([RealOrderNum] ASC);
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'欠費查詢資料表',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_Arrear',
    @level2type = NULL,
    @level2name = NULL