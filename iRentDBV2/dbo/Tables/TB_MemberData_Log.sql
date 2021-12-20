/****** Object:  Table [dbo].[TB_MemberData_Log]    Script Date: 2021/3/23 上午 09:47:01 ******/
CREATE TABLE [dbo].[TB_MemberData_Log] (
    [PROCD]          VARCHAR (5)     CONSTRAINT [DF__TB_Member__PROCD__3CF5A22C] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [UPD_PRG]        VARCHAR (10)    NOT NULL,
    [UPD_TIME]       DATETIME        NOT NULL,
    [A_PRGID]        INT             NULL,
    [A_USERID]       VARCHAR (10)    NULL,
    [A_SYSDT]        DATETIME        CONSTRAINT [DF__TB_Member__A_SYS__3DE9C665] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [U_PRGID]        INT             NULL,
    [U_USERID]       VARCHAR (10)    NULL,
    [U_SYSDT]        DATETIME        CONSTRAINT [DF__TB_Member__U_SYS__3EDDEA9E] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [MEMIDNO]        VARCHAR (10)    CONSTRAINT [DF__TB_Member__MEMID__3FD20ED7] DEFAULT ('') NOT NULL,
    [MEMCNAME]       NVARCHAR (60)   CONSTRAINT [DF__TB_Member__MEMCN__40C63310] DEFAULT (N'') NOT NULL,
    [MEMPWD]         VARCHAR (100)   CONSTRAINT [DF__TB_Member__MEMPW__41BA5749] DEFAULT ('') NOT NULL,
    [MEMTEL]         VARCHAR (20)    CONSTRAINT [DF__TB_Member__MEMTE__42AE7B82] DEFAULT ('') NOT NULL,
    [MEMHTEL]        VARCHAR (20)    CONSTRAINT [DF__TB_Member__MEMHT__43A29FBB] DEFAULT ('') NOT NULL,
    [MEMBIRTH]       DATETIME        NULL,
    [MEMCOUNTRY]     INT             CONSTRAINT [DF__TB_Member__MEMCO__4496C3F4] DEFAULT ((0)) NOT NULL,
    [MEMCITY]        INT             CONSTRAINT [DF__TB_Member__MEMCI__458AE82D] DEFAULT ((0)) NOT NULL,
    [MEMADDR]        NVARCHAR (500)  CONSTRAINT [DF__TB_Member__MEMAD__467F0C66] DEFAULT (N'') NOT NULL,
    [MEMEMAIL]       VARCHAR (200)   CONSTRAINT [DF__TB_Member__MEMEM__4773309F] DEFAULT (N'') NOT NULL,
    [MEMCOMTEL]      VARCHAR (20)    CONSTRAINT [DF__TB_Member__MEMCO__486754D8] DEFAULT ('') NOT NULL,
    [MEMCONTRACT]    NVARCHAR (10)   CONSTRAINT [DF__TB_Member__MEMCO__495B7911] DEFAULT (N'') NOT NULL,
    [MEMCONTEL]      VARCHAR (20)    CONSTRAINT [DF__TB_Member__MEMCO__4A4F9D4A] DEFAULT ('') NOT NULL,
    [MEMMSG]         VARCHAR (1)     CONSTRAINT [DF__TB_Member__MEMMS__4B43C183] DEFAULT ('Y') NOT NULL,
    [CARDNO]         VARCHAR (30)    CONSTRAINT [DF__TB_Member__CARDN__4C37E5BC] DEFAULT ('') NOT NULL,
    [UNIMNO]         VARCHAR (10)    CONSTRAINT [DF__TB_Member__UNIMN__4D2C09F5] DEFAULT ('') NOT NULL,
    [MEMSENDCD]      TINYINT         CONSTRAINT [DF__TB_Member__MEMSE__4E202E2E] DEFAULT ((2)) NOT NULL,
    [CARRIERID]      VARCHAR (20)    CONSTRAINT [DF__TB_Member__CARRI__4F145267] DEFAULT ('') NOT NULL,
    [NPOBAN]         VARCHAR (20)    CONSTRAINT [DF__TB_Member__NPOBA__500876A0] DEFAULT ('') NOT NULL,
    [HasVaildEMail]  TINYINT         CONSTRAINT [DF__TB_Member__HasVa__50FC9AD9] DEFAULT ((0)) NOT NULL,
    [HasCheckMobile] TINYINT         CONSTRAINT [DF__TB_Member__HasCh__51F0BF12] DEFAULT ((0)) NOT NULL,
    [NeedChangePWD]  TINYINT         CONSTRAINT [DF__TB_Member__NeedC__52E4E34B] DEFAULT ((0)) NOT NULL,
    [HasBindSocial]  TINYINT         CONSTRAINT [DF__TB_Member__HasBi__53D90784] DEFAULT ((0)) NOT NULL,
    [Audit]          INT             CONSTRAINT [DF__TB_Member__Audit__54CD2BBD] DEFAULT ((0)) NOT NULL,
    [AuditMessage]   NVARCHAR (1024) CONSTRAINT [DF__TB_Member__Audit__55C14FF6] DEFAULT (N'') NOT NULL,
    [IrFlag]         INT             CONSTRAINT [DF__TB_Member__IrFla__56B5742F] DEFAULT ((-1)) NOT NULL,
    [PayMode]        TINYINT         CONSTRAINT [DF__TB_Member__PayMo__57A99868] DEFAULT ((0)) NOT NULL,
    [RentType]       TINYINT         CONSTRAINT [DF__TB_Member__RentT__589DBCA1] DEFAULT ((0)) NOT NULL,
    [SPECSTATUS]     VARCHAR (2)     CONSTRAINT [DF__TB_Member__SPECS__5991E0DA] DEFAULT ('00') NOT NULL,
    [SPSD]           VARCHAR (8)     CONSTRAINT [DF__TB_MemberD__SPSD__5A860513] DEFAULT ('') NOT NULL,
    [SPED]           VARCHAR (8)     CONSTRAINT [DF__TB_MemberD__SPED__5B7A294C] DEFAULT ('') NOT NULL,
    [PushREGID]      BIGINT          CONSTRAINT [DF__TB_Member__PushR__5C6E4D85] DEFAULT ((0)) NOT NULL,
    [MEMRFNBR]       INT             CONSTRAINT [DF__TB_Member__MEMRF__5D6271BE] DEFAULT ((0)) NOT NULL,
    [MEMONEW2]       NVARCHAR (50)   CONSTRAINT [DF__TB_Member__MEMON__5E5695F7] DEFAULT ('') NOT NULL,
    [MEMUPDT]        DATETIME        NULL,
    [APPLYDT]        DATETIME        NULL,
    [AutoStored]     INT             CONSTRAINT [DF_TB_MemberData_Log_AutoStored] DEFAULT ((0)) NULL,
    [isCancel]       TINYINT         CONSTRAINT [DF_TB_MemberData_Log_isCancel] DEFAULT ((0)) NOT NULL
);


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO


GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'會員資料表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_MemberData_Log'
GO
CREATE NONCLUSTERED INDEX [a2]
    ON [dbo].[TB_MemberData_Log]([Audit] ASC);


GO
CREATE NONCLUSTERED INDEX [a1]
    ON [dbo].[TB_MemberData_Log]([MEMRFNBR] ASC);


GO
CREATE NONCLUSTERED INDEX [a]
    ON [dbo].[TB_MemberData_Log]([MEMIDNO] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'異動時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'UPD_TIME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'異動來源(APIID)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'UPD_PRG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'統編', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'UNIMNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'U_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'U_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'由哪個程式修改（對應TB_APILIST PK)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'U_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'特殊身份有效日期(起)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'SPSD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'特殊身份有效日期(迄)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'SPED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'特殊身份說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'SPECSTATUS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'可租車類別：0:無法;1:汽車;2:機車;3:全部', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'RentType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'推播註冊流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'PushREGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'異動來源 (A:新增 U:修改)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'PROCD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付費方式：0:信用卡;1:和雲錢包;2:line pay;3:街口支付', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'PayMode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'愛心碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'NPOBAN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否需重新設定密碼(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'NeedChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聯絡電話(手機)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMSENDCD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'短租會員流水號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMRFNBR';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMPWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'活動及優惠訊息通知(Y:是 N:否)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMMSG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號(身份證)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'連絡電話(住家)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMHTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'電子郵件信箱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMEMAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'國家', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMCOUNTRY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'緊急連絡人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMCONTRACT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'緊急連絡人電話(手機)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMCONTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公司電話', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMCOMTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMCNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'縣市', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMCITY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'生日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMBIRTH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMADDR';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目前註冊進行至哪個步驟：-1驗證完手機 、0：設置密碼、1：其他', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'IrFlag';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否有驗證email;0:否;1:是', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'HasVaildEMail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否通過手機驗證(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'HasCheckMobile';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否綁定社群(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'HasBindSocial';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手機條碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'CARRIERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'卡片內碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'CARDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'審核不通過原因', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'AuditMessage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否通過審核(0:未審;1:已審;2:審核不通過)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'Audit';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'新增者帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'A_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'新增時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'A_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'由哪個程式新增，對應TB_APILIST PK', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'A_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員最新修改日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'MEMUPDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'解除會員註記 0:不解除 ; 1:解除', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'isCancel';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否同意自動儲值(0:不同意，1:同意)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'AutoStored';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = '會員申請日期', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData_Log', @level2type = N'COLUMN', @level2name = N'APPLYDT';

