CREATE TABLE [dbo].[TB_TogetherPassenger] (
    [Order_number] BIGINT        NOT NULL,
    [MEMIDNO]      VARCHAR (20)  NOT NULL,
    [APPUSEID]     VARCHAR (20)  NOT NULL,
    [MEMCNAME]     NVARCHAR (60) NOT NULL,
    [MEMTEL]       VARCHAR (20)  NOT NULL,
    [ChkType]      VARCHAR (1)   NOT NULL,
    [MKTime]       DATETIME      NOT NULL,
    [UPTime]       DATETIME      NULL
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TogetherPassenger', @level2type = N'COLUMN', @level2name = N'UPTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TogetherPassenger', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'邀請狀態; Y = 已接受，N = 已拒絕，F = 已取消，S = 邀請中', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TogetherPassenger', @level2type = N'COLUMN', @level2name = N'ChkType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手機號碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TogetherPassenger', @level2type = N'COLUMN', @level2name = N'MEMTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TogetherPassenger', @level2type = N'COLUMN', @level2name = N'MEMCNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'APP畫面顯示專用ID;身分證or手機', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TogetherPassenger', @level2type = N'COLUMN', @level2name = N'APPUSEID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'身分證', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TogetherPassenger', @level2type = N'COLUMN', @level2name = N'MEMIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_TogetherPassenger', @level2type = N'COLUMN', @level2name = N'Order_number';

