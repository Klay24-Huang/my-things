CREATE VIEW [dbo].[VW_GetFullProjectCollectionOfCarTypeGroup]
	AS 
		SELECT Project.[PROJID],[PRONAME],[PRODESC],[PROHOUR],[PROHOUR_MAX],[PROTYPE_N],[PROTYPE_H]
      ,[PROPRICE_N],[PROPRICE_H],[ShowStart],[ShowEnd],[PRSTDT],[PRENDT],[SPCLOCK],[RNTDTLOCK],[SPECProj],Project.[use_flag],[PROJTYPE],[PayMode],[SORT],[IsMonthRent]
	  ,ProjectStation.IOType,ProjectStation.StationID
	  ,ProjectDiscount.[CARTYPE],ProjectDiscount.PRICE,ProjectDiscount.PRICE_H
	  ,ISNULL(CarTypeData.CarBrend,'') AS CarBrend,ISNULL(CarTypeData.CarTypeName,'') AS CarTypeName,CarTypeGroup.CarTypeGroupCode,CarTypeGroup.CarTypeImg,CarTypeGroup.Seat
	  ,Operator.OperatorICon,Operator.Score
  FROM [dbo].[TB_Project] AS Project
  
  LEFT JOIN [dbo].[TB_ProjectStation] AS ProjectStation  WITH(NOLOCK) ON ProjectStation.PROJID=Project.PROJID
  LEFT JOIN [dbo].[TB_ProjectDiscount] AS ProjectDiscount WITH(NOLOCK) ON ProjectDiscount.ProjID=ProjectStation.PROJID AND ProjectDiscount.ProjID=Project.PROJID
  LEFT JOIN [dbo].[TB_CarType] AS CarTypeData WITH(NOLOCK) ON CarTypeData.CarType= ProjectDiscount.[CARTYPE]
  INNER JOIN [dbo].[TB_CarTypeGroupConsist] AS Consist WITH(NOLOCK) ON CarTypeData.CarType=Consist.CarType
  INNER JOIN [dbo].[TB_CarTypeGroup] AS CarTypeGroup WITH(NOLOCK) ON CarTypeGroup.CarTypeGroupID=Consist.CarTypeGroupID
  INNER JOIN [dbo].[TB_OperatorBase] As Operator WITH(NOLOCK) ON Operator.OperatorID=CarTypeData.Operator 
  WHERE IOType='O' 
	  GO
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetFullProjectCollectionOfCarTypeGroup';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetFullProjectCollectionOfCarTypeGroup';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取得所有專案(以車型群組方式輸出)', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetFullProjectCollectionOfCarTypeGroup';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetFullProjectCollectionOfCarTypeGroup';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_GetFullProjectCollectionOfCarTypeGroup';
