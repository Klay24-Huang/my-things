CREATE TABLE [dbo].[TB_OrderOtherFee]
(
	[OrderNo]           BIGINT         NOT NULL,
    [CarDispatch]       INT             DEFAULT ((0)) NOT NULL,
    [DispatchRemark]    NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [CleanFee]          INT             DEFAULT ((0)) NOT NULL,
    [CleanFeeRemark]    NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [DestroyFee]        INT             DEFAULT ((0)) NOT NULL,
    [DestroyFeeRemark]  NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [ParkingFee]        INT             DEFAULT ((0)) NOT NULL,
    [ParkingFeeRemark]  NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [DraggingFee]       INT             DEFAULT ((0)) NOT NULL,
    [DraggingFeeRemark] NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [OtherFee]          INT             DEFAULT ((0)) NOT NULL,
    [OtherFeeRemark]    NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [hasSync]           TINYINT         DEFAULT ((0)) NOT NULL,
    [AddUser]           NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [AddTime]           DATETIME        DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL, 
    [UpdateUser]        NVARCHAR (100)  DEFAULT (N'') NOT NULL,
    [UpdateTime]        DATETIME        DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL, 
    CONSTRAINT [PK_TB_OrderOtherFee] PRIMARY KEY CLUSTERED ([OrderNo] ASC)
)
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'UpdateTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'UpdateUser';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'AddTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'AddUser';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否有同步到短租(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'hasSync';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'其他費用說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'OtherFeeRemark';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'其他費用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'OtherFee';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'拖吊費說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'DraggingFeeRemark';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'拖吊費', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'DraggingFee';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'非配合停車費說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'ParkingFeeRemark';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'非配合停車費', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'ParkingFee';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'物品損壞/遣失備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'DestroyFeeRemark';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'物品損壞/遣失費', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'DestroyFee';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'清潔費備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'CleanFeeRemark';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'清潔費', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'CleanFee';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'調度備註', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'DispatchRemark';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'車輛調度費', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee', @level2type = N'COLUMN', @level2name = N'CarDispatch';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'待繳費用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderOtherFee';