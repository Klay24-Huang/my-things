CREATE TABLE [dbo].[TB_BookingControl]
(
	[order_number] BIGINT        NOT NULL,
    [PROCD]        CHAR (1)      DEFAULT ('A') NOT NULL,
    [ODCUSTID]     VARCHAR (10)  DEFAULT ('') NOT NULL,
    [ODCUSTNM]     NVARCHAR (10) DEFAULT (N'') NOT NULL,
    [TEL1]         VARCHAR (20)  DEFAULT ('') NOT NULL,
    [TEL2]         VARCHAR (20)  DEFAULT ('') NOT NULL,
    [TEL3]         VARCHAR (20)  DEFAULT ('') NOT NULL,
    [ODDATE]       VARCHAR (8)   DEFAULT ('') NOT NULL,
    [GIVEDATE]     VARCHAR (8)   DEFAULT ('') NOT NULL,
    [GIVETIME]     VARCHAR (4)   DEFAULT ('') NOT NULL,
    [RNTDATE]      VARCHAR (8)   DEFAULT ('') NOT NULL,
    [RNTTIME]      VARCHAR (4)   DEFAULT ('') NOT NULL,
    [CARTYPE]      VARCHAR (10)  DEFAULT ('') NOT NULL,
    [CARNO]        VARCHAR (10)  DEFAULT ('') NOT NULL,
    [OUTBRNH]      VARCHAR (6)   DEFAULT ('') NOT NULL,
    [INBRNH]       VARCHAR (6)   DEFAULT ('') NOT NULL,
    [ORDAMT]       INT           DEFAULT ((0)) NOT NULL,
    [REMARK]       NVARCHAR (50) DEFAULT (N'') NOT NULL,
    [PAYAMT]       INT           DEFAULT ((0)) NOT NULL,
    [RPRICE]       INT           DEFAULT ((0)) NOT NULL,
    [RINV]         INT           DEFAULT ((0)) NOT NULL,
    [DISRATE]      FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [NETRPRICE]    INT           DEFAULT ((0)) NOT NULL,
    [RNTAMT]       INT           DEFAULT ((0)) NOT NULL,
    [INSUAMT]      INT           DEFAULT ((0)) NOT NULL,
    [RENTDAY]      FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [EBONUS]       INT           DEFAULT ((0)) NOT NULL,
    [PROJTYPE]     VARCHAR (6)   DEFAULT ('') NOT NULL,
    [TYPE]         TINYINT       DEFAULT ((1)) NOT NULL,
    [INVKIND]      TINYINT       DEFAULT ((0)) NOT NULL,
    [INVTITLE]     NVARCHAR (20) DEFAULT ('') NOT NULL,
    [UNIMNO]       VARCHAR (20)  DEFAULT ('') NOT NULL,
    [TSEQNO]       VARCHAR (10)  DEFAULT ('') NOT NULL,
    [ORDNO]        VARCHAR (50)  DEFAULT ('') NOT NULL,
    [MKTime]       DATETIME      DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]      DATETIME      NULL,
    [isRetry]      TINYINT       DEFAULT ((0)) NOT NULL,
    [RetryTimes]   TINYINT       DEFAULT ((0)) NOT NULL,
    [CARRIERID]    VARCHAR (100) NOT NULL DEFAULT '',
    [NPOBAN]       VARCHAR (100) NOT NULL DEFAULT '',
    [NOCAMT] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_TB_BookingControl] PRIMARY KEY CLUSTERED ([order_number] ASC)
)
GO
CREATE NONCLUSTERED INDEX [IX_UPD_BookingControl_isRetry]
    ON [dbo].[TB_BookingControl]([isRetry] ASC)
    INCLUDE([order_number], [ODCUSTID]);
GO
CREATE NONCLUSTERED INDEX [IX_TB_BookingControl_201611_Retry]
    ON [dbo].[TB_BookingControl]([isRetry] ASC, [RetryTimes] ASC);

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'重試次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BookingControl', @level2type = N'COLUMN', @level2name = N'RetryTimes';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否Retry', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BookingControl', @level2type = N'COLUMN', @level2name = N'isRetry';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'處理區分(A、U、F)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_BookingControl', @level2type = N'COLUMN', @level2name = N'PROCD';
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租060',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControl',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'安心服務費',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_BookingControl',
    @level2type = N'COLUMN',
    @level2name = N'NOCAMT'