CREATE VIEW [dbo].[VW_BE_BeforeNPR136GetData]
	AS 
	SELECT [PROCD]
      ,[ORDNO]
      ,[IRENTORDNO]
      ,[CUSTID]
      ,[CUSTNM]
      ,[BIRTH]
      ,[CUSTTYPE]
      ,[ODCUSTID]
      ,[CARTYPE]
      ,[CARNO]
      ,[TSEQNO]
      ,[GIVEDATE]
      ,[GIVETIME]
      ,[RENTDAYS]
      ,[GIVEKM]
      ,[OUTBRNHCD]
      ,[RNTDATE]
      ,[RNTTIME]
      ,[RNTKM]
      ,[INBRNHCD]
      ,[RPRICE]
      ,[RINSU]
      ,[DISRATE]
      ,[OVERHOURS]
      ,[OVERAMT2]
      ,[RNTAMT]
      ,[RENTAMT]
      ,[LOSSAMT2]
      ,[PROJID]
      ,[REMARK]
      ,[INVKIND]
      ,[UNIMNO]
      ,[INVTITLE]
      ,[INVADDR]
      ,[GIFT]
      ,[GIFT_MOTO]
      ,[CARDNO]
      ,[PAYAMT]
      ,[AUTHCODE]
      ,[isRetry]
      ,[RetryTimes]
      ,[CARRIERID]
      ,[NPOBAN]
      ,[NOCAMT]  
      ,ISNULL(OtherFee.CarDispatch,'0') AS CTRLAMT
	  ,ISNULL(OtherFee.DispatchRemark,'') AS CTRLMEMO
	  ,ISNULL(OtherFee.CleanFee,'0') AS CLEANAMT
	  ,ISNULL(OtherFee.CleanFeeRemark,'') AS CLEANMEMO
	  ,ISNULL(OtherFee.DestroyFee,'0') AS EQUIPAMT
	  ,ISNULL(OtherFee.DestroyFeeRemark,'') AS EQUIPMEMO
	  ,ISNULL(OtherFee.DraggingFee,'0') AS TOWINGAMT
	  ,ISNULL(OtherFee.DraggingFeeRemark,'') AS TOWINGMEMO
	  ,ISNULL(OtherFee.OtherFee,'0') AS OTHERAMT
	  ,ISNULL(OtherFee.OtherFeeRemark,'') AS OTHERMEMO
	  ,ISNULL(OtherFee.ParkingFee,'0') AS PARKINGAMT
	  ,ISNULL(OtherFee.ParkingFeeRemark,'') AS PARKINGMEMO
	   , ISNULL(OtherFee.ParkingFeeByMachi, 0) AS PARKINGAMT2
	  , ISNULL(OtherFee.ParkingFeeByMachiRemark, '') AS PARKINGMEMO2
  FROM [dbo].[TB_ReturnCarControl] AS ReturnCar LEFT JOIN TB_OrderOtherFee AS OtherFee ON ReturnCar.IRENTORDNO=OtherFee.OrderNo 
  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_BeforeNPR136GetData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_BeforeNPR136GetData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'第一次修改合約，由130取得相關資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_BeforeNPR136GetData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_BeforeNPR136GetData';


GO