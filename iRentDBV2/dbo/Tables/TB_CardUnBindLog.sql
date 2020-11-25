CREATE TABLE [dbo].[TB_CardUnBindLog] (
    [UnBindCardLogId] BIGINT        IDENTITY (1, 1) NOT NULL,
    [OrderNo]         VARCHAR (50)  CONSTRAINT [DF_Table_1_CarNo] DEFAULT ('') NOT NULL,
    [IDNO]            VARCHAR (20)  CONSTRAINT [DF_Table_1_CID] DEFAULT ('') NOT NULL,
    [CardToken]       VARCHAR (100) CONSTRAINT [DF_Table_1_CardNo] DEFAULT ('') NOT NULL,
    [IsSuccess]       TINYINT       CONSTRAINT [DF_TB_CardUnBindLog_IsSuccess] DEFAULT ((0)) NOT NULL,
    [MKTime]          DATETIME      CONSTRAINT [DF_TB_CardUnBindLog_MKTime] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [UPDTime]         DATETIME      NOT NULL,
    CONSTRAINT [PK_TB_CardUnBindLog] PRIMARY KEY CLUSTERED ([UnBindCardLogId] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最近一次更新時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CardUnBindLog', @level2type = N'COLUMN', @level2name = N'UPDTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'新增時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CardUnBindLog', @level2type = N'COLUMN', @level2name = N'MKTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否成功解綁', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CardUnBindLog', @level2type = N'COLUMN', @level2name = N'IsSuccess';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'信用卡密鑰', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CardUnBindLog', @level2type = N'COLUMN', @level2name = N'CardToken';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'身分證號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CardUnBindLog', @level2type = N'COLUMN', @level2name = N'IDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'解綁訂單序號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CardUnBindLog', @level2type = N'COLUMN', @level2name = N'OrderNo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'解除綁定序號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_CardUnBindLog', @level2type = N'COLUMN', @level2name = N'UnBindCardLogId';

