CREATE TABLE [dbo].[TB_MemberData] (
    [A_PRGID]        INT            NULL,
    [A_USERID]       VARCHAR (10)   NULL,
    [A_SYSDT]        DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [U_PRGID]        INT            NULL,
    [U_USERID]       VARCHAR (10)   NULL,
    [U_SYSDT]        DATETIME       DEFAULT (DATEADD(HOUR,8,GETDATE())) NOT NULL,
    [MEMIDNO]        VARCHAR (10)   DEFAULT ('') NOT NULL,
	[MEMCNAME]       NVARCHAR (10)   DEFAULT (N'') NOT NULL,
    [MEMPWD]         VARCHAR (50)   DEFAULT ('') NOT NULL,
    [MEMTEL]         VARCHAR (20)   DEFAULT ('') NOT NULL,
    [MEMBIRTH]       DATETIME       NULL,
    [MEMCOUNTRY]     INT            DEFAULT ((0)) NOT NULL,
    [MEMCITY]        INT            DEFAULT ((0)) NOT NULL,
    [MEMADDR]        NVARCHAR (500) DEFAULT (N'') NOT NULL,
    [MEMEMAIL]       VARCHAR (200)  DEFAULT (N'') NOT NULL,
    [CARDNO]         VARCHAR (20)   DEFAULT ('') NOT NULL,
    [UNIMNO]         VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMSENDCD]      TINYINT        DEFAULT ((2)) NOT NULL,
    [CARRIERID]      VARCHAR (20)   DEFAULT ('') NOT NULL,
    [NPOBAN]         VARCHAR (20)   DEFAULT ('') NOT NULL,
	[HasVaildEMail]  TINYINT        DEFAULT ((0)) NOT NULL,
    [HasCheckMobile] TINYINT        DEFAULT ((0)) NOT NULL,
    [NeedChangePWD]  TINYINT        DEFAULT ((0)) NOT NULL,
    [HasBindSocial]  TINYINT        DEFAULT ((0)) NOT NULL,
	[Audit]          INT            DEFAULT ((0)) NOT NULL,
	[AuditMessage]   NVARCHAR(1024) DEFAULT (N'') NOT NULL,
    [IrFlag]         INT            DEFAULT ((-1)) NOT NULL,
    [PayMode]        TINYINT        DEFAULT ((0)) NOT NULL,
    [RentType] TINYINT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_TB_MemberData] PRIMARY KEY CLUSTERED ([MEMIDNO] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TB_MemberData_CheckMobile]
    ON [dbo].[TB_MemberData]([HasCheckMobile], [HasVaildEMail], [MEMIDNO]);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'付費方式：0:信用卡;1:和雲錢包;2:line pay;3:街口支付', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'PayMode';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'目前註冊進行至哪個步驟', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'IrFlag';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否綁定社群(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'HasBindSocial';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否需重新設定密碼(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'NeedChangePWD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否通過手機驗證(0:否;1:是)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData', @level2type = N'COLUMN', @level2name = N'HasCheckMobile';


GO



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
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'會員資料表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberData';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否有驗證email;0:否;1:是',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberData',
    @level2type = N'COLUMN',
    @level2name = N'HasVaildEMail'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'姓名',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberData',
    @level2type = N'COLUMN',
    @level2name = N'MEMCNAME'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否通過審核(0:未審;1:已審;2:審核不通過)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberData',
    @level2type = N'COLUMN',
    @level2name = N'Audit'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'可租車類別：0:無法;1:汽車;2:機車;3:全部',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberData',
    @level2type = N'COLUMN',
    @level2name = N'RentType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'審核不通過原因',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberData',
    @level2type = N'COLUMN',
    @level2name = N'AuditMessage'