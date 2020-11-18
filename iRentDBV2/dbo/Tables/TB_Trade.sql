CREATE TABLE [dbo].[TB_Trade]
(
	[TradeID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [OrderNo]        BIGINT   DEFAULT 0 NOT NULL,
    [MerchantTradeNo]  VARCHAR (30)   DEFAULT ('') NOT NULL,
    [CreditType]       TINYINT        DEFAULT ((0)) NOT NULL,
    [MerchantMemberID] VARCHAR (10)   DEFAULT ('') NOT NULL,
    [RetCode]          VARCHAR(10)            DEFAULT ((-1)) NOT NULL,
    [RetMsg]           NVARCHAR (400) DEFAULT (N'') NOT NULL,
    [TaishinTradeNo]    VARCHAR (50)   DEFAULT ('') NOT NULL,
    [CardNumber]          VARCHAR (20)    DEFAULT ('') NOT NULL,
    [process_date]     DATETIME       NULL,
    [AUTHAMT]          INT            DEFAULT ((0)) NOT NULL,
    [amount]           INT            NOT NULL,
    [AuthIdResp]              INT            DEFAULT ((-1)) NOT NULL,
    [IsSuccess]        INT        DEFAULT ((0)) NOT NULL,
    [MKTime]           DATETIME       DEFAULT (DATEADD(HOUR,8,getdate())) NOT NULL,
    [UPDTime]          DATETIME       NULL, 
    CONSTRAINT [PK_TB_Trade] PRIMARY KEY ([TradeID]),
)
GO
CREATE NONCLUSTERED INDEX [IX_INSAllPayForLand]
    ON [dbo].TB_Trade([CreditType] ASC, [TaishinTradeNo] ASC)
    INCLUDE([OrderNo]);


GO
CREATE NONCLUSTERED INDEX [IX_CreditUpdate]
    ON [dbo].TB_Trade([MerchantTradeNo] ASC, [CreditType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SearchForWS134]
    ON [dbo].TB_Trade([CreditType] ASC, [IsSuccess] ASC, [TaishinTradeNo] ASC)
    INCLUDE([OrderNo], [AUTHAMT]);


GO
CREATE NONCLUSTERED INDEX [IX_TB_TradeForAllPay_201705_Search]
    ON [dbo].TB_Trade([OrderNo] ASC, [CreditType] ASC, [MerchantTradeNo] ASC, [MerchantMemberID] ASC, [RetCode] ASC, [IsSuccess] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'收到回覆時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否成功0:失敗或未送出;1:成功', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'IsSuccess';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'3D(VBV) 回傳值，(eci=5,6,2,1 代表該筆交易不可否認)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = 'AuthIdResp';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'amount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'AUTHAMT';


GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'處理時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'process_date';


GO



GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'卡號前六碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = 'CardNumber';


GO



GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'台新交易序號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'TaishinTradeNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易訊息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = 'RetMsg';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易狀態：1:成功，其餘失敗', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = 'RetCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'身份證', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'MerchantMemberID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易類型：0:租金;1:etag補繳;2:補繳;3:直接取款', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'CreditType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'送出的交易序號，對應CreditType，規則：HXXXXX+代碼+流水號，代碼：C：註冊、S：取車E：延長用車，F：還車，T：ETAG補繳，B：綁定，O：修改綁定', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'MerchantTradeNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = 'OrderNo';


GO

CREATE INDEX [IX_TB_Trade_SearchForUpd] ON [dbo].[TB_Trade] ([OrderNo], [MerchantTradeNo])
