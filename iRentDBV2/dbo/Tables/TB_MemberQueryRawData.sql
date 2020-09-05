CREATE TABLE [dbo].[TB_MemberQueryRawData] (
    [MEMIDNO]       VARCHAR (20)   DEFAULT ('') NOT NULL,
    [MEMCNAME]      NVARCHAR (10)  DEFAULT (N'') NOT NULL,
    [MEMSEX]        VARCHAR (5)    DEFAULT ('') NOT NULL,
    [MEMDRIDNO]     VARCHAR (20)   DEFAULT ('') NOT NULL,
    [MEMCEIL]       VARCHAR (20)   DEFAULT ('') NOT NULL,
    [MEMPWD]        VARCHAR (20)   DEFAULT ('') NOT NULL,
    [MEMEMAIL]      VARCHAR (200)  DEFAULT ('') NOT NULL,
    [MEMBIRTH]      VARCHAR (20)   DEFAULT ('') NOT NULL,
    [MEMCITY]       VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMPOST]       VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMADDR]       NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [MEMTELZIP]     VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMTEL]        VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMTEL_EXT]    VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCOMTELZIP]  VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCOMTEL]     VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCOMTEL_EXT] VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCONNM]      NVARCHAR (20)  DEFAULT (N'') NOT NULL,
    [MEMSELKEY_K]   VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCONTEL]     VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MMSG]          VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCD]         VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCITY2]      VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMPOST2]      VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMADDR2]      NVARCHAR (200) DEFAULT (N'') NOT NULL,
    [MEMDM]         VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCARDRCV]    VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMVERCD]      VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCARDNO1]    VARCHAR (20)   DEFAULT ('') NOT NULL,
    [MEMCARDNO2]    VARCHAR (20)   DEFAULT ('') NOT NULL,
    [MEMCARDCD]     VARCHAR (10)   DEFAULT ('') NOT NULL,
    [UNIMNO]        VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMSENDCD]     VARCHAR (10)   DEFAULT ('') NOT NULL,
    [IRENTBRNHCD]   VARCHAR (10)   DEFAULT ('') NOT NULL,
    [SPCSTATUS]     VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMCITYNM]     NVARCHAR (20)  DEFAULT (N'') NOT NULL,
    [MEMPOSTNM]     NVARCHAR (20)  DEFAULT (N'') NOT NULL,
    [MEMCITY2NM]    NVARCHAR (20)  DEFAULT (N'') NOT NULL,
    [MEMPOST2NM]    NVARCHAR (20)  DEFAULT (N'') NOT NULL,
    [MEMRFNBR]      VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MEMGROUP]      VARCHAR (10)   DEFAULT ('') NOT NULL,
    [IRENTFLG]      VARCHAR (10)   DEFAULT ('') NOT NULL,
    [MARKETING]     VARCHAR (10)   DEFAULT ('') NOT NULL,
    [CARRIERID]     VARCHAR (20)   DEFAULT ('') NOT NULL,
    [NPOBAN]        VARCHAR (20)   DEFAULT ('') NOT NULL,
    [RawDataType]   TINYINT        DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_TB_MemberQueryRawData] PRIMARY KEY CLUSTERED ([MEMIDNO] ASC, [RawDataType] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1:已審核過;0:審核中', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberQueryRawData', @level2type = N'COLUMN', @level2name = N'RawDataType';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'短租會員查詢RawData', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_MemberQueryRawData';

