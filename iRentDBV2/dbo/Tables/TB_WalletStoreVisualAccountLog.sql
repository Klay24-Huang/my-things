/*
   2021年9月28日下午 01:49:45
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
DECLARE @v sql_variant 
SET @v = N'0:未回呼，1:已回呼'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'TB_WalletStoreVisualAccountLog', N'COLUMN', N'ReCallFLG'
GO
DECLARE @v sql_variant 
SET @v = N'回呼時間'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'TB_WalletStoreVisualAccountLog', N'COLUMN', N'ReCallTime'
GO
ALTER TABLE dbo.TB_WalletStoreVisualAccountLog SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.TB_WalletStoreVisualAccountLog', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.TB_WalletStoreVisualAccountLog', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.TB_WalletStoreVisualAccountLog', 'Object', 'CONTROL') as Contr_Per 