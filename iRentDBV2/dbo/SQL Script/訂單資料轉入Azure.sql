

SELECT TOP 10 [order_number]
      ,[IDNO]=citizen_id
      ,[CarNo]=assigned_car_id
      ,[City]
      ,[ZipCode]
      ,[email]
      ,[ProjID]=A.premium
      ,[lend_place]
      ,[return_place]
      ,[start_time]
      ,[stop_time]
      ,[stop_pick_time]=DATEADD(MINUTE,30,start_time)
      ,[fine_Time]
      ,[init_price]
      ,[Insurance]=0
      ,[InsurancePurePrice]=0
      ,[car_mgt_status]
      ,[booking_status]
      ,[cancel_status]
      ,[modified_status]
      ,[bill_option]
      ,[title]
      ,[unified_business_no]
      ,[invoiceAddress]
      ,[booking_date]
      ,[spec_status]
      ,[invoiceCode]
      ,[isDelete]
      ,[ProjType]=D.PROJTYPE
      ,[PayMode]=D.PayMode
      ,[init_TransDiscount]=0
      ,[CARRIERID]=C.CARRIERID
      ,[NPOBAN]=C.NPOBAN
      ,[invoice_price]=ISNULL(F.INVAMT,0)    --需要轉短租的發票資料
      --,invoice_date=ISNULL(F.INVADTE,'')   --需要轉短租的發票資料
  FROM [dbo].[TB_BookingMain_201609] A WITH(NOLOCK)
  JOIN MEMBER_NEW B WITH(NOLOCK) ON A.citizen_id=B.MEMIDNO
  LEFT JOIN MEMBER_NEW_ONTRANS C WITH(NOLOCK) ON A.citizen_id=C.MEMIDNO
  LEFT JOIN TB_Project D WITH(NOLOCK) ON A.premium=D.PROJID
  LEFT JOIN LSIRENTINVTF_SYNC_AZURE F WITH(NOLOCK) ON A.order_number=F.IRENTORDNO 

SELECT TOP 10 A.[order_number]
      ,[already_lend_car]
      ,[already_return_car]
      ,[extend_stop_time]
      ,[force_extend_stop_time]
      ,[final_start_time]
      ,[final_stop_time]
      ,[start_door_time]
      ,[end_door_time]
      ,[transaction_no]
      ,[final_price]
      ,[pure_price]
      ,[mileage_price]
      ,[Insurance_price]=0
      ,[fine_price]
      ,[fine_interval]
      ,[fine_rate]
      ,[gift_point]=CASE WHEN LEFT(B.assigned_car_id,1)='E' THEN 0 ELSE gift_point END
      ,[gift_motor_point]=CASE WHEN LEFT(B.assigned_car_id,1)='E' THEN gift_point ELSE 0 END
      ,[monthly_workday]=D.UseWorkDayHours
      ,[monthly_holiday]=D.UseHolidayHours
      ,[Etag]
      ,[already_payment]
      ,[start_mile]
      ,[end_mile]
      ,[trade_status]
      ,[parkingFee]=C.Amount
      ,[parkingSpace]=''
      ,[TransDiscount]=0
  FROM [dbo].[TB_BookingDetail_201610] A WITH(NOLOCK)
  JOIN TB_BookingMain_201609 B WITH(NOLOCK) ON A.order_number=B.order_number
  JOIN TB_OrderParkingData_202001 C WITH(NOLOCK) ON A.order_number=C.order_number
  JOIN TB_SubScriptionHistory_201907 D WITH(NOLOCK) ON A.order_number=D.OrderNum
  JOIN TB_ParkingSpace_201701 E WITH(NOLOCK) ON B.assigned_car_id=E.CarNo