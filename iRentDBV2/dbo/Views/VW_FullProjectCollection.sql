CREATE VIEW [dbo].[VW_FullProjectCollection]
	AS 
	SELECT Project.[PROJID],[PRONAME],[PRODESC],[PROHOUR],[PROHOUR_MAX],[PROTYPE_N],[PROTYPE_H]
      ,[PROPRICE_N],[PROPRICE_H],[ShowStart],[ShowEnd],[PRSTDT],[PRENDT],[SPCLOCK],[RNTDTLOCK],[SPECProj],Project.[use_flag],[PROJTYPE],[PayMode],[SORT],[IsMonthRent]
	  ,ProjectStation.IOType,ProjectStation.StationID
	  ,ProjectDiscount.[CARTYPE],ProjectDiscount.PRICE,ProjectDiscount.PRICE_H
	  ,ISNULL(CarTypeData.CarBrend,'') AS CarBrend,ISNULL(CarTypeData.CarTypeName,'') AS CarTypeName
  FROM [dbo].[TB_Project] AS Project
  
  LEFT JOIN [dbo].[TB_ProjectStation] AS ProjectStation  WITH(NOLOCK) ON ProjectStation.PROJID=Project.PROJID
  LEFT JOIN [dbo].[TB_ProjectDiscount] AS ProjectDiscount WITH(NOLOCK) ON ProjectDiscount.ProjID=ProjectStation.PROJID AND ProjectDiscount.ProjID=Project.PROJID
  LEFT JOIN [dbo].[TB_CarType] AS CarTypeData WITH(NOLOCK) ON CarTypeData.CarType= ProjectDiscount.[CARTYPE]
  GO
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_FullProjectCollection';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_FullProjectCollection';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取得所有專案', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_FullProjectCollection';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_FullProjectCollection';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_FullProjectCollection';