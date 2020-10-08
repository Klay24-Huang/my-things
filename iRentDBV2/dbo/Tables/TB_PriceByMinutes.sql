CREATE TABLE [dbo].[TB_PriceByMinutes]
(
	[PriceMinutesID]      INT          IDENTITY (1, 1) NOT NULL,
    [ProjID]              VARCHAR (10)  DEFAULT ('') NOT NULL,
    [BaseMinutes]         INT           DEFAULT ((0)) NOT NULL,
    [BaseMinutesPrice]    FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [BaseMinutesPriceH]   FLOAT (53)   DEFAULT ((0.0)) NOT NULL,
    [CarType]             VARCHAR (10)  DEFAULT ('') NOT NULL,
    [CarNo]               VARCHAR (10)  DEFAULT ('') NOT NULL,
    [Price]               FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [PriceH]              FLOAT (53)   DEFAULT ((0.0)) NOT NULL,
    [OverPrice]           FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [OverPriceH]          FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [MaxPrice]            INT           DEFAULT ((0)) NOT NULL,
    [MaxPriceH]           INT           DEFAULT ((0)) NOT NULL,
    [OverMaxPrice]        INT           DEFAULT (0) NOT NULL,
    [OverMaxPriceH]        INT           DEFAULT (0) NOT NULL,
  
    [MileagePrice]        FLOAT (53)    DEFAULT ((0.0)) NOT NULL,
    [AllowDiscount]       TINYINT       DEFAULT ((1)) NOT NULL,
    [PointerOfMinutes]    FLOAT (53)    DEFAULT ((1)) NOT NULL,
    [PointerOfMinutesH]   FLOAT (53)    DEFAULT ((1)) NOT NULL,
    [BaseDiscountMinutes] INT           DEFAULT ((0)) NOT NULL,
    [use_flag]            TINYINT       DEFAULT ((0)) NOT NULL,
    [MKTime]              DATETIME     DEFAULT (getdate()) NOT NULL,
    [UPTime]              DATETIME     NULL, 
    CONSTRAINT [PK_TB_PriceByMinutes] PRIMARY KEY ([PriceMinutesID]),
)
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後一次更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'UPTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否啟用(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'use_flag';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最低折抵（分鐘）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'BaseDiscountMinutes';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'每分鐘需多少點數折抵(假日)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'PointerOfMinutesH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'每分鐘需多少點數折抵', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'PointerOfMinutes';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否允許折抵(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'AllowDiscount';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'里程費（每公里）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'MileagePrice';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'假日上限金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'MaxPriceH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'平日上限金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'MaxPrice';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'逾時每分鐘假日價', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'OverPriceH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'逾時每分鐘平日價', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'OverPrice';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'過了基本分鐘數後每分鐘假日價', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'PriceH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'過了基本分鐘數後每分鐘平日價', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'Price';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'CarNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車型', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'CarType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'每分鐘假日價（基本分鐘數）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'BaseMinutesPriceH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'每分鐘價錢（基本分鐘數）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'BaseMinutesPrice';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'基本分鐘數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'BaseMinutes';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'專案代碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes', @level2type = N'COLUMN', @level2name = N'ProjID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'租金（以分計費）', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_PriceByMinutes';


GO

CREATE INDEX [IX_TB_PriceByMinutes_Search] ON [dbo].[TB_PriceByMinutes] ([ProjID], [CarType])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'逾時平日上限',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PriceByMinutes',
    @level2type = N'COLUMN',
    @level2name = N'OverMaxPrice'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'逾時假日上限',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_PriceByMinutes',
    @level2type = N'COLUMN',
    @level2name = N'OverMaxPriceH'