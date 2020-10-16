CREATE VIEW [dbo].[VW_GetCarBindData]
	AS 
SELECT CarInfo.[CarNo]
     
      ,CarInfo.[CID]
      ,CarInfo.[deviceToken]

      ,CarInfo.[HasIButton]
      ,CarInfo.[iButtonKey]
	  ,Car.available  AS NowStatus
	  ,ISNULL(Mobile.MobileNum,'未設定門號') AS MobileNum
	  ,IIF(ISNULL(CarMachine.MachineNo,'')='',0,1) AS BindStatus
  FROM [dbo].[TB_CarInfo] AS CarInfo
  LEFT JOIN TB_Car AS Car ON Car.CarNo=CarInfo.CarNo
  LEFT JOIN TB_CarMachine AS CarMachine ON CarMachine.MachineNo=CarInfo.CID
  LEFT JOIN TB_Mobile AS Mobile ON Mobile.MobileID=CarMachine.MobileID
        GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarBindData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarBindData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台車輛綁定資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarBindData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarBindData';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetCarBindData'
