CREATE TABLE [dbo].[TB_OrderAuthReservation] (
    [authSeq]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [order_number]    BIGINT         NOT NULL,
    [final_price]     INT            CONSTRAINT [DF_TB_OrderAuthReservation_final_price] DEFAULT ((0)) NOT NULL,
    [IDNO]            VARCHAR (10)   CONSTRAINT [DF_TB_OrderAuthReservation_IDNO] DEFAULT ('') NOT NULL,
    [AuthFlg]         INT            CONSTRAINT [DF_TB_OrderAuthReservation_AuthFlg] DEFAULT ((0)) NOT NULL,
    [AuthCode]        VARCHAR (50)   CONSTRAINT [DF_TB_OrderAuthReservation_AuthCode] DEFAULT ('') NOT NULL,
    [AuthMessage]     NVARCHAR (120) NOT NULL,
    [transaction_no]  VARCHAR (50)   CONSTRAINT [DF_TB_OrderAuthReservation_transaction_no] DEFAULT ('') NOT NULL,
    [CardType]        INT            CONSTRAINT [DF_TB_OrderAuthReservation_CardType] DEFAULT ((1)) NOT NULL,
    [AuthType]        INT            CONSTRAINT [DF_TB_OrderAuthReservation_AuthType] DEFAULT ((0)) NOT NULL,
    [GateNO]          INT            CONSTRAINT [DF_TB_OrderAuthReservation_GateNO] DEFAULT ((0)) NOT NULL,
    [isRetry]         INT            CONSTRAINT [DF_TB_OrderAuthReservation_isRetry] DEFAULT ((0)) NOT NULL,
    [AutoClose]       INT            CONSTRAINT [DF_TB_OrderAuthReservation_AutoClose] DEFAULT ((0)) NOT NULL,
    [AppointmentTime] DATETIME       NOT NULL,
    [A_PRGID]         VARCHAR (50)   CONSTRAINT [DF_TB_OrderAuthReservation_A_PRGID] DEFAULT ('') NOT NULL,
    [A_USERID]        VARCHAR (20)   CONSTRAINT [DF_TB_OrderAuthReservation_A_USERID] DEFAULT ('') NOT NULL,
    [A_SYSDT]         DATETIME       CONSTRAINT [DF_TB_OrderAuthReservation_A_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [U_PRGID]         VARCHAR (50)   CONSTRAINT [DF_TB_OrderAuthReservation_U_PRGID] DEFAULT ('') NOT NULL,
    [U_USERID]        VARCHAR (20)   CONSTRAINT [DF_TB_OrderAuthReservation_U_USERID] DEFAULT ('') NOT NULL,
    [U_SYSDT]         DATETIME       CONSTRAINT [DF_TB_OrderAuthReservation_U_SYSDT] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    CONSTRAINT [PK_TB_OrderAuthReservation] PRIMARY KEY CLUSTERED ([authSeq] ASC)
);




GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'自動關帳', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'AutoClose';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'處理次數', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'isRetry';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權處理閘道編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'GateNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權類別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'AuthType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用卡類別(0:和泰; 1:台新)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'CardType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'U_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'U_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'U_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'交易序號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'transaction_no';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'訂單編號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'order_number';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'身分證號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'IDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權金額', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'final_price';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'序號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'authSeq';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權訊息', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'AuthMessage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權狀態(0:未授權,1:已授權,-1:授權失敗;9:處理中)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'AuthFlg';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'授權代碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'AuthCode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'預約發送授權的時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'AppointmentTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄者', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'A_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'A_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'登錄程式代號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderAuthReservation', @level2type = N'COLUMN', @level2name = N'A_PRGID';

