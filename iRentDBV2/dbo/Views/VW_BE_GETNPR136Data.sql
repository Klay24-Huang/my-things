CREATE VIEW [dbo].[VW_BE_GETNPR136Data]
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
      ,[CARRIERID]
      ,[NPOBAN]
      ,[NOCAMT]
      ,[CTRLAMT]
      ,[CTRLMEMO]
      ,[CLEANAMT]
      ,[CLEANMEMO]
      ,[EQUIPAMT]
      ,[EQUIPMEMO]
      ,[PARKINGAMT]
      ,[PARKINGMEMO]
      ,[TOWINGAMT]
      ,[TOWINGMEMO]
      ,[OTHERAMT]
      ,[OTHERMEMO]
      ,[PARKINGAMT2]
      ,[PARKINGMEMO2]

  FROM [dbo].[TB_NPR136Save] WHERE isRetry=1
    GO
  EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GETNPR136Data';
GO
EXECUTE sp_addextendedproperty @name = N'Owner', @value = N'Eric', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GETNPR136Data';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'取得需要補傳136的資料', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GETNPR136Data';


GO
EXECUTE sp_addextendedproperty @name = N'IsActive', @value = N'1:使用', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'VW_BE_GETNPR136Data';


GO