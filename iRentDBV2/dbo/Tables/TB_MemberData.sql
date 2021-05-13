CREATE TABLE [dbo].[TB_MemberData] (
    [A_PRGID]        INT             NULL,
    [A_USERID]       VARCHAR (10)    NULL,
    [A_SYSDT]        DATETIME        CONSTRAINT [DF__tmp_ms_xx__A_SYS__2665ABE1] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [U_PRGID]        INT             NULL,
    [U_USERID]       VARCHAR (10)    NULL,
    [U_SYSDT]        DATETIME        CONSTRAINT [DF__tmp_ms_xx__U_SYS__2759D01A] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [MEMIDNO]        VARCHAR (10)    CONSTRAINT [DF__tmp_ms_xx__MEMID__284DF453] DEFAULT ('') NOT NULL,
    [MEMCNAME]       NVARCHAR (10)   CONSTRAINT [DF__tmp_ms_xx__MEMCN__2942188C] DEFAULT (N'') NOT NULL,
    [MEMPWD]         VARCHAR (100)   CONSTRAINT [DF__tmp_ms_xx__MEMPW__2A363CC5] DEFAULT ('') NOT NULL,
    [MEMTEL]         VARCHAR (20)    CONSTRAINT [DF__tmp_ms_xx__MEMTE__2B2A60FE] DEFAULT ('') NOT NULL,
    [MEMHTEL]        VARCHAR (20)    CONSTRAINT [DF_TB_MemberData_MEMHTEL] DEFAULT ('') NOT NULL,
    [MEMBIRTH]       DATETIME        NULL,
    [MEMCOUNTRY]     INT             CONSTRAINT [DF__tmp_ms_xx__MEMCO__2C1E8537] DEFAULT ((0)) NOT NULL,
    [MEMCITY]        INT             CONSTRAINT [DF__tmp_ms_xx__MEMCI__2D12A970] DEFAULT ((0)) NOT NULL,
    [MEMADDR]        NVARCHAR (500)  CONSTRAINT [DF__tmp_ms_xx__MEMAD__2E06CDA9] DEFAULT (N'') NOT NULL,
    [MEMEMAIL]       VARCHAR (200)   CONSTRAINT [DF__tmp_ms_xx__MEMEM__2EFAF1E2] DEFAULT (N'') NOT NULL,
    [MEMCOMTEL]      VARCHAR (20)    CONSTRAINT [DF_TB_MemberData_MEMCOMTEL] DEFAULT ('') NOT NULL,
    [MEMCONTRACT]    NVARCHAR (10)   CONSTRAINT [DF_TB_MemberData_MEMCONTRACT] DEFAULT (N'') NOT NULL,
    [MEMCONTEL]      VARCHAR (20)    CONSTRAINT [DF_TB_MemberData_MEMCONTEL] DEFAULT ('') NOT NULL,
    [MEMMSG]         VARCHAR (1)     CONSTRAINT [DF_TB_MemberData_MEMMSG] DEFAULT ('Y') NOT NULL,
    [CARDNO]         VARCHAR (20)    CONSTRAINT [DF__tmp_ms_xx__CARDN__2FEF161B] DEFAULT ('') NOT NULL,
    [UNIMNO]         VARCHAR (10)    CONSTRAINT [DF__tmp_ms_xx__UNIMN__30E33A54] DEFAULT ('') NOT NULL,
    [MEMSENDCD]      TINYINT         CONSTRAINT [DF__tmp_ms_xx__MEMSE__31D75E8D] DEFAULT ((2)) NOT NULL,
    [CARRIERID]      VARCHAR (20)    CONSTRAINT [DF__tmp_ms_xx__CARRI__32CB82C6] DEFAULT ('') NOT NULL,
    [NPOBAN]         VARCHAR (20)    CONSTRAINT [DF__tmp_ms_xx__NPOBA__33BFA6FF] DEFAULT ('') NOT NULL,
    [HasVaildEMail]  TINYINT         CONSTRAINT [DF__tmp_ms_xx__HasVa__34B3CB38] DEFAULT ((0)) NOT NULL,
    [HasCheckMobile] TINYINT         CONSTRAINT [DF__tmp_ms_xx__HasCh__35A7EF71] DEFAULT ((0)) NOT NULL,
    [NeedChangePWD]  TINYINT         CONSTRAINT [DF__tmp_ms_xx__NeedC__369C13AA] DEFAULT ((0)) NOT NULL,
    [HasBindSocial]  TINYINT         CONSTRAINT [DF__tmp_ms_xx__HasBi__379037E3] DEFAULT ((0)) NOT NULL,
    [Audit]          INT             CONSTRAINT [DF__tmp_ms_xx__Audit__38845C1C] DEFAULT ((0)) NOT NULL,
    [AuditMessage]   NVARCHAR (1024) CONSTRAINT [DF__tmp_ms_xx__Audit__39788055] DEFAULT (N'') NOT NULL,
    [IrFlag]         INT             CONSTRAINT [DF__tmp_ms_xx__IrFla__3A6CA48E] DEFAULT ((-1)) NOT NULL,
    [PayMode]        TINYINT         CONSTRAINT [DF__tmp_ms_xx__PayMo__3B60C8C7] DEFAULT ((0)) NOT NULL,
    [RentType]       TINYINT         CONSTRAINT [DF__tmp_ms_xx__RentT__3C54ED00] DEFAULT ((0)) NOT NULL,
    [SPECSTATUS]     VARCHAR (2)     CONSTRAINT [DF_TB_MemberData_SPECSTATUS] DEFAULT ('00') NOT NULL,
    [SPSD]           VARCHAR (8)     CONSTRAINT [DF_TB_MemberData_SPSD] DEFAULT ('') NOT NULL,
    [SPED]           VARCHAR (8)     CONSTRAINT [DF_TB_MemberData_SPED] DEFAULT ('') NOT NULL,
    [PushREGID]      BIGINT          CONSTRAINT [DF_TB_MemberData_PushREGID] DEFAULT ((0)) NOT NULL,
    [MEMRFNBR] INT NOT NULL DEFAULT ((0)), 
    [MEMONEW2] NVARCHAR(50) NOT NULL DEFAULT (''), 
    [MEMUPDT] [datetime] NULL,
	[APPLYDT] [datetime] NULL,
    CONSTRAINT [PK_TB_MemberData] PRIMARY KEY CLUSTERED ([MEMIDNO] ASC)
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TB_MemberData_CheckMobile]
    ON [dbo].[TB_MemberData]([HasCheckMobile] ASC, [HasVaildEMail] ASC, [MEMIDNO] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'可租車類別：0:無法;1:汽車;2:機車;3:全部', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'RentType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付費方式：0:信用卡;1:和雲錢包;2:line pay;3:街口支付', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'PayMode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目前註冊進行至哪個步驟', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'IrFlag';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'審核不通過原因', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'AuditMessage';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否通過審核(0:未審;1:已審;2:審核不通過)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'Audit';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否綁定社群(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'HasBindSocial';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否需重新設定密碼(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'NeedChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否通過手機驗證(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'HasCheckMobile';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否有驗證email;0:否;1:是', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'HasVaildEMail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'愛心碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'NPOBAN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手機條碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'CARRIERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMSENDCD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'統編', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'UNIMNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'卡片內碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'CARDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'活動及優惠訊息通知(Y:是 N:否)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMMSG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'緊急連絡人電話', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMCONTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'緊急連絡人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMCONTRACT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公司電話', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMCOMTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'電子郵件信箱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMEMAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMADDR';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'縣市', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMCITY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'國家', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMCOUNTRY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'生日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMBIRTH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聯絡電話(手機)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMPWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMCNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號(身份證)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMIDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'U_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'修改者帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'U_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'由哪個程式修改（對應TB_APILIST PK)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'U_PRGID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'新增時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'A_SYSDT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'新增者帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'A_USERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'由哪個程式新增，對應TB_APILIST PK', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'A_PRGID';

GO


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員資料表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'連絡電話(住家)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'MEMHTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'特殊身份有效日期(起)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'SPSD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'特殊身份有效日期(迄)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'SPED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'特殊身份說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'SPECSTATUS';


GO




EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'推播註冊ID',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberData',
    @level2type = N'COLUMN',
    @level2name = N'PushREGID'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'短租easyrent會員流水號',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberData',
    @level2type = N'COLUMN',
    @level2name = N'MEMRFNBR'