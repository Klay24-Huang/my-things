CREATE VIEW [dbo].[VW_BE_GetFETSetCardLog]
	AS 
    SELECT SendCMD.requestId
       ,CarInfo.CarNo
       ,SendCMD.method
      ,SendCMD.[CID]
      ,[SendParams]
	  ,ReceiveCMD.[CmdReply]
      ,SendCMD.[MKTime] AS SendDate
	  ,ReceiveCMD.MKTime AS ReceiveDate
  FROM [dbo].[TB_SendFETCatCMD] AS SendCMD
  LEFT JOIN TB_CarInfo AS CarInfo ON CarInfo.CID=SendCMD.CID
  LEFT JOIN [dbo].[TB_ReceiveFETCatCMD] AS ReceiveCMD ON SendCMD.method=ReceiveCMD.method AND SendCMD.requestId=ReceiveCMD.requestId
  WHERE SendCMD.method IN ('SetClientCardNo','ClearAllClientCard','SetUnivCardNo','ClearAllUnivCard')
    GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFETSetCardLog';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFETSetCardLog';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'後台取出遠傳車機設定卡號紀錄', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFETSetCardLog';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFETSetCardLog';


GO
EXECUTE sp_addextendedproperty @name = N'Comments', @value = N'', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GetFETSetCardLog';