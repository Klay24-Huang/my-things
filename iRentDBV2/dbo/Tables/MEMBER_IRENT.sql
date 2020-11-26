﻿CREATE TABLE [dbo].[MEMBER_IRENT] (
    [A_PRGID]       VARCHAR (10)  NOT NULL,
    [A_USERID]      VARCHAR (10)  NOT NULL,
    [A_SYSDT]       DATETIME      NOT NULL,
    [U_PRGID]       VARCHAR (10)  NOT NULL,
    [U_USERID]      VARCHAR (10)  NOT NULL,
    [U_SYSDT]       DATETIME      NOT NULL,
    [MEMRFNBR]      INT           CONSTRAINT [DF_MEMBER_IRENT_MEMRFNBR] DEFAULT ((0)) NOT NULL,
    [AUDITCD]       VARCHAR (5)   CONSTRAINT [DF_MEMBER_IRENT_AUDITCD] DEFAULT ('') NOT NULL,
    [AUDITRESULT]   VARCHAR (5)   CONSTRAINT [DF_MEMBER_IRENT_AUDITRESULT] DEFAULT ('') NOT NULL,
    [AUDITREASON]   VARCHAR (300) CONSTRAINT [DF_MEMBER_IRENT_AUDITREASON] DEFAULT ('') NOT NULL,
    [CARDNO1]       VARCHAR (20)  CONSTRAINT [DF_MEMBER_IRENT_CARDNO1] DEFAULT ('') NOT NULL,
    [CARDNO2]       VARCHAR (20)  CONSTRAINT [DF_MEMBER_IRENT_CARDNO2] DEFAULT ('') NOT NULL,
    [CARD]          VARCHAR (5)   CONSTRAINT [DF_MEMBER_IRENT_CARD] DEFAULT ('') NOT NULL,
    [AUDITREASONCD] VARCHAR (2)   CONSTRAINT [DF_MEMBER_IRENT_AUDITREASONCD] DEFAULT ('') NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_MEMBER_IRENT]
    ON [dbo].[MEMBER_IRENT]([MEMRFNBR] ASC, [AUDITCD] ASC, [AUDITRESULT] ASC) WITH (FILLFACTOR = 90);
