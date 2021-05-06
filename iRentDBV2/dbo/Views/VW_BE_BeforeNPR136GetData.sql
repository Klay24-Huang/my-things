CREATE VIEW [dbo].[VW_BE_BeforeNPR136GetData]
	AS 
	SELECT A.[PROCD]
      ,[ORDNO]=CASE WHEN A.ORDNO='' THEN B.ORDNO ELSE A.ORDNO END
      ,[IRENTORDNO]
      ,[CUSTID]
      ,[CUSTNM]
      ,[BIRTH]
      ,[CUSTTYPE]
      ,A.[ODCUSTID]
      ,A.[CARTYPE]
      ,A.[CARNO]
      ,A.[TSEQNO]
      ,A.[GIVEDATE]
      ,A.[GIVETIME]
      ,[RENTDAYS]
      ,[GIVEKM]
      ,[OUTBRNHCD]
      ,A.[RNTDATE]
      ,A.[RNTTIME]
      ,[RNTKM]
      ,[INBRNHCD]
      ,A.[RPRICE]
      ,[RINSU]
      ,A.[DISRATE]
      ,[OVERHOURS]
      ,[OVERAMT2]
      ,A.[RNTAMT]
      ,[RENTAMT]
      ,[LOSSAMT2]
      ,[PROJID]
      ,A.[REMARK]
      ,A.[INVKIND]
      ,A.[UNIMNO]
      ,A.[INVTITLE]
      ,[INVADDR]
      ,[GIFT]
      ,[GIFT_MOTO]
      ,[CARDNO]
      ,A.[PAYAMT]
      ,[AUTHCODE]
      ,A.[isRetry]
      ,A.[RetryTimes]
      ,A.[CARRIERID]
      ,A.[NPOBAN]
      ,A.[NOCAMT]  
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
	  --,ISNULL(OtherFee.ParkingFeeByMachi, 0) AS PARKINGAMT2
	  -- 20210506;UPD BY YEH REASON.停車費改抓Detail的總和
	  ,ISNULL(Detail.parkingFee, 0) AS PARKINGAMT2
	  ,ISNULL(OtherFee.ParkingFeeByMachiRemark, '') AS PARKINGMEMO2
  FROM [dbo].[TB_ReturnCarControl] AS A WITH(NOLOCK)
  JOIN TB_BookingControl B WITH(NOLOCK) ON B.order_number=A.IRENTORDNO
  LEFT JOIN TB_OrderOtherFee AS OtherFee WITH(NOLOCK) ON A.IRENTORDNO=OtherFee.OrderNo
  LEFT JOIN TB_OrderDetail AS Detail WITH(NOLOCK) ON A.IRENTORDNO=Detail.order_number
  GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_BeforeNPR136GetData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_BeforeNPR136GetData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'第一次修改合約，由130取得相關資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_BeforeNPR136GetData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_BeforeNPR136GetData';


GO