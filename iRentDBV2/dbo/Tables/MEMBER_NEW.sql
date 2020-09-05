CREATE TABLE [dbo].[MEMBER_NEW] (
    [A_PRGID]       VARCHAR (10)    NULL,
    [A_USERID]      VARCHAR (10)    NULL,
    [A_SYSDT]       DATETIME        NULL,
    [U_PRGID]       VARCHAR (10)    NULL,
    [U_USERID]      VARCHAR (10)    NULL,
    [U_SYSDT]       DATETIME        NULL,
    [MEMRFNBR]      INT             IDENTITY (430947, 1) NOT NULL,
    [MEMCNAME]      NVARCHAR (50)   NULL,
    [MEMIDNO]       VARCHAR (10)    NULL,
    [MEMPWD]        VARCHAR (10)    NULL,
    [MEMSEX]        CHAR (1)        NULL,
    [MEMBIRTH]      DATETIME        NULL,
    [MEMMARRY]      CHAR (1)        NULL,
    [MEMTEL_ZIP]    VARCHAR (5)     NULL,
    [MEMTEL]        VARCHAR (20)    NULL,
    [MEMTEL_EXT]    VARCHAR (10)    NULL,
    [MEMCEIL]       VARCHAR (20)    NULL,
    [MEMCITY]       INT             NULL,
    [MEMCOUNTRY]    INT             NULL,
    [MEMADDR]       NVARCHAR (200)  NULL,
    [MEMEMAIL]      VARCHAR (100)   NULL,
    [MEMSELKEY_I]   INT             NULL,
    [MEMSELKEY_J]   INT             NULL,
    [MEMCOMPANY]    VARCHAR (40)    NULL,
    [MEMCOMTEL_ZIP] VARCHAR (5)     NULL,
    [MEMCOMTEL]     VARCHAR (20)    NULL,
    [MEMCOMTEL_EXT] VARCHAR (10)    NULL,
    [MEMCOMCITY]    INT             NULL,
    [MEMCOMCOUNTRY] INT             NULL,
    [MEMCOMADDR]    NVARCHAR (200)  NULL,
    [MEMCOMNONEED]  CHAR (1)        NULL,
    [MEMCOMTITLE]   VARCHAR (40)    NULL,
    [MEMCOMNO]      VARCHAR (10)    NULL,
    [MEMCONTRACT]   NVARCHAR (50)   NULL,
    [MEMSELKEY_K]   INT             NULL,
    [MEMCONTEL_ZIP] VARCHAR (5)     NULL,
    [MEMCONTEL]     VARCHAR (20)    NULL,
    [MEMCONTEL_EXT] VARCHAR (10)    NULL,
    [MEMEPAPER]     CHAR (1)        NULL,
    [MEMEBONUS]     FLOAT (53)      NULL,
    [MEMAPPLYDATE]  DATETIME        NULL,
    [MEMGROUP]      CHAR (1)        NULL,
    [MEMLNGCOUNT]   INT             NULL,
    [MEMLNGDATE]    DATETIME        NULL,
    [MEMMEMO]       NVARCHAR (1000) NULL,
    [MEMMEMO_1]     VARCHAR (5000)  NULL,
    [MEMSTARFNBR]   INT             NULL,
    [MEMSTAFF]      CHAR (2)        NULL,
    [MEMTAG]        CHAR (1)        NULL,
    [MEMCOUNT_P]    INT             NULL,
    [MEMCOUNT_S]    INT             NULL,
    [STADT]         DATETIME        NULL,
    [ENDDT]         DATETIME        NULL,
    [FAMILY]        INT             NULL,
    [INCOMEY]       INT             NULL,
    [RENTFOR]       INT             NULL,
    [SYSCD]         VARCHAR (6)     CONSTRAINT [DF_MEMBER_NEW_SYSCD] DEFAULT ('ER') NOT NULL,
    [MEMMSG]        VARCHAR (1)     CONSTRAINT [DF_MEMBER_NEW_MEMMSG] DEFAULT ('Y') NOT NULL,
    [MEMMSGMEMO]    NVARCHAR (200)  CONSTRAINT [DF_MEMBER_NEW_MEMMSGMEMO] DEFAULT ('') NOT NULL,
    [MEMCD]         CHAR (1)        NULL,
    [MEMPOST2]      VARCHAR (10)    NULL,
    [MEMCITY2]      VARCHAR (10)    NULL,
    [MEMCOUNTRY2]   VARCHAR (10)    NULL,
    [MEMADDR2]      NVARCHAR (100)  NULL,
    [MEMVERCD]      CHAR (1)        NULL,
    [WEBFLG]        VARCHAR (1)     NULL,
    [EVFLG]         VARCHAR (1)     NULL,
    [IRENTFLG]      VARCHAR (1)     NULL,
    [DRIVERNO]      VARCHAR (20)    NULL,
    [SIDENTITY]     VARCHAR (5)     NULL,
    [CARDCD]        VARCHAR (5)     NULL,
    [CARDRECVCD]    VARCHAR (5)     NULL,
    [CARDNO1]       VARCHAR (30)    NULL,
    [CARDNO2]       VARCHAR (30)    NULL,
    [CARDRECVDT]    DATETIME        NULL,
    [CARDSENDDT]    DATETIME        NULL,
    [ADDCD]         VARCHAR (5)     NULL,
    [APISTATUS]     VARCHAR (2)     NULL,
    [APISTATUS2]    VARCHAR (5)     NULL,
    [IRENTSTATUS]   VARCHAR (5)     NULL,
    [IRENTSENDCD]   VARCHAR (5)     NULL,
    [IRENTBRNHCD]   VARCHAR (10)    NULL,
    [BOOKLETSENDDT] DATETIME        NULL,
    [SIDENTITYSDT]  DATETIME        NULL,
    [SIDENTITYEDT]  DATETIME        NULL,
    [HTCID]         VARCHAR (150)   NULL,
    [HTTIME]        DATETIME        NULL,
    [QUICKFLG]      VARCHAR (1)     NULL,
    [MARKETING]     VARCHAR (1)     NULL,
    [MEMLICENCE01]  VARCHAR (1)     NULL,
    [MEMLICENCE02]  VARCHAR (1)     NULL,
    [MEMLICENCE03]  VARCHAR (1)     NULL,
    [MEMLICENCE04]  VARCHAR (1)     NULL,
    [MEMCONTEL2]    VARCHAR (20)    CONSTRAINT [DF_MEMBER_NEW_MEMCONTEL2] DEFAULT ('') NULL,
    [HIAPPENABLED]  VARCHAR (2)     CONSTRAINT [DF_MEMBER_NEW_HIAPPENABLED] DEFAULT ('N') NOT NULL,
    [HIAPPMSGCODE]  VARCHAR (10)    CONSTRAINT [DF_MEMBER_NEW_HIAPPMSGCODE] DEFAULT ('') NOT NULL,
    [HIAPPMSGSTS]   VARCHAR (2)     CONSTRAINT [DF_MEMBER_NEW_HIAPPMSGSTS] DEFAULT ('') NOT NULL,
    [HIAPPMAILCODE] VARCHAR (100)   CONSTRAINT [DF_MEMBER_NEW_HIAPPMAILCODE] DEFAULT ('') NOT NULL,
    [HIAPPMAILSTS]  VARCHAR (2)     CONSTRAINT [DF_MEMBER_NEW_HIAPPMAILSTS] DEFAULT ('') NOT NULL,
    [IDTYPE]        VARCHAR (10)    CONSTRAINT [DF_MEMBER_NEW_IDTYPE] DEFAULT ('') NOT NULL,
    CONSTRAINT [PK_MEMBER_NEW] PRIMARY KEY CLUSTERED ([MEMRFNBR] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'ID身份別', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MEMBER_NEW', @level2type = N'COLUMN', @level2name = N'IDTYPE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'接送APPMAIL驗證狀態', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MEMBER_NEW', @level2type = N'COLUMN', @level2name = N'HIAPPMAILSTS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'接送APPMAIL驗證碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MEMBER_NEW', @level2type = N'COLUMN', @level2name = N'HIAPPMAILCODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'接送APP簡訊驗證狀態', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MEMBER_NEW', @level2type = N'COLUMN', @level2name = N'HIAPPMSGSTS';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'接送APP簡訊驗證碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MEMBER_NEW', @level2type = N'COLUMN', @level2name = N'HIAPPMSGCODE';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'接送APP會員', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MEMBER_NEW', @level2type = N'COLUMN', @level2name = N'HIAPPENABLED';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'緊急聯絡人電話2', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'MEMBER_NEW', @level2type = N'COLUMN', @level2name = N'MEMCONTEL2';

