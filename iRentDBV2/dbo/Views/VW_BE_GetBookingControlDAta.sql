CREATE VIEW [dbo].[VW_BE_GetBookingControlData]
	AS
	SELECT 
       [order_number] AS OrderNo
      ,[PROCD]
      ,[ODCUSTID]
      ,[ODCUSTNM]
      ,[TEL1]
      ,[TEL2]
      ,[TEL3]
      ,[ODDATE]
      ,[GIVEDATE]
      ,[GIVETIME]
      ,[RNTDATE]
      ,[RNTTIME]
      ,[CARTYPE]
      ,[CARNO]
      ,[OUTBRNH]
      ,[INBRNH]
      ,[ORDAMT]
      ,[REMARK]
      ,[PAYAMT]
      ,[RPRICE]
      ,[RINV]
      ,[DISRATE]
      ,[NETRPRICE]
      ,[RNTAMT]
      ,[INSUAMT]
      ,[RENTDAY]
      ,[EBONUS]
      ,[PROJTYPE]
      ,[TYPE]
      ,[INVKIND]
      ,[INVTITLE]
      ,[UNIMNO]
      ,[TSEQNO]
      ,[CARRIERID]
      ,[NPOBAN]
      ,[NOCAMT]
  FROM [dbo].[TB_BookingControl]
                                                            GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetBookingControlData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetBookingControlData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出要送短租060的資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetBookingControlData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetBookingControlData';


GO