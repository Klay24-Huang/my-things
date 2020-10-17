CREATE TABLE [dbo].[TB_lendCarControl]
(
	 [PROCD]      CHAR (1)      DEFAULT ('A') NOT NULL,
    [ORDNO]      VARCHAR (30)  DEFAULT ('') NOT NULL,
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
    [OVERHOURS]  INT           DEFAULT ((0)) NOT NULL,
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
    [MKTime]     DATETIME      DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [UPDTime]    DATETIME      NULL,
    [isRetry]    TINYINT       DEFAULT ((0)) NOT NULL,
    [RetryTimes] TINYINT       DEFAULT ((0)) NOT NULL,
    [CARRIERID]  VARCHAR (100) NULL,
    [NPOBAN]     VARCHAR (100) NULL,
    [NOCAMT] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_TB_lendCarControl] PRIMARY KEY CLUSTERED ([IRENTORDNO] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TB_lendCarControl_Retry]
    ON [dbo].[TB_lendCarControl]([isRetry] ASC, [RetryTimes] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TB_lendCarControl_INDEX]
    ON [dbo].[TB_lendCarControl]([IRENTORDNO] ASC);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租125',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_lendCarControl',
    @level2type = NULL,
    @level2name = NULL