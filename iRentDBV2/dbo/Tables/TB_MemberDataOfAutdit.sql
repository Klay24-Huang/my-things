CREATE TABLE [dbo].[TB_MemberDataOfAutdit] (
    [AuditID]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [MEMIDNO]     VARCHAR (10)   CONSTRAINT [DF__tmp_ms_xx__MEMID__563FA78C] DEFAULT ('') NOT NULL,
    [MEMCNAME]    NVARCHAR (10)  CONSTRAINT [DF__tmp_ms_xx__MEMCN__5733CBC5] DEFAULT (N'') NOT NULL,
    [MEMTEL]      VARCHAR (20)   CONSTRAINT [DF__tmp_ms_xx__MEMTE__5827EFFE] DEFAULT ('') NOT NULL,
    [MEMHTEL]     VARCHAR (20)   CONSTRAINT [DF_TB_MemberDataOfAutdit_MEMHTEL] DEFAULT ('') NOT NULL,
    [MEMBIRTH]    DATETIME       NULL,
    [MEMCOUNTRY]  INT            CONSTRAINT [DF__tmp_ms_xx__MEMCO__591C1437] DEFAULT ((0)) NOT NULL,
    [MEMCITY]     INT            CONSTRAINT [DF__tmp_ms_xx__MEMCI__5A103870] DEFAULT ((0)) NOT NULL,
    [MEMADDR]     NVARCHAR (500) CONSTRAINT [DF__tmp_ms_xx__MEMAD__5B045CA9] DEFAULT (N'') NOT NULL,
    [MEMEMAIL]    VARCHAR (200)  CONSTRAINT [DF__tmp_ms_xx__MEMEM__5BF880E2] DEFAULT (N'') NOT NULL,
    [MEMCOMTEL]   VARCHAR (20)   CONSTRAINT [DF__tmp_ms_xx__MEMCO__5CECA51B] DEFAULT ('') NOT NULL,
    [MEMCONTRACT] NVARCHAR (10)  CONSTRAINT [DF__tmp_ms_xx__MEMCO__5DE0C954] DEFAULT (N'') NOT NULL,
    [MEMCONTEL]   VARCHAR (20)   CONSTRAINT [DF__tmp_ms_xx__MEMCO__5ED4ED8D] DEFAULT ('') NOT NULL,
    [MEMMSG]      VARCHAR (1)    CONSTRAINT [DF__tmp_ms_xx__MEMMS__5FC911C6] DEFAULT ('Y') NOT NULL,
    [CARDNO]      VARCHAR (20)   CONSTRAINT [DF__tmp_ms_xx__CARDN__60BD35FF] DEFAULT ('') NOT NULL,
    [UNIMNO]      VARCHAR (10)   CONSTRAINT [DF__tmp_ms_xx__UNIMN__61B15A38] DEFAULT ('') NOT NULL,
    [MEMSENDCD]   TINYINT        CONSTRAINT [DF__tmp_ms_xx__MEMSE__62A57E71] DEFAULT ((2)) NOT NULL,
    [CARRIERID]   VARCHAR (20)   CONSTRAINT [DF__tmp_ms_xx__CARRI__6399A2AA] DEFAULT ('') NOT NULL,
    [NPOBAN]      VARCHAR (20)   CONSTRAINT [DF__tmp_ms_xx__NPOBA__648DC6E3] DEFAULT ('') NOT NULL,
    [AuditKind]   TINYINT        CONSTRAINT [DF__tmp_ms_xx__Audit__6581EB1C] DEFAULT ((0)) NOT NULL,
    [HasAudit]    TINYINT        CONSTRAINT [DF__tmp_ms_xx__HasAu__66760F55] DEFAULT ((0)) NOT NULL,
    [IsNew]       TINYINT        CONSTRAINT [DF__tmp_ms_xx__IsNew__676A338E] DEFAULT ((0)) NOT NULL,
    [SPECSTATUS]  VARCHAR (2)    CONSTRAINT [DF_TB_MemberDataOfAutdit_SPECSTATUS] DEFAULT ('00') NOT NULL,
    [SPSD]        VARCHAR (8)    CONSTRAINT [DF_TB_MemberDataOfAutdit_SPSD] DEFAULT ('') NOT NULL,
    [SPED]        VARCHAR (8)    CONSTRAINT [DF_TB_MemberDataOfAutdit_SPED] DEFAULT ('') NOT NULL,
    [MKTime]      DATETIME       CONSTRAINT [DF__tmp_ms_xx__MKTim__685E57C7] DEFAULT (dateadd(hour,(8),getdate())) NOT NULL,
    [UPDTime]     DATETIME       NULL,
    CONSTRAINT [PK_TB_MemberDataOfAutdit] PRIMARY KEY CLUSTERED ([AuditID] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'待審核會員資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit';
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'愛心碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'NPOBAN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手機條碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'CARRIERID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMSENDCD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'統編', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'UNIMNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'卡片內碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'CARDNO';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'活動及優惠訊息通知(Y:是 N:否)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMMSG';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'緊急連絡人電話', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMCONTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'緊急連絡人', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMCONTRACT';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'公司電話', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMCOMTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'電子郵件信箱', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMEMAIL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'地址', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMADDR';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'縣市', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMCITY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'國家', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMCOUNTRY';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'生日', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMBIRTH';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聯絡電話(手機)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMTEL';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'聯絡電話(住家)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMHTEL';


GO

EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMCNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號(身份證)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMIDNO';

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'有無審核過（0:否;1:有(未通過);2:有(通過))',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberDataOfAutdit',
    @level2type = N'COLUMN',
    @level2name = N'HasAudit'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'送審方式（0:修改會員資料;1:修改證件照;2:兩者皆有)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberDataOfAutdit',
    @level2type = N'COLUMN',
    @level2name = N'AuditKind'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否為新加入(0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberDataOfAutdit',
    @level2type = N'COLUMN',
    @level2name = N'IsNew'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'上次審核時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_MemberDataOfAutdit',
    @level2type = N'COLUMN',
    @level2name = N'UPDTime'
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'特殊身份有效日期(起)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'SPSD';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'特殊身份有效日期(迄)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'SPED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'特殊身份說明', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'SPECSTATUS';

