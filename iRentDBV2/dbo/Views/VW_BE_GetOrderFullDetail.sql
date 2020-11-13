CREATE VIEW [dbo].[VW_BE_GetOrderFullDetail]
	AS 
	SELECT  OrderMain.[order_number] AS OrderNo,[IDNO],ISNULL(MemberData.MEMCNAME,'') AS UserName,booking_date AS BookingDate ,[car_mgt_status] AS CMS,[booking_status] AS BS,[cancel_status] AS CS,[modified_status] AS MS
       ,[start_time] AS SD,[stop_time] AS ED	, ISNULL
                                     ((SELECT          TOP (1) StopTime
                                         FROM              TB_OrderExtendHistory AS History
                                         WHERE          (History.order_number = OrderMain.order_number)), '1911-01-01 00:00:00') AS OStopTime
	  ,ISNULL((SELECT [Location] FROM [TB_iRentStation] WHERE StationID=OrderMain.lend_place),'') AS LStation 
      ,ISNULL((SELECT [Location] FROM [TB_iRentStation] WHERE StationID=OrderMain.[return_place]),'') AS RStation 
      ,CarDetail.CarTypeName,[OrderMain].CarNo,Project.PRONAME  ,[bill_option] AS InvoicKind   ,[unified_business_no] AS [BCode],MemberData.MEMTEL AS TEL
	  ,init_price AS PurePrice,OrderMain.[CARRIERID],OrderMain.[NPOBAN],[title]
	  ,ISNULL(Detail.final_start_time,'1911-01-01 00:00:00') AS FS,ISNULL(Detail.final_stop_time,'1911-01-01 00:00:00') AS FE
	  ,ISNULL(Detail.start_mile,-1) AS StartMile,ISNULL(Detail.end_mile,-1) AS StopMile,ISNULL(Detail.pure_price,0) AS CarRent,ISNULL(Detail.final_price,0) AS FinalPrice,ISNULL(Detail.fine_price,0) AS FinePrice,ISNULL(Detail.mileage_price,0) AS Mileage,ISNULL(Detail.Etag,0) AS eTag,ISNULL(Detail.TransDiscount,0) AS TransDiscount
	  ,ISNULL(Detail.gift_point,0) AS CarPoint,ISNULL(Detail.gift_motor_point,0) AS MotorPoint
	   ,ISNULL([fine_Time],'1911-01-01 00:00:00') AS FineTime,[Insurance],[InsurancePurePrice]
	   ,ISNULL((SELECT descript FROM TB_FeedBack AS FeedBack WHERE FeedBack.OrderNo=OrderMain.order_number AND mode=0),'') AS LFeedBack
	   ,ISNULL((SELECT descript FROM TB_FeedBack AS FeedBack WHERE FeedBack.OrderNo=OrderMain.order_number AND mode=1),'') AS RFeedBack
	   ,ISNULL(MotorData.[P_LBA] ,0) AS [P_LBA] ,ISNULL(MotorData.[P_RBA] ,0) AS [P_RBA] ,ISNULL(MotorData.[P_TBA] ,0) AS [P_TBA] ,ISNULL(MotorData.[P_MBA] ,0) AS [P_MBA] 
	   ,ISNULL(MotorData.[P_lon] ,0) AS [P_lon] ,ISNULL(MotorData.[P_lat] ,0) AS [P_lat] ,ISNULL(MotorData.[R_LBA] ,0) AS [R_LBA] ,ISNULL(MotorData.[R_RBA] ,0) AS [R_RBA] 
	   ,ISNULL(MotorData.[R_TBA] ,0) AS [R_TBA] ,ISNULL(MotorData.[R_MBA] ,0) AS [R_MBA] ,ISNULL(MotorData.[R_lon] ,0) AS [R_lon] ,ISNULL(MotorData.[R_lat] ,0) AS [R_lat] 
	   ,ISNULL(MotorData.[Reward],0) AS [Reward]
	   ,ISNULL(OrderOtherFee.[CarDispatch]      ,0) AS [CarDispatch],ISNULL(OrderOtherFee.[DispatchRemark]   ,'') AS [DispatchRemark],ISNULL(OrderOtherFee.[CleanFee]         ,0) AS [CleanFee]         
	   ,ISNULL(OrderOtherFee.[CleanFeeRemark]   ,'') AS [CleanFeeRemark],ISNULL(OrderOtherFee.[DestroyFee]       ,0) AS [DestroyFee] ,ISNULL(OrderOtherFee.[DestroyFeeRemark] ,'') AS [DestroyFeeRemark] 
	   ,ISNULL(OrderOtherFee.[ParkingFee]       ,0) AS [ParkingFee],ISNULL(OrderOtherFee.[ParkingFeeRemark] ,'') AS [ParkingFeeRemark],ISNULL(OrderOtherFee.[DraggingFee]      ,0) AS [DraggingFee]      
	   ,ISNULL(OrderOtherFee.[DraggingFeeRemark],'') AS [DraggingFeeRemark],ISNULL(OrderOtherFee.[OtherFee]         ,0) AS [OtherFee],ISNULL(OrderOtherFee.[OtherFeeRemark]   ,'') AS [OtherFeeRemark]   
	   ,ISNULL((SELECT Amount FROM TB_OrderParkingFeeByMachi AS Machi WITH(NOLOCK) WHERE Machi.OrderNo=OrderMain.order_number),0) AS MachiFee
	   --20201112 ADD BY JERRY 增加缺漏欄位
	   ,ISNULL(CarDetail.EngineNO,'') AS EngineNO
	   ,ISNULL(CarDetail.CarColor,'') AS CarColor
	   ,ISNULL(CarDetail.CarBrend,'') AS CarBrend
	   ,ISNULL(MemberData.MEMBIRTH,'') AS MEMBIRTH
	   ,ISNULL(City.CityName ,'') AS CityName
	   ,ISNULL(AreaZip.AreaName ,'') AS AreaName
	   ,ISNULL(MemberData.MEMADDR,'') AS MEMADDR
  FROM [dbo].[TB_OrderMain] AS OrderMain
  LEFT JOIN TB_OrderDetail AS Detail ON Detail.order_number=OrderMain.order_number
  LEFT JOIN TB_MemberData AS MemberData ON MemberData.MEMIDNO=OrderMain.IDNO
  LEFT JOIN TB_AreaZip AS AreaZip ON MemberData.MEMCITY=AreaZip.AreaID 
  LEFT JOIN TB_City AS City ON AreaZip.CityID=City.CityID
  LEFT JOIN VW_GetCarDetail AS CarDetail ON CarDetail.CarNo=OrderMain.CarNo
  LEFT JOIN TB_Project AS Project ON Project.PROJID=OrderMain.ProjID
  LEFT JOIN TB_PayMoney AS PayMoney ON PayMoney.OrderNo=OrderMain.order_number
  LEFT JOIN TB_OrderDataByMotor AS MotorData ON MotorData.OrderNo=OrderMain.order_number
   LEFT JOIN TB_OrderOtherFee AS OrderOtherFee ON OrderOtherFee.OrderNo=OrderMain.order_number
       GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台訂單明細取得完整資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetOrderFullDetail';