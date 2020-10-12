CREATE TABLE [dbo].[TB_OrderMain]
(
	[order_number] [bigint] IDENTITY(1,1) NOT NULL,
	[IDNO] [varchar](20) NOT NULL DEFAULT '',
	[CarNo] [varchar](10) NOT NULL DEFAULT '',
	[City] [tinyint] NOT NULL DEFAULT 0,
	[ZipCode] [varchar](6) NOT NULL DEFAULT '',
	[email] [varchar](200) NOT NULL DEFAULT '',
	[ProjID] [varchar](10) NOT NULL DEFAULT '',
	[lend_place] [varchar](10) NOT NULL DEFAULT '',
	[return_place] [varchar](10) NOT NULL DEFAULT '',
	[start_time] [datetime] NOT NULL,
	[stop_time] [datetime] NOT NULL,
	[stop_pick_time][datetime] NOT NULL,
	[fine_Time] [datetime] NULL,
	[init_price] [int] NOT NULL DEFAULT 0,
	[Insurance] [int] NOT NULL DEFAULT 0,
	[InsurancePurePrice] [INT] NOT NULL DEFAULT 0,
	[car_mgt_status] [tinyint] NOT NULL DEFAULT 0,
	[booking_status] [tinyint] NOT NULL DEFAULT 0,
	[cancel_status] [tinyint] NOT NULL DEFAULT 0,
	[modified_status] [tinyint] NOT NULL DEFAULT 0,
	[bill_option] [tinyint] NOT NULL DEFAULT 1,
	[title] [nvarchar](96) NOT NULL DEFAULT N'',
	[unified_business_no] [varchar](8) NOT NULL DEFAULT '',
	[invoiceAddress] [nvarchar](200) NOT NULL DEFAULT N'',
	[booking_date] [datetime] NOT NULL DEFAULT DATEADD(HOUR,8,GETDATE()),
	[spec_status] [int] NULL DEFAULT 0,
	[invoiceCode] [varchar](100) NULL DEFAULT '',
	[isDelete] [tinyint] NOT NULL DEFAULT 0, 
	[ProjType] [tinyint] NOT NULL DEFAULT 5,
	[PayMode] [tinyint] NOT NULL DEFAULT 0,
    [init_TransDiscount] INT NOT NULL DEFAULT -1, 
    [CARRIERID]      VARCHAR (20)   DEFAULT ('') NOT NULL,
    [NPOBAN]         VARCHAR (20)   DEFAULT ('') NOT NULL,
    [invoice_price] INT NOT NULL DEFAULT 0, 
    [invoice_date] VARCHAR(20) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_TB_OrderMain] PRIMARY KEY ([order_number]),
)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂單編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'order_number'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'會員帳號(身份證)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name='IDNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出租車輛ID(車牌)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name='CarNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'縣市代碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'City'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'郵遞區號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'ZipCode'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'優惠專案代碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name='ProjID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出車據點' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'lend_place'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'還車據點' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'return_place'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'預約取車時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'start_time'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'預約還車時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'stop_time'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'租金試算' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'init_price'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'取還車狀態：0 = 尚未取車、1 = 已經上傳出車照片、2 = 已經簽名出車單、3 = 已經信用卡認證、4 = 已經取車(記錄起始時間)、11 = 已經紀錄還車時間、12 = 已經上傳還車角度照片、13 = 已經上傳還車車損照片、14 = 已經簽名還車單、15 = 已經信用卡付款、16 = 已經檢查車輛完成並已經解除卡號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'car_mgt_status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'預約單狀態0 = 會員預約、1 = 管理員清潔預約、2 = 管理員保修預約、3 = 延長用車狀態、4 = 強迫延長用車狀態、5 = 合約完成(已完成解除卡號動作)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'booking_status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訂單修改狀態：0 = 無(訂單未刪除，正常預約狀態)、1 = 修改指派車輛(此訂單因其他預約單強迫延長而更改過訂單 or 後台重新配車過 or 取車時無車，重新配車)、2 = 此訂單被人工介入過(後台協助取還車 or 後台修改訂單資料)、3 = 訂單已取消(會員主動取消 or 逾時15分鐘未取車)、4 = 訂單已取消(因車輛仍在使用中又無法預約到其他車輛而取消)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'cancel_status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'發票寄送方式：1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'bill_option'
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'愛心碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderMain', @level2type = N'COLUMN', @level2name = N'NPOBAN';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'手機條碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'TB_OrderMain', @level2type = N'COLUMN', @level2name = N'CARRIERID';
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'發票抬頭' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'title'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'統編' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'unified_business_no'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'發票寄送地址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'invoiceAddress'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'預約單寫入時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'TB_OrderMain', @level2type=N'COLUMN',@level2name=N'booking_date'
GO

CREATE INDEX [IX_TB_OrderMain_Search] ON [dbo].[TB_OrderMain] ([CarNo], [start_time], [stop_time])

GO

CREATE INDEX [IX_TB_OrderMain_Search_Status] ON [dbo].[TB_OrderMain] ([booking_status], [car_mgt_status], [cancel_status], [CarNo])

GO

CREATE INDEX [IX_TB_OrderMain_Search_Booking_Status] ON [dbo].[TB_OrderMain] ([IDNO], [booking_status], [cancel_status], [car_mgt_status])

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否刪除（不顯示於客戶訂單，0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'isDelete'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'加購安心服務(0:否;1:有)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'Insurance'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'訂單基本資料檔',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = NULL,
    @level2name = NULL
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'會員email',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'email'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'逾時時間',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'fine_Time'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否有修改訂單（0:否;1:有)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'modified_status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'發票號碼',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'invoiceCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否為特殊訂單（0:否;1:是)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'spec_status'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'安心服務預估金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'InsurancePurePrice'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'專案類型：0:同站;3:路邊;4:機車',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'ProjType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'計費模式：0:以時計費;1:以分計費',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'PayMode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'預估轉乘優惠可折抵',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'init_TransDiscount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'發票金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'invoice_price'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'發票金額',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'TB_OrderMain',
    @level2type = N'COLUMN',
    @level2name = N'invoice_date'