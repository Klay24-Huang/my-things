CREATE TABLE [dbo].[TB_MemberDataOfAutdit]
(
	[AuditID] BIGINT NOT NULL IDENTITY ,
	[MEMIDNO]        VARCHAR (10)    DEFAULT ('') NOT NULL,
    [MEMCNAME]       NVARCHAR (10)   DEFAULT (N'') NOT NULL,
    [MEMTEL]         VARCHAR (20)    DEFAULT ('') NOT NULL,
    [MEMBIRTH]       DATETIME        NULL,
    [MEMCOUNTRY]     INT             DEFAULT ((0)) NOT NULL,
    [MEMCITY]        INT             DEFAULT ((0)) NOT NULL,
    [MEMADDR]        NVARCHAR (500)  DEFAULT (N'') NOT NULL,
    [MEMEMAIL]       VARCHAR (200)   DEFAULT (N'') NOT NULL,
    [MEMCOMTEL] 	 varchar (20) 	 DEFAULT ('')  NOT NULL,
	[MEMCONTRACT] 	 nvarchar (10)   DEFAULT (N'') NOT NULL,
	[MEMCONTEL] 	 varchar (20)    DEFAULT ('')  NOT NULL,
	[MEMMSG] 		 varchar(1)      DEFAULT ('Y') NOT NULL,
    [CARDNO]         VARCHAR (20)    DEFAULT ('') NOT NULL,
    [UNIMNO]         VARCHAR (10)    DEFAULT ('') NOT NULL,
    [MEMSENDCD]      TINYINT         DEFAULT ((2)) NOT NULL,
    [CARRIERID]      VARCHAR (20)    DEFAULT ('') NOT NULL,
    [NPOBAN]         VARCHAR (20)    DEFAULT ('') NOT NULL, 
    [MKTime] DATETIME  NOT NULL DEFAULT(dateadd(hour,(8),getdate())) NOT NULL,
    CONSTRAINT [PK_TB_MemberDataOfAutdit] PRIMARY KEY ([AuditID])

)
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
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'姓名', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMCNAME';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號(身份證)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberDataOfAutdit', @level2type = N'COLUMN', @level2name = N'MEMIDNO';
