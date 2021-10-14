/*
   2021年10月8日上午 09:32:43
   使用者: CHI88137
   伺服器: sqyhi03az.database.windows.net
   資料庫: IRENT_V2
   應用程式: 
*/

/* 為了避免任何可能發生資料遺失的問題，您應該先詳細檢視此指令碼，然後才能在資料庫設計工具環境以外的位置執行。*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_CTxSn
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_TxSn
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_TxType
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_PaymentId
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_CvsType
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_CvsCode
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_PayAmount
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_PayPeriod
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_DueDate
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_OverPaid
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_CustId
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_CustMobile
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_CustEmail
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_Code1
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_Code2
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_Code3
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_Memo
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_StatusCode
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_StatusDesc
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_Barcode64
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog
	DROP CONSTRAINT DF_TB_TaishinWalletStoreShopLog_ReCallFLG
GO
CREATE TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog
	(
	SEQNO bigint NOT NULL IDENTITY (1, 1),
	CTxSn varchar(64) NOT NULL,
	TxSn varchar(26) NOT NULL,
	TxType nchar(1) NOT NULL,
	PaymentId varchar(20) NOT NULL,
	CvsType int NOT NULL,
	CvsCode varchar(3) NOT NULL,
	PayAmount int NOT NULL,
	PayPeriod int NOT NULL,
	DueDate varchar(8) NOT NULL,
	OverPaid varchar(1) NOT NULL,
	CustId varchar(10) NULL,
	CustMobile varchar(20) NULL,
	CustEmail varchar(50) NULL,
	Code1 varchar(200) NOT NULL,
	Code2 varchar(200) NOT NULL,
	Code3 varchar(200) NOT NULL,
	Memo nvarchar(50) NULL,
	StatusCode varchar(20) NOT NULL,
	StatusDesc nvarchar(200) NOT NULL,
	Barcode64 varchar(MAX) NOT NULL,
	Url varchar(2000) NOT NULL,
	ReCallFLG int NULL,
	ReCallTime datetime NULL,
	MKTime datetime NOT NULL,
	UPDTime datetime NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog SET (LOCK_ESCALATION = TABLE)
GO
DECLARE @v sql_variant 
SET @v = N'用戶端交易代碼(GUID)'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'CTxSn'
GO
DECLARE @v sql_variant 
SET @v = N'台新交易序號'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'TxSn'
GO
DECLARE @v sql_variant 
SET @v = N'交易類別(i:新增,d:刪除)'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'TxType'
GO
DECLARE @v sql_variant 
SET @v = N'銷帳編號'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'PaymentId'
GO
DECLARE @v sql_variant 
SET @v = N'超商類型( 0:7-11,1:全家)'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'CvsType'
GO
DECLARE @v sql_variant 
SET @v = N'超商代收代號'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'CvsCode'
GO
DECLARE @v sql_variant 
SET @v = N'繳費金額'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'PayAmount'
GO
DECLARE @v sql_variant 
SET @v = N'期數'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'PayPeriod'
GO
DECLARE @v sql_variant 
SET @v = N'繳費期限'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'DueDate'
GO
DECLARE @v sql_variant 
SET @v = N'是否允許溢繳'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'OverPaid'
GO
DECLARE @v sql_variant 
SET @v = N'繳費人客戶編號'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'CustId'
GO
DECLARE @v sql_variant 
SET @v = N'繳費人行動電話'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'CustMobile'
GO
DECLARE @v sql_variant 
SET @v = N'繳費人Email'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'CustEmail'
GO
DECLARE @v sql_variant 
SET @v = N'第一段條碼文字'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'Code1'
GO
DECLARE @v sql_variant 
SET @v = N'第二段條碼文字'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'Code2'
GO
DECLARE @v sql_variant 
SET @v = N'第三段條碼文字'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'Code3'
GO
DECLARE @v sql_variant 
SET @v = N'備註'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'Memo'
GO
DECLARE @v sql_variant 
SET @v = N'回應狀態代碼'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'StatusCode'
GO
DECLARE @v sql_variant 
SET @v = N'回應狀態說明'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'StatusDesc'
GO
DECLARE @v sql_variant 
SET @v = N'BarCode64'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'Barcode64'
GO
DECLARE @v sql_variant 
SET @v = N'短網址URL'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'Url'
GO
DECLARE @v sql_variant 
SET @v = N'0:未回呼,1:已回呼'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'ReCallFLG'
GO
DECLARE @v sql_variant 
SET @v = N'回呼時間'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'ReCallTime'
GO
DECLARE @v sql_variant 
SET @v = N'建立時間'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'MKTime'
GO
DECLARE @v sql_variant 
SET @v = N'更新時間'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tmp_TB_TaishinWalletStoreShopLog', N'COLUMN', N'UPDTime'
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_CTxSn DEFAULT ('') FOR CTxSn
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_TxSn DEFAULT ('') FOR TxSn
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_TxType DEFAULT ('') FOR TxType
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_PaymentId DEFAULT ('') FOR PaymentId
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_CvsType DEFAULT ((0)) FOR CvsType
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_CvsCode DEFAULT ('') FOR CvsCode
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_PayAmount DEFAULT ((0)) FOR PayAmount
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_PayPeriod DEFAULT ((1)) FOR PayPeriod
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_DueDate DEFAULT ('') FOR DueDate
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_OverPaid DEFAULT ('N') FOR OverPaid
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_CustId DEFAULT ('') FOR CustId
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_CustMobile DEFAULT ('') FOR CustMobile
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_CustEmail DEFAULT ('') FOR CustEmail
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_Code1 DEFAULT ('') FOR Code1
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_Code2 DEFAULT ('') FOR Code2
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_Code3 DEFAULT ('') FOR Code3
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_Memo DEFAULT ('') FOR Memo
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_StatusCode DEFAULT ('') FOR StatusCode
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_StatusDesc DEFAULT ('') FOR StatusDesc
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_Barcode64 DEFAULT ('') FOR Barcode64
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_Url DEFAULT '' FOR Url
GO
ALTER TABLE dbo.Tmp_TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	DF_TB_TaishinWalletStoreShopLog_ReCallFLG DEFAULT ((0)) FOR ReCallFLG
GO
SET IDENTITY_INSERT dbo.Tmp_TB_TaishinWalletStoreShopLog ON
GO
IF EXISTS(SELECT * FROM dbo.TB_TaishinWalletStoreShopLog)
	 EXEC('INSERT INTO dbo.Tmp_TB_TaishinWalletStoreShopLog (SEQNO, CTxSn, TxSn, TxType, PaymentId, CvsType, CvsCode, PayAmount, PayPeriod, DueDate, OverPaid, CustId, CustMobile, CustEmail, Code1, Code2, Code3, Memo, StatusCode, StatusDesc, Barcode64, Url, ReCallFLG, ReCallTime, MKTime, UPDTime)
		SELECT SEQNO, CTxSn, TxSn, TxType, PaymentId, CvsType, CvsCode, PayAmount, PayPeriod, DueDate, OverPaid, CustId, CustMobile, CustEmail, Code1, Code2, Code3, Memo, StatusCode, StatusDesc, Barcode64, Url, ReCallFLG, ReCallTime, MKTime, UPDTime FROM dbo.TB_TaishinWalletStoreShopLog WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_TB_TaishinWalletStoreShopLog OFF
GO
DROP TABLE dbo.TB_TaishinWalletStoreShopLog
GO
EXECUTE sp_rename N'dbo.Tmp_TB_TaishinWalletStoreShopLog', N'TB_TaishinWalletStoreShopLog', 'OBJECT' 
GO
ALTER TABLE dbo.TB_TaishinWalletStoreShopLog ADD CONSTRAINT
	PK_TB_TaishinWalletStoreShopLog PRIMARY KEY CLUSTERED 
	(
	SEQNO
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
COMMIT
select Has_Perms_By_Name(N'dbo.TB_TaishinWalletStoreShopLog', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.TB_TaishinWalletStoreShopLog', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.TB_TaishinWalletStoreShopLog', 'Object', 'CONTROL') as Contr_Per 