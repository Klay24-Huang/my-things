CREATE TABLE [dbo].[TB_OrderAuthReservation] (
    [A_PRGID]         VARCHAR (20)   CONSTRAINT [DF_TB_OrderAuthReservation_A_PRGID] DEFAULT ('') NOT NULL,
    [A_USERID]        VARCHAR (20)   CONSTRAINT [DF_TB_OrderAuthReservation_A_USERID] DEFAULT ('') NOT NULL,
    [A_SYSDT]         DATETIME       CONSTRAINT [DF_TB_OrderAuthReservation_A_SYSDT] DEFAULT (getdate()) NOT NULL,
    [U_PRGID]         VARCHAR (20)   CONSTRAINT [DF_TB_OrderAuthReservation_U_PRGID] DEFAULT ('') NOT NULL,
    [U_USERID]        VARCHAR (20)   CONSTRAINT [DF_TB_OrderAuthReservation_U_USERID] DEFAULT ('') NOT NULL,
    [U_SYSDT]         DATETIME       CONSTRAINT [DF_TB_OrderAuthReservation_U_SYSDT] DEFAULT (getdate()) NOT NULL,
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

