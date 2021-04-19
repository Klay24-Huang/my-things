/****** Object:  Trigger [dbo].[TR_UpdateCarStatus]    Script Date: 2021/3/30 下午 05:11:03 ******/

CREATE TRIGGER [dbo].[TR_UpdateCarStatus] ON [dbo].[TB_CarStatus]
AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	DECLARE @CarNo			VARCHAR(10);	--車號
	DECLARE @CID			VARCHAR(10);	--車機編號
	DECLARE @SPEED			INT;			--速度
	DECLARE @DoorStatus		VARCHAR(10);	--車門狀態：1111關門;0000開門
	DECLARE @AccOn			INT;			--車輛發動狀態，發動為1，熄火為0
	DECLARE @PowOn			INT;			--引擎狀態，發動為1，熄火為0
	DECLARE @OrderNum		BIGINT;			--訂單編號
	DECLARE @NowTime		DATETIME;		--系統時間
    DECLARE @Receiver       VARCHAR(450);	--收信者MAIL
	DECLARE @EventType      TINYINT;		--告警事件
	DECLARE @StationID		VARCHAR(10);	--據點代碼
	DECLARE @IsMotor		INT;			--是否為機車（0:否;1:是)

	SET @CarNo='';
	SET @CID='';
	SET @SPEED=0;
	SET @DoorStatus='';
	SET @AccOn=0;
	SET @PowOn=0;
	SET @OrderNum=0;
	SET @NowTime=DATEADD(HOUR,8,GETDATE());
	SET @Receiver='';
	SET @EventType=0;
	SET @StationID='';
	SET @IsMotor=0;

	SELECT @CarNo=A.CarNo,
		   @CID=A.CID,
		   @SPEED=A.Speed,
		   @DoorStatus=ISNULL(A.DoorStatus,''),
		   @AccOn=A.ACCStatus,
		   @PowOn=ISNULL(A.PowerOnStatus,0),
		   @IsMotor=C.IsMotor
	FROM INSERTED A
	INNER JOIN TB_Car B ON B.CarNo=A.CarNo AND B.available<>2
	INNER JOIN TB_CarInfo C ON A.CarNo=C.CarNo;
	
	--收信者MAIL
	--要寄給營管中心的就設定在AlertEmail，用AlertEmail來判斷
	SELECT @Receiver=CONCAT(ISNULL(Station.AlertEmail,Station.[ManageStationID]),'@hotaimotor.com.tw;'),
		@StationID=Station.StationID
	FROM TB_iRentStation AS Station WITH(NOLOCK)
	INNER JOIN TB_Car AS Car WITH(NOLOCK) ON Car.nowStationID=Station.StationID AND Car.CarNo=@CarNo;

	--1.據點XXXX和X0XX不發
	--2.沒車號不發
	--3.沒收件者不發
	--4.沒據點代碼不發
	IF @StationID <> 'XXXX' AND @StationID <> 'X0XX' AND @CarNo <> '' AND ISNULL(@Receiver,'') <> '' AND ISNULL(@StationID,'') <> ''
	BEGIN
		SELECT @OrderNum=ISNULL(order_number,0) 
		FROM TB_OrderMain WITH (NOLOCK)
		WHERE CarNo=@CarNo
		AND (@NowTime BETWEEN DATEADD(MINUTE,-30,start_time) AND DATEADD(MINUTE,15,stop_time) )
		AND ( 
				((car_mgt_status>0 AND car_mgt_status<16) AND cancel_status=0 AND booking_status<5 )	--正常取車使用中
				OR 
				(booking_status = 1 AND cancel_status < 5 AND car_mgt_status <> 16)		--整備使用中
			);

		--20210323確認邏輯：時速、引擎、電門為一個群組
		--有時速就不要再寫引擎及電門事件(車子再走，必定要發動引擎及電門)
		--有引擎就不要再寫電門事件		(引擎發動，必定要啟動電門)

		--沒租約但是有時速(EventType:1)
		IF CONVERT(INT,@SPEED)>=20 
		BEGIN
			SET @EventType=1;

			IF ISNULL(@OrderNum,0)=0
			BEGIN
				INSERT INTO TB_EventHandle(EventType,MachineNo,CarNo,MKTime)VALUES(@EventType,@CID,@CarNo,@NowTime);
				INSERT INTO TB_AlertMailLog([EventType],[Receiver],[Sender],[HasSend],CarNo,[MKTime])
				VALUES(@EventType,@Receiver,'0',0,@CarNo,@NowTime);
			END
		END

		--車輛無租約但引擎被發動(EventType:9)
		-- 20210419;機車不發此項
		IF @AccOn=1 AND @EventType=0 AND @IsMotor=0
		BEGIN
			SET @EventType=9;

			IF ISNULL(@OrderNum,0)=0
			BEGIN
				INSERT INTO TB_EventHandle(EventType,MachineNo,CarNo,MKTime)VALUES(@EventType,@CID,@CarNo,@NowTime);
				INSERT INTO TB_AlertMailLog([EventType],[Receiver],[Sender],[HasSend],CarNo,[MKTime])
				VALUES(@EventType,@Receiver,'0',0,@CarNo,@NowTime);
			END
		END

		--車輛無租約但電門被啟動(EventType:8)
		IF @PowOn=1 AND @EventType=0
		BEGIN
			SET @EventType=8;

			IF ISNULL(@OrderNum,0)=0
			BEGIN
				INSERT INTO TB_EventHandle(EventType,MachineNo,CarNo,MKTime)VALUES(@EventType,@CID,@CarNo,@NowTime);
				INSERT INTO TB_AlertMailLog([EventType],[Receiver],[Sender],[HasSend],CarNo,[MKTime])
				VALUES(@EventType,@Receiver,'0',0,@CarNo,@NowTime);
			END
		END

		--車輛無租約但車門被打開(EventType:7)
		IF @DoorStatus='0000' 
		BEGIN
			SET @EventType=7;

			IF ISNULL(@OrderNum,0)=0
			BEGIN
				INSERT INTO TB_EventHandle(EventType,MachineNo,CarNo,MKTime)VALUES(@EventType,@CID,@CarNo,@NowTime);
				INSERT INTO TB_AlertMailLog([EventType],[Receiver],[Sender],[HasSend],CarNo,[MKTime])
				VALUES(@EventType,@Receiver,'0',0,@CarNo,@NowTime);
			END
		END
	END
END
GO

ALTER TABLE [dbo].[TB_CarStatus] ENABLE TRIGGER [TR_UpdateCarStatus]
GO

