CREATE TABLE [dbo].[TB_Trade] (
    [TradeID]          BIGINT         IDENTITY (2790000, 1) NOT NULL,
    [OrderNo]          BIGINT         CONSTRAINT [DF__tmp_ms_xx__Order__04115F34] DEFAULT ((0)) NOT NULL,
    [MerchantTradeNo]  VARCHAR (30)   CONSTRAINT [DF__tmp_ms_xx__Merch__0505836D] DEFAULT ('') NOT NULL,
    [CreditType]       TINYINT        CONSTRAINT [DF__tmp_ms_xx__Credi__05F9A7A6] DEFAULT ((0)) NOT NULL,
    [MerchantMemberID] VARCHAR (10)   CONSTRAINT [DF__tmp_ms_xx__Merch__06EDCBDF] DEFAULT ('') NOT NULL,
    [RetCode]          VARCHAR (10)   CONSTRAINT [DF__tmp_ms_xx__RetCo__07E1F018] DEFAULT ((-1)) NOT NULL,
    [RetMsg]           NVARCHAR (400) CONSTRAINT [DF__tmp_ms_xx__RetMs__08D61451] DEFAULT (N'') NOT NULL,
    [TaishinTradeNo]   VARCHAR (50)   CONSTRAINT [DF__tmp_ms_xx__Taish__09CA388A] DEFAULT ('') NOT NULL,
    [CardNumber]       VARCHAR (20)   CONSTRAINT [DF__tmp_ms_xx__CardN__0ABE5CC3] DEFAULT ('') NOT NULL,
    [CardToken]        VARCHAR (128)  CONSTRAINT [DF__tmp_ms_xx__CardT__0BB280FC] DEFAULT ('') NOT NULL,
    [process_date]     DATETIME       NULL,
    [AUTHAMT]          INT            CONSTRAINT [DF__tmp_ms_xx__AUTHA__0CA6A535] DEFAULT ((0)) NOT NULL,
    [amount]           INT            NOT NULL,
    [AuthIdResp]       VARCHAR (50)   CONSTRAINT [DF__tmp_ms_xx__AuthI__0D9AC96E] DEFAULT ((-1)) NOT NULL,
    [IsSuccess]        INT            CONSTRAINT [DF__tmp_ms_xx__IsSuc__0E8EEDA7] DEFAULT ((0)) NOT NULL,
    [MKTime]           DATETIME       CONSTRAINT [DF__tmp_ms_xx__MKTim__0F8311E0] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [UPDTime]          DATETIME       NULL,
    [AutoClose]        INT            CONSTRAINT [DF_TB_Trade_AutoClose] DEFAULT ((0)) NOT NULL,
    [AuthType]         INT            CONSTRAINT [DF_TB_Trade_AuthType] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_TB_Trade] PRIMARY KEY CLUSTERED ([TradeID] ASC)
);




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
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否成功 0:未送出;1:成功;-1:失敗;-2:連線失敗', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'IsSuccess';




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
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易類型 (0:租金; 1:etag補繳(無用); 2:補繳(無用); 3:直接取款; 4:訂閱制; 5:訂閱制;6:定金;7:錢包儲值; 99:訂閱制)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'CreditType';




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'送出的交易序號，對應CreditType，規則：HXXXXX+代碼+流水號，代碼：C：註冊、S：取車E：延長用車，F：還車，T：ETAG補繳，B：綁定，O：修改綁定，D：定金，P：預授權，W：錢包儲值', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'MerchantTradeNo';




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原始訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = 'OrderNo';


GO

CREATE INDEX [IX_TB_Trade_SearchForUpd] ON [dbo].[TB_Trade] ([OrderNo], [MerchantTradeNo])

GO
CREATE NONCLUSTERED INDEX [IX_SearchTaishinTradeNoByOrderModify2]
    ON [dbo].[TB_Trade]([OrderNo] ASC, [IsSuccess] ASC)
    INCLUDE([TaishinTradeNo]);


GO
CREATE NONCLUSTERED INDEX [IX_SearchTaishinTradeNoByOrderModify]
    ON [dbo].[TB_Trade]([IsSuccess] ASC, [TaishinTradeNo] ASC)
    INCLUDE([OrderNo]);


GO
CREATE NONCLUSTERED INDEX [IX_MissIndex_TB_Trade_20210329]
    ON [dbo].[TB_Trade]([CreditType] ASC, [IsSuccess] ASC)
    INCLUDE([OrderNo], [TaishinTradeNo], [CardNumber], [AUTHAMT], [AuthIdResp]);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自動關帳 1:自動關 0:手動關', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'AutoClose';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預授權授權類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_Trade', @level2type = N'COLUMN', @level2name = N'AuthType';

