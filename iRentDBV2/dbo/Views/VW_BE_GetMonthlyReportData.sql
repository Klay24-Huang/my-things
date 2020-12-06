CREATE VIEW [dbo].[VW_BE_GetMonthlyReportData]
	AS 
	SELECT OrderNo,History.IDNO,lend_place,UseWorkDayHours,UseHolidayHours,UseMotoTotalHours,History.MKTime,ISNULL(Rate.[SEQNO],0) AS SEQNO,ISNULL(Rate.[ProjID],'') AS ProjID,ISNULL(Rate.[ProjNM],'') AS ProjNM  
FROM TB_MonthlyRentHistory AS History 
LEFT JOIN [TB_MonthlyRent] AS Rate WITH(NOLOCK) ON History.MonthlyRentId=Rate.MonthlyRentId 
 INNER JOIN TB_OrderMain AS Main ON Main.order_number=OrderNo

   GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetMonthlyReportData';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetMonthlyReportData';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'【後台】取得月租明細資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetMonthlyReportData';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetMonthlyReportData';


GO
