CREATE TABLE [dbo].[TB_TradeClose] (
    [CloseID]         BIGINT       IDENTITY (1, 1) NOT NULL,
    [TradeID]         BIGINT       CONSTRAINT [DF_TB_TradeClose_TradeID] DEFAULT ((0)) NOT NULL,
    [OrderNo]         BIGINT       CONSTRAINT [DF_TB_TradeClose_OrderNo] DEFAULT ((0)) NOT NULL,
    [MerchantTradeNo] VARCHAR (30) CONSTRAINT [DF_TB_TradeClose_MerchantTradeNo] DEFAULT ('') NOT NULL,
    [CardType]        INT          CONSTRAINT [DF_TB_TradeClose_CardType] DEFAULT ((0)) NOT NULL,
    [AuthType]        INT          CONSTRAINT [DF_TB_TradeClose_AuthType] DEFAULT ((0)) NOT NULL,
    [ChkClose]        INT          CONSTRAINT [DF_TB_TradeClose_ChkClose] DEFAULT ((0)) NOT NULL,
    [IsClose]         INT          CONSTRAINT [DF_TB_TradeClose_IsClose] DEFAULT ((0)) NOT NULL,
    [CloseAmout]      INT          CONSTRAINT [DF_TB_TradeClose_CloseAmout] DEFAULT ((0)) NOT NULL,
    [CloseTime]       DATETIME     NULL,
    [CloseCount]      SMALLINT     CONSTRAINT [DF_TB_TradeClose_isRetry] DEFAULT ((0)) NOT NULL,
    [A_PRGID]         VARCHAR (50) CONSTRAINT [DF_TB_TradeClose_A_PRGID] DEFAULT ('') NOT NULL,
    [A_USERID]        VARCHAR (20) CONSTRAINT [DF_TB_TradeClose_A_USERID] DEFAULT ('') NOT NULL,
    [A_SYSDT]         DATETIME     CONSTRAINT [DF_TB_TradeClose_A_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [U_PRGID]         VARCHAR (50) CONSTRAINT [DF_TB_TradeClose_U_PRGID] DEFAULT ('') NOT NULL,
    [U_USERID]        VARCHAR (20) CONSTRAINT [DF_TB_TradeClose_U_USERID] DEFAULT ('') NOT NULL,
    [U_SYSDT]         DATETIME     CONSTRAINT [DF_TB_TradeClose_U_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    CONSTRAINT [PK_TB_TradeClose] PRIMARY KEY NONCLUSTERED ([CloseID] ASC)
);




GO
CREATE CLUSTERED INDEX [IX_TB_TradeClose]
    ON [dbo].[TB_TradeClose]([TradeID] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'U_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'U_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'U_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'A_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'A_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'A_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'關帳時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'CloseTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'關帳金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'CloseAmout';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'關帳狀態(0:未關帳; 1:已關帳; 2:已送收單行處理中)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'IsClose';




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'可否關帳 0:不關 1:要關 2:退貨', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'ChkClose';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權類別TB_Code.MapCode (1:預約; 2:訂金; 3:取車; 4:延長用車; 5:逾時;6:欠費; 7:還車)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'AuthType';




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用卡類別 0:和泰、1:台新', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'CardType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'廠商訂單編號(TB_Trade)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'MerchantTradeNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單編號(TB_Trade)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'OrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'TB_Trade流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'TradeID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'關帳紀錄檔流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'CloseID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'關帳次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TradeClose', @level2type = N'COLUMN', @level2name = N'CloseCount';

