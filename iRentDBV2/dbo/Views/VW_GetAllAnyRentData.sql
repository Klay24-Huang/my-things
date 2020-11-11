CREATE VIEW [dbo].[VW_GetAllAnyRentData]
	AS 
SELECT Car.CarNo,
       CarStatus.CID,
       CarStatus.Token,
       CarStatus.deviceType,
       CarStatus.ACCStatus,
       CarStatus.GPSStatus,
       CarStatus.GPSTime,
       CarStatus.OBDStatus,
       CarStatus.GPRSStatus,
       CarStatus.PowerOnStatus,
       CarStatus.CentralLockStatus,
       CarStatus.DoorStatus,
       CarStatus.LockStatus,
       CarStatus.IndoorLightStatus,
       CarStatus.SecurityStatus,
       CarStatus.Speed,
       CarStatus.Volt,
       CarStatus.Latitude,
       CarStatus.Longitude,
       CarStatus.Millage,
       CarStatus.extDeviceStatus1,
       CarStatus.extDeviceStatus2,
       CarStatus.extDeviceData3,
       CarStatus.extDeviceData4,
       CarStatus.extDeviceData7,
       CarStatus.WriteTime,
       Car.available,
       Car.nowStationID,
       FullProj.PROJID,
       FullProj.PRONAME,
       FullProj.PRODESC,
       FullProj.CarBrend,
       FullProj.CarTypeName,
       Car.CarType,
       FullProj.ShowStart,
       FullProj.ShowEnd,
       FullProj.PRSTDT,
       FullProj.PRENDT,
       FullProj.PRICE,
       FullProj.PRICE_H,
       op.OperatorICon,
       op.Score,
       cg.CarTypeImg,
       cg.Seat,
       Station.Area,
       cg.CarTypeGroupCode      --20201109 ADD BY ADAM REASON.
FROM dbo.TB_CarStatus AS CarStatus WITH (NOLOCK)
INNER JOIN dbo.TB_Car AS Car WITH (NOLOCK) ON Car.CarNo = CarStatus.CarNo
INNER JOIN dbo.TB_OperatorBase AS op WITH (NOLOCK) ON op.OperatorID = Car.Operator
INNER JOIN dbo.TB_CarTypeGroupConsist AS cgc WITH (NOLOCK) ON cgc.CarType = Car.CarType
INNER JOIN dbo.TB_CarTypeGroup AS cg WITH (NOLOCK) ON cg.CarTypeGroupID = cgc.CarTypeGroupID
INNER JOIN dbo.VW_FullProjectCollection AS FullProj WITH (NOLOCK) ON Car.nowStationID = FullProj.StationID AND Car.CarType = FullProj.CARTYPE AND FullProj.IOType = 'O' AND FullProj.PROJTYPE = 3
LEFT OUTER JOIN dbo.TB_iRentStation AS Station WITH (NOLOCK) ON Station.StationID = Car.nowStationID
WHERE (Car.available < 2)
  AND (FullProj.ShowStart <= GETDATE())
  AND (FullProj.ShowEnd > GETDATE())
  OR (Car.available < 2)
  AND (FullProj.PRSTDT <= GETDATE())
  AND (FullProj.PRENDT > GETDATE())