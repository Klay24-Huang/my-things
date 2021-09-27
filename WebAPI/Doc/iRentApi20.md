版本: 1.0

# iRentApi2 WebAPI

iRentApi20 Web API版本

目錄

註冊相關

- [Register_Step2 設定密碼](#Register_Step2)
- [RegisterMemberData 設定會員資料](#RegisterMemberData)

登入相關

- [Login 登入](#Login)
- [RefrashToken 更新Token](#RefrashToken)
- [CheckAppVersion 檢查APP版本](#CheckAppVersion)
- [GetMemberStatus 取得會員狀態](#GetMemberStatus)

會員相關

- [GetMemberScore 取得會員積分](#GetMemberScore)
- [SetMemberScoreDetail 修改會員積分明細](#SetMemberScoreDetail)
- [GetMemberMedal 取得會員徽章](#GetMemberMedal)
- [SetMemberCMK 更新會員條款](#SetMemberCMK)
- [TransWebMemCMK 拋轉官網會員同意資料](#TransWebMemCMK)

首頁地圖相關

- [GetFavoriteStation取得常用站點](#GetFavoriteStation)
- [SetFavoriteStation設定常用站點](#SetFavoriteStation)
- [GetCarType同站以據點取出車型](#GetCarType)
- [GetProject取得專案與資費(同站)](#GetProject)
- [GetBanner 取得廣告資訊](#GetBanner)
- [GetNormalRent 取得同站租還站點](#GetNormalRent)
- [GetCarTypeGroupList取得車型清單](#GetCarTypeGroupList)
- [GetMapMedal 取得地圖徽章](#GetMapMedal)
- [AnyRent 取得路邊租還車輛](#AnyRent)
- [GetAnyRentProject 取得專案與資費(路邊)](#GetAnyRentProject)
- [MotorRent 取得路邊租還機車](#MotorRent)
- [GetMotorRentProject 取得專案與資費(機車)](#GetMotorRentProject)
- [GetPolygon 取得電子柵欄](#GetPolygon)

取還車跟車機操控相關

- [ChangeUUCard 變更悠遊卡](#ChangeUUCard)
- [GetPayDetail 取得租金明細](#GetPayDetail)
- [BookingStart 汽車取車](#BookingStart)
- [BookingStartMotor 機車取車](#BookingStartMotor)

月租訂閱制相關

- [GetMonthList   取得訂閱制月租列表](#GetMonthList)  
- [GetMonthGroup  訂閱制月租專案群組](#GetMonthGroup)  
- [BuyNow/AddMonth 月租購買](#BuyNowAddMonth)
- [BuyNow/UpMonth 月租升轉](#BuyNowUpMonth)
- [BuyNow/PayArrs 欠費繳交](#BuyNowPayArrs)
- [GetMySubs我的方案牌卡明細](#GetMySubs)
- [GetSubsCNT取得合約明細](#GetSubsCNT)
- [GetChgSubsList 變更下期續約列表](#GetChgSubsList)
- [GetUpSubsList 訂閱制升轉列表](#GetUpSubsList)
- [GetSubsHist 訂閱制歷史紀錄](#GetSubsHist)
- [GetSubsHist-del 訂閱制歷史紀錄-刪除](#GetSubsHist-del)
- [GetArrsSubsList 訂閱制欠費查詢](#GetArrsSubsList)
- [SetSubsNxt 設定自動續約](#SetSubsNxt)

預約以及訂單相關

- [OrderDetail 訂單明細](#OrderDetail)
- [Booking 預約](#Booking)
- [BookingQuery 訂單列表](#BookingQuery)
- [BookingFinishQuery 完成的訂單查詢](#BookingFinishQuery)
- [BookingDelete 刪除訂單](#BookingDelete)
- [GetOrderInsuranceInfo 訂單安心服務資格及價格查詢](#GetOrderInsuranceInfo)

車輛調度停車場

- [GetMotorParkingData 取得機車調度停車場](#GetMotorParkingData)
- [GetParkingData 取得汽車調度停車場](#GetParkingData)

電子錢包相關
- [CreditAndWalletQuery 查詢綁卡跟錢包](#CreditAndWalletQuery)
- [WalletStoreTradeTransHistory 錢包歷史紀錄查詢](#WalletStoreTradeTransHistory)
- [WalletStoreTradeHistoryHidden 錢包歷程-儲值交易紀錄隱藏](#WalletStoreTradeHistoryHidden)
- [GetWalletStoredMoneySet 錢包儲值-設定資訊](#GetWalletStoredMoneySet)
- [WalletStoredMoney 錢包儲值-信用卡](#WalletStoredMoney)
- [WalletStoreVisualAccount 錢包儲值-虛擬帳號](#WalletStoreVisualAccount)
- [WalletStoreShop 錢包儲值-商店條碼](#WalletStoreShop)
- [GetPayInfoReturnCar 還車付款-取得付款方式](#GetPayInfoReturnCar)
- [DoPayReturnCar 還車付款-執行付款方式](#DoPayReturnCar)
- [ChoseCheckoutModeShow 付款方式設定-顯示](#ChoseCheckoutModeShow)
- [GetPayInfoArrears 欠費繳交-付款方式-顯示](#GetPayInfoArrears)
- [DoPayArrears 欠費繳交-執行](#GetPayInfoArrears)
- [WalletTransferStoredValue 錢包轉贈](#WalletTransferStoredValue)

共同承租人機制

- [JointRentInviteeListQuery 共同承租人邀請清單查詢](#JointRentInviteeListQuery)
- [JointRentInviteeVerify 共同承租人邀請檢核](#JointRentInviteeVerify)
- [JointRentInvitation 案件共同承租人邀請](#JointRentInvitation)
- [JointRentInviteeModify 案件共同承租人邀請狀態維護](#JointRentInviteeModify)
- [JointRentIviteeFeedBack 案件共同承租人回應邀請](#JointRentIviteeFeedBack)

推播相關

- [News 活動通知](#News)
- [NewsRead 活動通知讀取](#NewsRead)
- [PersonNotice 個人訊息](#PersonNotice)
- [PersonNoticeRead 個人訊息讀取](#PersonNoticeRead)
----------
# 修改歷程

20210315 常用站點API修改

20210316 增加取得廣告資訊

20210317 修正API位置，區分正式及測試

20210322 新增登入、更新Token、取得會員狀態

20210324 增加地圖搜尋相關修改，有變動到的API清單為 GetNormalRent,GetCarType,GetProject

20210407 GetCarTypeGroupList補文件，移除掉部分API的Seats欄位

20210407 新增檢查APP版本、還原更新Token

20210415 新增變更悠遊卡

20210510 新增月租訂閱制相關

20210511 新增月租訂閱制相關(GetMySubs,GetSubsCNT)

20210517 GetMemberStatus增加會員積分相關欄位

20210518 訂單明細增加回饋明細

20210519 新增取得會員積分(GetMemberScore)

20210520 新增會員積分紀錄刪除(SetMemberScoreDetail)

20210521 新增取得會員徽章(GetMemberMedal)、取得地圖徽章(GetMapMedal)、修改取得會員積分(GetMemberScore)

20210524 取得會員徽章(GetMemberMedal)增加欄位

20210526 取得會員積分(GetMemberScore)、取得會員徽章(GetMemberMedal)欄位格式調整

20210526 補上設定自動續約

20210527 補上取得租金明細

20210528 新增車輛調度停車場相關API

20210601 取得會員徽章(GetMemberMedal)、取得地圖徽章(GetMapMedal)欄位值調整

20210609 補上預約

20210708 新增電子柵欄API

20210811 新增設定密碼(Register_Step2)

20210812 新增設定會員資料(RegisterMemberData)、取得會員狀態(GetMemberStatus)增加欄位

20210813 新增更新會員條款(SetMemberCMK)

20210818 新增拋轉官網會員同意資料(TransWebMemCMK)

20210819 新增電子錢包相關API

20210819 新增共同承租人邀請清單查詢API

20210819 共同承租人邀請檢核API

20210819 案件共同承租人邀請API

20210819 案件共同承租人邀請狀態維護API

20210819 案件共同承租人回應邀請API

20210819 歷史訂單明細(OrderDetail)增加欄位承租人類型(RenterType)

20210820 拋轉官網會員同意資料(TransWebMemCMK)欄位格式調整

20210820 補訂單清單查詢API

20210820 訂單清單查詢(BookingQuery)增加欄位承租人類型(RenterType)

20210820 補汽車取車API

20210820 汽車取車(BookingStart)增加欄位略過未回應的被邀請人(SkipNoFeedbackInvitees)

20210820 補機車取車API

20210820 機車取車(BookingStartMotor)增加欄位略過未回應的被邀請人(SkipNoFeedbackInvitees)

20210820 補完成的訂單查詢API

20210820 完成的訂單查詢(BookingFinishQuery)增加欄位是否為共同承租訂單(IsJointOrder)

20210820 補刪除訂單API

20210825 汽車取車(BookingStart)移除欄位略過未回應的被邀請人(SkipNoFeedbackInvitees)

20210825 機車取車(BookingStartMotor)移除欄位略過未回應的被邀請人(SkipNoFeedbackInvitees)

20210825 共同承租人邀請清單查詢(JointRentInviteeListQuery)新增欄位邀請時輸入的ID或手機(QueryId)

20210830 訂單列表(BookingQuery)欄位修正

20210831 共同承租人邀請(JointRentInvitation) 回應邀請(JointRentIviteeFeedBack ) 參數名稱&錯誤代碼更新

20210901 機車取車(BookingStartMotor) input修正

20210901 共同承租人回應邀請(JointRentIviteeFeedBack) 欄位值修正

20210907 增加推播相關

20210910 取得會員狀態(GetMemberStatus)增加是否顯示購買牌卡

# Header參數相關說明
| KEY | VALUE |
| -------- | -------- |
| Authorization | Bearer AccessToken |
| Content-Type | application/json |

# 共用錯誤代碼
| 錯誤代碼 | 說明 |
| ------- | ------- |
| ERR100 | 帳號或密碼錯誤 |
| ERR101 | 請重新登入 |
| ERR103 | 身份證格式不符 |
| ERR104 | APP版號錯誤 |
| ERR105 | APP格式錯誤 |
| ERR150 | 此功能需登入後才能使用 |
| ERR900 | 參數遺漏(必填參數遺漏) |
| ERR901 | 參數遺漏(未傳入參數) |
| ERR902 | 參數遺漏(格式不符) |



# 註冊相關

## Register_Step2 設定密碼

### [/api/Register_Step2/]

- 20210811發佈

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

- 傳送跟接收採JSON格式

- 動作 [POST]

- Input 傳入參數說明

| 參數名稱   | 參數說明                 | 必要 |  型態  | 範例                                                         |
| ---------- | ------------------------ | :--: | :----: | ------------------------------------------------------------ |
| IDNO       | 帳號                     |  Y   |  int   | A123456789                                                   |
| PWD        | 密碼                     |  Y   | string | 0xe3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855 |
| DeviceID   | 機碼                     |  Y   | string | 170f2199f13bf36bcb6bf85a3e1c19c99                            |
| app        | APP類型(0:Android 1:iOS) |  Y   |  int   | 0                                                            |
| appVersion | APP版號                  |  Y   | string | 5.10.0                                                       |

* Input範例

```
{
    "IDNO": "A123456789",
    "DeviceID": "170f2199f13bf36bcb6bf85a3e1c19c99",
    "PWD": "0xe3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
    "app": 0,
    "appVersion": "5.10.0"
}
```

* Output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```



## RegisterMemberData 設定會員資料

### [/api/RegisterMemberData/]

- 20210812發佈

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

- 傳送跟接收採JSON格式

- 動作 [POST]

- Input 傳入參數說明

| 參數名稱   | 參數說明                 | 必要 |  型態  | 範例                              |
| ---------- | ------------------------ | :--: | :----: | --------------------------------- |
| IDNO       | 帳號                     |  Y   |  int   | A123456789                        |
| DeviceID   | 機碼                     |  Y   | string | 170f2199f13bf36bcb6bf85a3e1c19c99 |
| app        | APP類型(0:Android 1:iOS) |  Y   |  int   | 0                                 |
| appVersion | APP版號                  |  Y   | string | 5.10.0                            |
| MEMCNAME   | 姓名                     |  Y   | string | 花蓮咘                            |
| MEMBIRTH   | 生日                     |  Y   | string | 2020-05-03                        |
| AreaID     | 行政區ID                 |  Y   |  int   | 347                               |
| MEMADDR    | 會員住址                 |  Y   | string | 中山路276號                       |
| MEMEMAIL   | 會員email                |  Y   | string | bubu@hotaimotor.com.tw            |
| Signture   | 電子簽名（Base64)        |  Y   | string |                                   |

* Input範例

```
{
    "IDNO": "A123456789",
    "DeviceID": "170f2199f13bf36bcb6bf85a3e1c19c99",
    "app": 0,
    "appVersion": "5.10.0",
    "MEMCNAME": "花蓮咘",
    "MEMBIRTH": "2020-05-03",
    "AreaID": 347,
    "MEMADDR": "中山路276號",
    "MEMEMAIL": "bubu@hotaimotor.com.tw",
    "Signture": "/9j/4AAQSkZJRgABAQEAeAB4AAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAB+AKADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/"
}
```

* Output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```



# 登入相關

##  Login 登入 

### [/api/Login/]

* 20210322發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                       | 必要 |  型態  | 範例                                                         |
| ---------- | ------------------------------ | :--: | :----: | ------------------------------------------------------------ |
| IDNO       | 帳號                           |  Y   | string | A123456789                                                   |
| PWD        | 密碼                           |  Y   | string | 51158202974E9174B6390D0DB832168B2BCB024E05505095FAA562F91DBC75AF |
| DeviceID   | DeviceID                       |  Y   | string | 171DD37E-E20A-4281-94C9-3DA48AAAAA                           |
| APP        | APP類型<br />(0:Android 1:iOS) |  Y   |  int   | 1                                                            |
| APPVersion | APP版號                        |  Y   | string | 5.6.0                                                        |
| PushREGID  | 推播註冊流水號                 |  Y   | string | 123456                                                       |

- input範例

```
{
    "IDNO": "A123456789",
    "PWD": "51158202974E9174B6390D0DB832168B2BCB024E05505095FAA562F91DBC75AF",
    "DeviceID": "171DD37E-E20A-4281-94C9-3DA48AAAAA",
    "app": 1,
    "appVersion": "5.4.0",
    "PushREGID": 0
}
```

* output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |
| Token        | Token列表          |  List  |               |
| UserData     | 會員資料列表       |  List  |               |

* Token 參數說明

| 參數名稱           | 參數說明                 |  型態  | 範例                                                         |
| ------------------ | ------------------------ | :----: | ------------------------------------------------------------ |
| Access_token       | Token                    | string | B832168B2BCB024E05505095FAA562F91DBC75AFC70852185932582751158202 |
| Refrash_token      | Refrash Token            | string | 51158202974E9174B6390D0DB832168B2BCB024E05505095FAA562F91DBC75AF |
| Rxpires_in         | 有效期限(單位秒)         |  int   | 86400                                                        |
| Refrash_Rxpires_in | Refrash 有效期限(單位秒) |  int   | 604800                                                       |

* UserData 參數說明

| 參數名稱       | 參數說明                                                     |  型態  | 範例                                                         |
| -------------- | ------------------------------------------------------------ | :----: | ------------------------------------------------------------ |
| MEMIDNO        | 帳號                                                         | string | A123456789                                                   |
| MEMCNAME       | 姓名                                                         | string | 王OX                                                         |
| MEMTEL         | 電話                                                         | string | 0912345678                                                   |
| MEMHTEL        | 連絡電話(住家)                                               | string | 25046290                                                     |
| MEMBIRTH       | 生日                                                         | string | 1990-03-22                                                   |
| MEMAREAID      | 城市                                                         |  int   | 53                                                           |
| MEMADDR        | 地址                                                         | string | 中山北路                                                     |
| MEMEMAIL       | 信箱                                                         | string | irent@gmail.com                                              |
| MEMCOMTEL      | 公司電話                                                     | string | 25046290                                                     |
| MEMCONTRACT    | 緊急連絡人                                                   | string | 王XO                                                         |
| MEMCONTEL      | 緊急連絡人電話(手機)                                         | string | 0987654321                                                   |
| MEMMSG         | 活動及優惠訊息通知 (Y:是 N:否)                               | string | N                                                            |
| CARDNO         | 卡號                                                         | string | 459863745                                                    |
| UNIMNO         | 統編                                                         | string | 03089008                                                     |
| MEMSENDCD      | 發票寄送方式<br />1:捐贈;2:email;3:二聯;4:三聯;5:手機條碼;6:自然人憑證 |  int   | 5                                                            |
| CARRIERID      | 發票載具                                                     | string | /N37H2JD                                                     |
| NPOBAN         | 愛心碼                                                       | string | 885                                                          |
| HasCheckMobile | 是否通過手機驗證(0:否;1:是)                                  |  int   | 1                                                            |
| NeedChangePWD  | 是否需重新設定密碼(0:否;1:是)                                |  int   | 1                                                            |
| HasBindSocial  | 是否綁定社群(0:否;1:是)                                      |  int   | 0                                                            |
| HasVaildEMail  | 是否已驗證EMAIL(0:否;1:是)                                   |  int   | 1                                                            |
| Audit          | 審核狀態 0:未審核 1:審核通過 2:審核不通過                    |  int   | 1                                                            |
| IrFlag         | 目前註冊進行至哪個步驟<br />-1驗證完手機 、0：設置密碼、1：其他 |  int   | 1                                                            |
| PayMode        | 付費方式 0:信用卡                                            |  int   | 0                                                            |
| RentType       | 可租車類別<br />0:無法;1:汽車;2:機車;3:全部                  |  int   | 3                                                            |
| ID_pic         | 身份證                                                       |  int   | 0                                                            |
| DD_pic         | 汽車駕照                                                     |  int   | 0                                                            |
| MOTOR_pic      | 機車駕照                                                     |  int   | 0                                                            |
| AA_pic         | 自拍照                                                       |  int   | 2                                                            |
| F01_pic        | 法定代理人                                                   |  int   | 0                                                            |
| Signture_pic   | 電子簽名                                                     |  int   | 2                                                            |
| SigntureCode   | 電子簽名URL                                                  | string | https://irentv2data.blob.core.windows.net/credential/A123456789_Signture_20201221153957.png |
| MEMRFNBR       | 短租會員IR+流水號                                            | string | ir609412                                                     |
| SIGNATURE      | 短租網站電子簽名                                             | string | http://iRent.iRentCar.com.tw/iMoto_BackEnd/SigntureHelper/Index?IDNO=A123456789&signTime=2020-06-09 18:37:34 |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "Token": {
            "Access_token": "B832168B2BCB024E05505095FAA562F91DBC75AFC70852185932582751158202",
            "Refrash_token": "51158202974E9174B6390D0DB832168B2BCB024E05505095FAA562F91DBC75AF",
            "Rxpires_in": 86400,
            "Refrash_Rxpires_in": 604800
        },
        "UserData": {
            "MEMIDNO": "A123456789",
            "MEMCNAME": "王OX",
            "MEMTEL": "0912345678",
            "MEMHTEL": "",
            "MEMBIRTH": "1990-03-22",
            "MEMAREAID": 53,
            "MEMADDR": "中山北路",
            "MEMEMAIL": "irent@gmail.com",
            "MEMCOMTEL": "",
            "MEMCONTRACT": "王XO",
            "MEMCONTEL": "0987654321",
            "MEMMSG": "N",
            "CARDNO": "",
            "UNIMNO": "",
            "MEMSENDCD": 5,
            "CARRIERID": "/N37H2JD",
            "NPOBAN": "",
            "HasCheckMobile": 1,
            "NeedChangePWD": 1,
            "HasBindSocial": 0,
            "HasVaildEMail": 1,
            "Audit": 1,
            "IrFlag": 1,
            "PayMode": 0,
            "RentType": 3,
            "ID_pic": 0,
            "DD_pic": 0,
            "MOTOR_pic": 0,
            "AA_pic": 2,
            "F01_pic": 0,
            "Signture_pic": 2,
            "SigntureCode": "https://irentv2data.blob.core.windows.net/credential/A123456789_Signture_20201221153957.png",
            "MEMRFNBR": "ir609412",
            "SIGNATURE": "http://iRent.iRentCar.com.tw/iMoto_BackEnd/SigntureHelper/Index?IDNO=A123456789&signTime=2020-06-09 18:37:34"
        }
    }
}
```

-------

## RefrashToken 更新Token
### [/api/RefrashToken/]

* 20210322發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱     | 參數說明                       | 必要 |  型態  | 範例                                                         |
| ------------ | ------------------------------ | :--: | :----: | ------------------------------------------------------------ |
| IDNO         | 帳號                           |  Y   | string | A123456789                                                   |
| RefrashToken | Refrash Token                  |  Y   | string | 51158202974E9174B6390D0DB832168B2BCB024E05505095FAA562F91DBC75AF |
| DeviceID     | DeviceID                       |  Y   | string | 171DD37E-E20A-4281-94C9-3DA48AAAAA                           |
| APP          | APP類型<br />(0:Android 1:iOS) |  Y   |  int   | 1                                                            |
| APPVersion   | APP版號                        |  Y   | string | 5.6.0                                                        |
| PushREGID    | 推播註冊流水號                 |  Y   | string | 123456                                                       |

* input範例

```
{
    "IDNO": "A123456789",
    "RefrashToken": "51158202974E9174B6390D0DB832168B2BCB024E05505095FAA562F91DBC75AF",
    "APP": 1,
    "APPVersion": "5.6.0",
    "DeviceID": "171DD37E-E20A-4281-94C9-3DA48AAAAA",
    "PushREGID": 123456
}
```

* output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |
| Token        | Token列表          |  List  |               |

* Token 參數說明

| 參數名稱           | 參數說明                 |  型態  | 範例                                                         |
| ------------------ | ------------------------ | :----: | ------------------------------------------------------------ |
| Access_token       | Token                    | string | B832168B2BCB024E05505095FAA562F91DBC75AFC70852185932582751158202 |
| Refrash_token      | Refrash Token            | string | 51158202974E9174B6390D0DB832168B2BCB024E05505095FAA562F91DBC75AF |
| Rxpires_in         | 有效期限(單位秒)         |  int   | 86400                                                        |
| Refrash_Rxpires_in | Refrash 有效期限(單位秒) |  int   | 604800                                                       |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "Token": {
            "Access_token": "B832168B2BCB024E05505095FAA562F91DBC75AFC70852185932582751158202",
            "Refrash_token": "51158202974E9174B6390D0DB832168B2BCB024E05505095FAA562F91DBC75AF",
            "Rxpires_in": 86400,
            "Refrash_Rxpires_in": 604800
        }
    }
}
```

------



## CheckAppVersion 檢查APP版本
### [/api/CheckAppVersion/]

* 20210407發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

*  動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                 | 必要 |  型態  | 範例                               |
| ---------- | ------------------------ | :--: | :----: | ---------------------------------- |
| DeviceID   | DeviceID                 |  Y   | string | 171DD37E-E20A-4281-94C9-3DA48AAAAA |
| APP        | APP類型(0:Android 1:iOS) |  Y   |  int   | 1                                  |
| APPVersion | APP版號                  |  Y   | string | 5.6.0                              |

* input範例

```
{
    "DeviceID": "171DD37E-E20A-4281-94C9-3DA48AAAAA",
    "APP": 1,
    "APPVersion": "5.6.0"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Result       | 是否成功                |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼                  | string | 000000        |
| NeedRelogin  | 是否需重新登入          |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新      |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息                | string | Success       |
| Data         | 資料物件                |        |               |
| MandatoryUPD | 強制更新 (1=強更，0=否) |  int   | 1             |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "MandatoryUPD": 1
    }
}
```

------



## GetMemberStatus 取得會員狀態
### [/api/GetMemberStatus/]

* 20210322發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| 無參數   |          |      |      |      |

* output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |
| StatusData   | 會員狀態列表       |  List  |               |

* StatusData 參數說明

| 參數名稱        | 參數說明                                                     |  型態  | 範例       |
| --------------- | ------------------------------------------------------------ | :----: | ---------- |
| MEMIDNO         | 身分證號                                                     | string | A123456789 |
| MEMNAME         | 姓名                                                         | string | 王曉明     |
| Login           | 登入狀態 Y/N                                                 | string | Y          |
| Register        | 註冊是否完成 0:未完成 1:已完成                               |  int   | 1          |
| Audit           | 審核結果 是否通過審核(0:未審;1:已審;2:審核不通過)            |  int   | 1          |
| Audit_ID        | 審核身分證 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)        |  int   | 2          |
| Audit_Car       | 審核汽車駕照 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)      |  int   | 2          |
| Audit_Motor     | 審核機車駕照 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)      |  int   | 2          |
| Audit_Selfie    | 審核自拍照 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)        |  int   | 2          |
| Audit_F01       | 審核法定代理人 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)    |  int   | 0          |
| Audit_Signture  | 審核簽名檔 (0:未上傳 -1:審核失敗 1:審核中 2:審核完成)        |  int   | 2          |
| BlackList       | 黑名單 Y/N                                                   | string | N          |
| MenuCTRL        | 會員頁9.0卡狀態 (0:PASS 1:未完成註冊 2:完成註冊未上傳照片 3:身分審核中 4:審核不通過 5:身分變更審核中 6:身分變更審核失敗) |  int   | 0          |
| MenuStatusText  | 會員頁9.0狀態顯示 (這邊要通過審核才會有文字 MenuCTRL5 6才會有文字提示) | string |            |
| StatusTextCar   | 狀態文字說明                                                 | string |            |
| StatusTextMotor | 機車狀態文字說明                                             | string |            |
| NormalRentCount | 目前汽車出租數                                               |  int   | 0          |
| AnyRentCount    | 目前路邊出租數                                               |  int   | 0          |
| MotorRentCount  | 目前路邊出租數                                               |  int   | 0          |
| TotalRentCount  | 目前全部出租數                                               |  int   | 0          |
| Score           | 會員積分                                                     |  int   | 100        |
| BlockFlag       | 停權等級 (0:無 1:暫時停權 2:永久停權)                        |  int   | 0          |
| BLOCK_EDATE     | 停權截止日                                                   | string | 2021/06/01 |
| CMKStatus       | 會員條款狀態 (Y:重新確認 N:不需重新確認)                     | string | Y          |
| IsShowBuy       | 是否顯示購買牌卡 (Y:是 N:否)                                 | string | Y          |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "StatusData": {
            "MEMIDNO": "A123456789",
            "MEMNAME": "王曉明",
            "Login": "Y",
            "Register": 1,
            "Audit": 1,
            "Audit_ID": 2,
            "Audit_Car": 2,
            "Audit_Motor": 2,
            "Audit_Selfie": 2,
            "Audit_F01": 0,
            "Audit_Signture": 2,
            "BlackList": "N",
            "MenuCTRL": 0,
            "MenuStatusText": "",
            "StatusTextCar": "",
            "StatusTextMotor": "",
            "NormalRentCount": 0,
            "AnyRentCount": 0,
            "MotorRentCount": 0,
            "TotalRentCount": 0,
            "Score": 100,
            "BlockFlag": 0,
            "BLOCK_EDATE": "",
            "CMKStatus": "Y",
            "IsShowBuy": "Y"
        }
    }
}
```



# 會員相關

## GetMemberScore 取得會員積分

### [/api/GetMemberScore/]

- 20210519發佈

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**


* 動作 [POST]
* input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例                |
| -------- | -------- | :--: | :--: | ------------------- |
| NowPage  | 目前頁數 |      | int  | 1 (可不輸入，預帶1) |

* input範例

```
{
    "NowPage": 1
}
```

* output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |
| Score        | 會員積分           |  int   | 100           |
| TotalPage    | 總頁數             |  int   | 1             |
| DetailList   | 積分歷程           |  List  |               |

* DetailList 參數說明

| 參數名稱   | 參數說明     |   型態   | 範例                |
| ---------- | ------------ | :------: | ------------------- |
| TotalCount | 總筆數       |   int    | 61                  |
| RowNo      | 編號         |   int    | 1                   |
| GetDate    | 取得日期     | DateTime | 2021-05-19T13:37:03 |
| SEQ        | 序號         |   int    | 103                 |
| SCORE      | 分數         |   int    | -50                 |
| UIDESC     | 用戶畫面敘述 |  string  | 天佑台灣            |
| ORDERNO    | 訂單編號     |  string  | H10365010           |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "Score": 100,
        "TotalPage": 6,
        "DetailList": [
            {
                "TotalCount": 58,
                "RowNo": 1,
                "GetDate": "2021-05-19T13:37:03",
                "SEQ": 103,
                "SCORE": -50,
                "UIDESC": "天佑台灣",
                "ORDERNO": "H0"
            },
            {
                "TotalCount": 58,
                "RowNo": 2,
                "GetDate": "2021-05-10T11:00:00",
                "SEQ": 101,
                "SCORE": 1,
                "UIDESC": "單次租用",
                "ORDERNO": "H10365010"
            },
            {
                "TotalCount": 58,
                "RowNo": 3,
                "GetDate": "2021-05-05T17:12:53",
                "SEQ": 105,
                "SCORE": -5,
                "UIDESC": "預約前三小時取消(預約<10小時)",
                "ORDERNO": "H10239531"
            }
        ]
    }
}
```



## SetMemberScoreDetail 修改會員積分明細

### [/api/SetMemberScoreDetail/]

- 20210519發佈

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**


* 動作 [POST]
* input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| SEQ      | 序號     |  Y   | int  | 1    |

* input範例

```
{
    "SEQ": 113
}
```

* output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```



## GetMemberMedal 取得會員徽章

### [/api/GetMemberMedal/]

- 20210521發佈

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**


* 動作 [POST]
* input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| 無參數   |          |      |      |      |

* output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |
| MedalList    | 徽章明細           |  List  |               |

* MedalList 參數說明

| 參數名稱      | 參數說明                   |  型態  | 範例                |
| ------------- | -------------------------- | :----: | ------------------- |
| MileStone     | 徽章代碼                   | string | newhand             |
| MileStoneName | 徽章名稱                   | string | 新手上路            |
| Norm          | 門檻指標                   |  int   | 1                   |
| Progress      | 目前進度                   |  int   | 1                   |
| Describe      | APP顯示的描述              | string | 通過會員審核        |
| GetFlag       | 是否獲得 (1:獲得 0:未獲得) |  int   | 1                   |
| GetMedalTime  | 徽章獲得時間               | string | 2021-05-20T14:27:45 |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "MedalList": [
            {
                "MileStone": "newhand",
                "MileStoneName": "新手上路",
                "Norm": 1,
                "Progress": 1,
                "Describe": "通過會員審核",
                "GetFlag": 1,
                "GetMedalTime": "2021-05-20T14:27:45"
            },
            {
                "MileStone": "guidebook",
                "MileStoneName": "有所作為",
                "Norm": 11,
                "Progress": 0,
                "Describe": "完成小學堂",
                "GetFlag": 0,
                "GetMedalTime": ""
            }
        ]
    }
}
```



## SetMemberCMK 更新會員條款

### [/api/SetMemberCMK/]

- 20210813發佈

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**


* 動作 [POST]
* Input 傳入參數說明

| 參數名稱  | 參數說明                       | 必要 |  型態  | 範例 |
| --------- | ------------------------------ | :--: | :----: | ---- |
| CHKStatus | 是否同意 (Y：同意 / N：不同意) |  Y   | string | Y    |

* Input範例

```
{
    "CHKStatus": "Y"
}
```

* Output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```



## TransWebMemCMK 拋轉官網會員同意資料

### [/api/TransWebMemCMK/]

- 20210818發佈

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

- 傳送跟接收採JSON格式

* 動作 [POST]
* Input 傳入參數說明

| 參數名稱  | 參數說明                       | 必要 |  型態  | 範例                                              |
| --------- | ------------------------------ | :--: | :----: | ------------------------------------------------- |
| IDNO      | 身分證字號                     |  Y   | string | A123456789                                        |
| VerType   | 同意書版本類型                 |  Y   | string | Hims                                              |
| Version   | 同意書版本號                   |  Y   | string | 100                                               |
| Source    | 同意來源管道 (I:IRENT W:官網)  |  Y   | string | W                                                 |
| AgreeDate | 同意時間                       |  Y   | string | 2021-08-17 14:45:21 <br>格式：yyyy-MM-dd HH:mm:ss |
| TEL       | 電話通知狀態 (N:不通知 Y:通知) |  Y   | string | Y                                                 |
| SMS       | 簡訊通知狀態 (N:不通知 Y:通知) |  Y   | string | Y                                                 |
| EMAIL     | EMAIL通知 (N:不通知 Y:通知)    |  Y   | string | Y                                                 |
| POST      | 郵寄通知 (N:不通知 Y:通知)     |  Y   | string | Y                                                 |

* Input範例

```
{
    "IDNO": "A123456789",
    "VerType": "Hims",
    "Version": "100",
    "Source": "W",
    "AgreeDate": "2021-08-17 14:41:53",
    "TEL": "Y",
    "SMS": "Y",
    "EMAIL": "Y",
    "POST": "Y"
}
```

* Output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```



# 首頁地圖相關

## GetFavoriteStation取得常用站點
### [/api/GetFavoriteStation/]

* 20210315發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**


* 動作 [POST]
  
* input傳入參數說明

| 參數名稱  | 參數說明 | 必要 |  型態  | 範例        |
| --------- | -------- | :--: | :----: | ----------- |
| 無參數 | | | |

* output回傳參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| Result | 是否成功 | int | 0:失敗 1:成功  |
| ErrorCode | 錯誤碼 | string | 000000 |
| NeedRelogin | 是否需重新登入 | int | 0:否 1:是 |
| NeedUpgrade | 是否需要至商店更新 | int | 0:否 1:是 |
| ErrorMessage | 錯誤訊息 | string | Success |
| Data | 資料物件 | | |
| FavoriteObj   | 常用站點列表 |  List  |  |

* FavoriteObj 參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| StationID | 據點代碼 | string | X0II |
| StationName | 據點名稱 | string | iRent濱江旗艦站 |
| Tel | 電話 | string | 0912345678 |
| ADDR | 地址 | string | 松江路557號 |
| Latitude | 緯度 | float | 25.069014 |
| Longitude | 經度 | float | 121.533842 |
| Content | 其他說明 | string | |
| ContentForAPP | 據點描述 | string | |
| IsRequiredForReturn | 還車位置資訊必填 | int | |
| StationPic | 據點照片 | List | 若空值則為null |

* StationPic 參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| StationPic | 據點照片URL位置 | string | |
| PicDescription | 據點說明 | string |  |

* Output範例
```Output範例
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "FavoriteObj": [
            {
                "StationID": "X0II",
                "StationName": "iRent濱江旗艦站",
                "Tel": "02-2516-3816",
                "ADDR": "松江路557號",
                "Latitude": 25.069014,
                "Longitude": 121.533842,
                "Content": "",
                "ContentForAPP": null,
                "IsRequiredForReturn": 0,
                "StationPic": null
            }
        ]
    }
}
```
---------------



## SetFavoriteStation設定常用站點
### [/api/SetFavoriteStation/]

* 20210315發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]
  
* input傳入參數說明

| 參數名稱  | 參數說明 | 必要 |  型態  | 範例        |
| --------- | -------- | :--: | :----: | ----------- |
| FavoriteStations | 常用據點清單 | Y | List | |


* FavoriteStations 參數說明

| 參數名稱  | 參數說明 | 必要 |  型態  | 範例        |
| --------- | -------- | :--: | :----: | ----------- |
| StationID | 據點代碼 | Y | string | X0II |
| Mode | 模式 | Y | int | 0:移除 1:設定 |

* input範例
```input
{
    "FavoriteStations": [
        {
            "StationID": "X0II",
            "Mode": 1
        },
        {
            "StationID": "X0I9",
            "Mode": 0
        }
    ]
}
```

* output回傳參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| Result | 是否成功 | int | 0:失敗 1:成功  |
| ErrorCode | 錯誤碼 | string | |
| ErrorMessage | 錯誤訊息 | string | |
| NeedRelogin | 是否需重新登入 | int | 0:否 1:是 |
| NeedUpgrade | 是否需要至商店更新 | int | 0:否 1:是 |
| Data | 資料物件 | |  |


* output範例
```output範例
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```
* 錯誤代碼清單

| 錯誤代碼 | 說明 |
| ------- | ------- |
| ERR147 | 您並未設定此常用站點 |
| ERR148 | 您已有設定此常用站點 |
| ERR149 | 此站點不存在或已停用 |


----------------

## GetCarType同站以據點取出車型

### [/api/GetCarType/]

* 20210315修改 - 增加是否為常用據點欄位

* 20210324修改 - 增加搜尋使用欄位CarTypes,Seats 

* 20210407修改 - input移除掉Seats 

* 20210408修改 - 增加IsRent欄位

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(可不填)**

* 動作 [POST]
  
* input傳入參數說明

| 參數名稱  | 參數說明 | 必要 |  型態  | 範例        |
| --------- | -------- | :--: | :----: | ----------- |
| StationID | 據點代碼 | Y | string | X0II |
| SD | 預計取車時間 | N | string | 2021-03-01 00:00:00 |
| ED | 預計還車時間 | N | string | 2021-03-01 23:59:59 |
| ☆CarTypes | 車型代碼 | N | Array string | [ "PRIUSC" ]|


* input範例
```input範例
{
    "StationID": "X0II",
	"CarTypes":[ "PRIUSC" ]
}
```

* output回傳參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| Result | 是否成功 | int | 0:失敗 1:成功  |
| ErrorCode | 錯誤碼 | string | |
| ErrorMessage | 錯誤訊息 | string | |
| NeedRelogin | 是否需重新登入 | int | 0:否 1:是 |
| NeedUpgrade | 是否需要至商店更新 | int | 0:否 1:是 |
| Data | 資料物件 | | |
| IsFavStation | 是否為常用據點 | int | 0:否 1:是 |
| GetCarTypeObj | 車型牌卡清單 | List |  |

* GetCarTypeObj回傳參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| CarBrend | 車子品牌 | string | TOYOTA |
| CarType | 車型代碼 | string | PRIUSC |
| CarTypeName | 車型名稱 | string | TOYOTA PRIUSc |
| CarTypePic | 車型圖片 | string | priusC |
| Operator | 業者icon | string | supplierIrent |
| OperatorScore | 業者評分 | Float | 5.0 |
| Price | 價格 | int | 168 |
| Seat | 座位數 | int | 5 |
| IsRent | 是否可出租 | string | Y/N|

* output範例
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "IsFavStation": 0,
        "GetCarTypeObj": [
            {
                "CarBrend": "TOYOTA",
                "CarType": "PRIUSC",
                "CarTypeName": "TOYOTA PRIUSc",
                "CarTypePic": "priusC",
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "Price": 168,
                "Seat": 5,
                "IsRent": "Y"
            },
            {
                "CarBrend": "TOYOTA",
                "CarType": "SIENTA5",
                "CarTypeName": "TOYOTA SIENTA5人",
                "CarTypePic": "sienta",
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "Price": 168,
                "Seat": 5,
                "IsRent": "N"
            },
            {
                "CarBrend": "TOYOTA",
                "CarType": "SIENTA7",
                "CarTypeName": "TOYOTA SIENTA7人",
                "CarTypePic": "sienta",
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "Price": 168,
                "Seat": 7,
                "IsRent": "Y"
            }
        ]
    }
}
```
* 錯誤代碼清單

| 錯誤代碼 | 說明 |
| ------- | ------- |
| ERR151 | 起始日期格式不符 |
| ERR152 | 結束日期格式不符 |
| ERR153 | 起始時間大於結束時間 |

----------------


## GetProject取得專案與資費(同站)
### [/api/GetProject/]

* 20210315修改 - 增加是否為常用據點欄位
* 20210324修改 - 增加搜尋欄位 
* 20210407修改 - input移除掉Seats 

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]
  
* input傳入參數說明

| 參數名稱  | 參數說明 | 必要 |  型態  | 範例        |
| --------- | -------- | :--: | :----: | ----------- |
| StationID | 據點代碼 | Y | string | X0II |
| CarType	| 車型代碼 | N | string | PRIUSC |
| SDate		| 預計取車時間 | Y | string | 2021-06-10 00:00:00 |
| EDate		| 預計還車時間 | Y | string | 2021-06-11 00:00:00 |
| Mode		| 顯示方式(0:依據點 1:依經緯度)	| Y | int | 0 |
| Latitude | 緯度(Mode=1必填) | | double | |
| Longitude | 經度(Mode=1必填) | | double | |
| Radius 	| 半徑(Mode=1必填) | | double | 0 |
| Insurance | 是否使用安心服務 | Y | int | 0 |
| CarTypes | 車型代碼 | N | array string | [ "PRIUSC" ] |
| Seats		| 座椅數 | N |  array int | [ 4 ] |

* input範例
```
{
    "CarTypes": [ "PRIUSC" ],
    "StationID": "X0II",
    "SDate": "2021-03-12 14:00:00",
    "EDate": "2021-03-13 15:00:00",
	"Latitude": "",
    "Radius": "0",
    "Mode": "0",
    "Longitude": "",
    "Insurance": 0
}
```

* output回傳參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| Result | 是否成功 | int | 0:失敗 1:成功  |
| ErrorCode | 錯誤碼 | string | 000000 |
| ErrorMessage | 錯誤訊息 | string | Success |
| NeedRelogin | 是否需重新登入 | int | 0:否 1:是 |
| NeedUpgrade | 是否需要至商店更新 | int | 0:否 1:是 |
| Data | 資料物件 | | |
| HasRentCard	| 是否有可租的卡片<br>無用欄位 | boolean | false |
| GetProjectObj | 回傳清單 | List | |


* GetProjectObj參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| StationID | 據點代碼 | string | X0II |
| StationName | 據點名稱 | string | iRent濱江旗艦站 |
| CityName | 縣市 | string | 台北市 |
| AreaName | 行政區 | string | 中山區 |
| ADDR | 地址 | string | 台北市中山區松江路557號 |
| Latitude | 緯度 | float | 25.069014 |
| Longitude | 經度 | float | 121.533842 |
| Content | 其他說明 | string | |
| ContentForAPP | 據點描述(app顯示) | string | |
| Mininum | 最低價 | int | 168 |
| IsRent | 是否有車可租(BY據點) | string | Y/N |
| IsFavStation | 是否為常用據點 | int | 0:否 1:是 |
| IsShowCard | 是否顯示牌卡 | int | 0:否 1:是 |
| ProjectObj | 專案清單 | List | |
| StationInfoObj | 站點照片 | List |  |


* ProjectObj參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| StationID | 據點代碼 | string | X0II |
| ProjID | 專案代碼 | string | P735 |
| ProjName | 專案名稱 | string | 同站汽車99推廣專案 |
| ProDesc | 優惠專案描述 | string | 1.本專案限iRent會員使用。... |
| CarBrend | 車輛廠牌 | string | TOYOTA |
| CarType | 車型代碼 | string | PRIUSC |
| CarTypeName | 車型名稱 | string | TOYOTA PRIUSc |
| CarTypePic | 車型圖片 | string | priusC |
| Seat | 座位數 | int | 5 |
| Operator | 業者ICON | string | supplierIrent |
| OperatorScore | 業者評分 | float | 5.0 |
| Insurance | 是否可加購安心服務 | int | 0:否 1:是 |
| InsurancePerHour | 加購安心服務每小時費用 | int | 50 |
| IsMinimum | 是否為最低價 | int | 0:否 1:是 |
| Price | 預估費用 | int | 2000 |
| WorkdayPerHour | 平日每小時金額 | int | 99 |
| HolidayPerHour | 假日每小時金額 | int | 168 |
| CarOfArea | 站別類型 | string | 同站 |
| Content | 其他備註 | string | ... |
| IsRent | 是否可租 | string | Y/N |
| IsFavStation | 是否為常用據點 | int | 0:否 1:是 |
| IsShowCard | 是否顯示牌卡 | int | 0:否 1:是 |
| MonthlyRentId | 訂閱制月租ID			| int | 1234 |
| CarWDHours	| 汽車平日時數         | double | 3.0    |
| CarHDHours	| 汽車假日時數         | double | 3.0      |
| MotoTotalMins	| 機車不分平假日分鐘數 | int | 300   |
| WDRateForCar	| 汽車平日優惠費率 | double | 99.0 |
| HDRateForCar	| 汽車假日優惠費率 | double | 168.0 |
| WDRateForMoto	| 機車平日優惠費率 | double | 1.0 |
| HDRateForMoto	| 機車假日優惠費率 | double | 1.2 |
| MonthStartDate | 月租全期(開始日) | string | 2021/05/21 |
| MonthEndDate | 月租全期(結束日) | string | 2021/05/21 |



* StationInfoObj參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| StationPic | 據點照片URL位置 | string | |
| PicDescription | 據點說明 | string | |


* output變數

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
		"HasRentCard": false,
        "GetProjectObj": [
            {
                "StationID": "X0II",
                "StationName": "iRent濱江旗艦站",
                "CityName": "台北市",
                "AreaName": "中山區",
                "ADDR": "台北市中山區松江路557號",
                "Latitude": 25.069014,
                "Longitude": 121.533842,
                "Content": "",
                "ContentForAPP": "1.松江路直走過民族東路約20公尺右手邊停車場\n2.固定車位(有塗iRent地漆)\n3.自由進出\n4.請勿停在專屬車位外\n5.門口無車位可再往裡面車位停放\n",
                "Minimum": 1774,
                "IsRent": "Y",
                "IsFavStation": 0,
				"IsShowCard": 0,
                "ProjectObj": [
                    {
                        "StationID": "XXXX",
                        "ProjID": "P735",
                        "ProjName": "同站汽車99推廣專案",
                        "ProDesc": "1.本專案限iRent會員使用。\n2.本專案活動期間為2020/1/30~2021/6/30。\n3.本專案優惠價：平日時租99元，日租990元；假日時租168元或198元，日租1,680元或1,980元(各車款價格詳見「關於iRent─租車費率」)。還車時依實際使用情形再收取里程費及eTag過路費。\n4.平日定義: 週一至週五(不含國定假日)。\n5.本專案為同站租還服務，還車需將車輛停放回原站點。\n6.預約租車時請選擇適合之預計取車時間，預約訂單成立後每台車將保留15分鐘。若超過預計取車時間15分鐘仍未取車，系統將自動取消該筆預約。\n7.本專案不得累計網路會員e-bonus，且不得折抵消費金額。\n8.實際費率以出車當日公告之活動內容為準。",
                        "CarBrend": "TOYOTA",
                        "CarType": "COROLLA CROSS",
                        "CarTypeName": "TOYOTA COROLLA CROSS",
                        "CarTypePic": "cross",
                        "Seat": 5,
                        "Operator": "supplierIrent",
                        "OperatorScore": 5.0,
                        "Insurance": 1,
                        "InsurancePerHour": 70,
                        "IsMinimum": 0,
                        "Price": 195,
                        "WorkdayPerHour": 135,
                        "HolidayPerHour": 218,
                        "CarOfArea": "XXXX",
                        "Content": "",
                        "IsRent": "N",
                        "IsFavStation": 0,
                        "IsShowCard": 0,
                        "MonthlyRentId": 0,
                        "CarWDHours": 0.0,
                        "CarHDHours": 0.0,
                        "MotoTotalMins": 0.0,
                        "WDRateForCar": 0.0,
                        "HDRateForCar": 0.0,
                        "WDRateForMoto": 0.0,
                        "HDRateForMoto": 0.0,
                        "MonthStartDate": "",
                        "MonthEndDate": ""
                    },
                    {
                        "StationID": "XXXX",
                        "ProjID": "P735",
                        "ProjName": "同站汽車99推廣專案_測試_汽包機66-3",
                        "ProDesc": "1.本專案限iRent會員使用。\n2.本專案活動期間為2020/1/30~2021/6/30。\n3.本專案優惠價：平日時租99元，日租990元；假日時租168元或198元，日租1,680元或1,980元(各車款價格詳見「關於iRent─租車費率」)。還車時依實際使用情形再收取里程費及eTag過路費。\n4.平日定義: 週一至週五(不含國定假日)。\n5.本專案為同站租還服務，還車需將車輛停放回原站點。\n6.預約租車時請選擇適合之預計取車時間，預約訂單成立後每台車將保留15分鐘。若超過預計取車時間15分鐘仍未取車，系統將自動取消該筆預約。\n7.本專案不得累計網路會員e-bonus，且不得折抵消費金額。\n8.實際費率以出車當日公告之活動內容為準。",
                        "CarBrend": "TOYOTA",
                        "CarType": "COROLLA CROSS",
                        "CarTypeName": "TOYOTA COROLLA CROSS",
                        "CarTypePic": "cross",
                        "Seat": 5,
                        "Operator": "supplierIrent",
                        "OperatorScore": 5.0,
                        "Insurance": 1,
                        "InsurancePerHour": 70,
                        "IsMinimum": 0,
                        "Price": 0,
                        "WorkdayPerHour": 99,
                        "HolidayPerHour": 168,
                        "CarOfArea": "XXXX",
                        "Content": "",
                        "IsRent": "N",
                        "IsFavStation": 0,
                        "IsShowCard": 0,
                        "MonthlyRentId": 1019,
                        "CarWDHours": 3.0,
                        "CarHDHours": 3.0,
                        "MotoTotalMins": 300.0,
                        "WDRateForCar": 99.0,
                        "HDRateForCar": 168.0,
                        "WDRateForMoto": 1.0,
                        "HDRateForMoto": 1.2,
                        "MonthStartDate": "2021/06/07",
                        "MonthEndDate": "2021/09/05"
                    }
                ],
                "StationInfoObj": [
                    {
                        "StationPic": "https://irentv2data.blob.core.windows.net/station/X0II_1_20210209000000.png",
                        "PicDescription": "停車場出入口\n"
                    },
                    {
                        "StationPic": "https://irentv2data.blob.core.windows.net/station/X0II_2_20210209000000.png",
                        "PicDescription": "劃設有專屬停車位"
                    },
                    {
                        "StationPic": "https://irentv2data.blob.core.windows.net/station/X0II_3_20210209000000.png",
                        "PicDescription": "劃設有專屬停車位"
                    },
                    {
                        "StationPic": "https://irentv2data.blob.core.windows.net/station/",
                        "PicDescription": ""
                    },
                    {
                        "StationPic": "https://irentv2data.blob.core.windows.net/station/",
                        "PicDescription": ""
                    }
                ]
            }
        ]
    }
}

```

* 錯誤代碼清單

| 錯誤代碼 | 說明 |
| ------- | ------- |
| ERR151 | 起始日期格式不符 |
| ERR152 | 結束日期格式不符 |
| ERR153 | 起始時間大於結束時間 |


-------------

## GetPolygon取得電子柵欄
### [/api/GetPolygon/]

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]
  
* input傳入參數說明

| 參數名稱  | 參數說明 | 必要 |  型態  | 範例        |
| --------- | -------- | :--: | :----: | ----------- |
| StationID | 據點代碼 | Y | string | X0II |
| ☆IsMotor| 機車汽車| Y | INT | 0:機車,1:汽車 |




* input範例
```input範例
{
    "StationID": "X0G1",
    "IsMotor": 0
}
```

* output回傳參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| Result | 是否成功 | int | 0:失敗 1:成功  |
| ErrorCode | 錯誤碼 | string | |
| ErrorMessage | 錯誤訊息 | string | |
| NeedRelogin | 是否需重新登入 | int | 0:否 1:是 |
| NeedUpgrade | 是否需要至商店更新 | int | 0:否 1:是 |
| Data | 資料物件 | | |
| PolygonType| 電子柵欄模式 | int | 0:優惠的取車;1:優惠的還車 |
| PolygonObj| 電子柵欄經緯度清單 | List |  |

* output範例
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "PolygonType": 0,
        "PolygonObj": [
            "POLYGON((120.28898139979174 22.66497932782427,120.2914597609215 22.665098131237773,120.2914597609215 22.662979454923615,120.28889020468523 22.662979454923615,120.28898139979174 22.66497932782427))"
        ]
    }
}
```

-------------


## GetBanner 取得廣告資訊

### [/api/GetBanner/]

* 20210316發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [GET]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| 無參數   |          |      |      |      |

* output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |
| BannerObj    | 廣告資訊列表       |  List  |               |

* BannerObj 參數說明

| 參數名稱    | 參數說明   |  型態  | 範例                                 |
| ----------- | ---------- | :----: | ------------------------------------------------------- |
| MarqueeText | 跑馬燈文字 | string | 測試Banner1                                             |
| PIC         | 圖片       | string | https://irentv2data.blob.core.windows.net/banner/01.png |
| URL         | 網頁網址   | string | https://www.easyrent.com.tw/upload/event/109event/2042/ |

* Output範例

```Output範例
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "BannerObj": [
            {
                "MarqueeText": "測試Banner1",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/01.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            },
            {
                "MarqueeText": "測試Banner2",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/02.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            },
            {
                "MarqueeText": "測試Banner3",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/03.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            },
            {
                "MarqueeText": "測試Banner4",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/04.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            },
            {
                "MarqueeText": "測試Banner5",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/05.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            },
            {
                "MarqueeText": "測試Banner6",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/06.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            },
            {
                "MarqueeText": "測試Banner7",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/07.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            },
            {
                "MarqueeText": "測試Banner8",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/08.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            },
            {
                "MarqueeText": "測試Banner9",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/09.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            },
            {
                "MarqueeText": "測試Banner10",
                "PIC": "https://irentv2data.blob.core.windows.net/banner/10.png",
                "URL": "https://www.easyrent.com.tw/upload/event/109event/2042/"
            }
        ]
    }
}
```

---------------



## GetNormalRent 取得同站租還站點

### [/api/GetNormalRent/]

* 20210324修改

* 20210407修改 - input移除掉Seats 

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式



* 動作 [GET]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| ShowALL | 是否顯示全部 | Y | int | 0:否 1:是 |
| Latitude | 緯度 |  | float | |
| Longitude | 經度 | | float | |
| Radius | 半徑 | | float | |
| * CarTypes | 車型清單 | N | array string | |


* input範例
```
{
    "ShowALL": "0",
    "Latitude": 25.060368,
    "Longitude": 121.520260,
    "Radius": 2.5,
    "CarTypes":[ "PRIUSC" ]
}
```

* output回傳參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| Result | 是否成功 | int | 0:失敗 1:成功  |
| ErrorCode | 錯誤碼 | string | 000000 |
| NeedRelogin | 是否需重新登入 | int | 0:否 1:是 |
| NeedUpgrade | 是否需要至商店更新 | int | 0:否 1:是 |
| ErrorMessage | 錯誤訊息 | string | Success |
| Data | 資料物件 | | |
| NormalRentObj   | 常用站點列表 |  List  | |

* NormalRentObj 參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| StationID | 據點代碼 | string | X0II |
| StationName | 據點名稱 | string | 濱江站 |
| Tel | 電話 | string  | 02-12345678 |
| ADDR | 地址 | string | 台北市松江路999號 |
| Latitude | 緯度 | float | |
| Longitude | 經度 | float | |
| Content | 其他說明 | string | |
| ContentForAPP | 據點描述(app顯示用) | string | |
| IsRequiredForReturn | 還車位置資訊必填 | int | 0:否 1:是 |
| StationPic | 據點照片 | List | |
| IsRent | 是否可租 | string | Y/N |


* StationPic參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| StationPic | 據點照片 | string | |
| PicDescription | 據點說明 | string | |


* output範例
```
{
	"Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "NormalRentObj": [
            {
                "StationID": "X0AO",
                "StationName": "iRent愛馬屋北車站",
                "Tel": "02-2375-6000",
                "ADDR": "中山北路一段18號旁空地",
                "Latitude": 25.046680,
                "Longitude": 121.519857,
                "Content": "",
                "ContentForAPP": null,
                "IsRequiredForReturn": 0,
                "StationPic": null,
                "IsRent": "Y"
            },
            {
                "StationID": "X0AP",
                "StationName": "iRent北市圓山站",
                "Tel": "02-2516-3816",
                "ADDR": "玉門街與敦煌路口旁 [車位已滿，請耐心等候入場]",
                "Latitude": 25.074977,
                "Longitude": 121.521395,
                "Content": "",
                "ContentForAPP": null,
                "IsRequiredForReturn": 0,
                "StationPic": null,
                "IsRent":"N"
            }
        ]
    }
}
```

----------------------------



## GetCarTypeGroupList 取得車型清單

### [/api/GetCarTypeGroupList/]

* 20210331發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| 無參數   |          |      |      |      |

* output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |


* SeatGroups資料物件說明

| 參數名稱 | 參數說明 | 型態 | 範例 |
| -------- | -------- | :--: | ---- |
| Seat     | 座椅數   | int  | 4    |
| CarInfos | 資料物件 |      |      |


* CarInfos 參數說明

| 參數名稱    | 參數說明   |  型態  | 範例                                 |
| ----------- | ---------- | :----: | ------------------------------------------------------- |
| Seat | 座椅數 | int | 4 |
| CarType       | 車型名稱   | string | TOYOTA Altis |
| CarTypePic    | 車型圖片   | string | altis |
| CarTypeName	| 車型代碼   | string | ALTIS |


* Output範例

```
{
	"Result": "0",
	"ErrorCode": "000000",
	"NeedRelogin": 0,
	"NeedUpgrade": 0,
	"ErrorMessage": "Success",
	"Data": {
		"SeatGroups": [
			{
				"Seat": 2,
				"CarInfos": [
					{
						"Seat": 2,
						"CarType": "KYMCO MANY-110",
						"CarTypePic": "iretScooter",
						"CarTypeName": "MANY-110"
					}
				]
			},
			{
				"Seat": 5,
				"CarInfos": [
					{
						"Seat": 5,
						"CarType": "TOYOTA ALTIS",
						"CarTypePic": "altis",
						"CarTypeName": "ALTIS"
					},
					{
						"Seat": 5,
						"CarType": "TOYOTA CAMRY",
						"CarTypePic": "camry",
						"CarTypeName": "CAMRY"
					},
					{
						"Seat": 5,
						"CarType": "TOYOTA PRIUS PHV",
						"CarTypePic": "priusPhv",
						"CarTypeName": "PRIUS PHV"
					},
					{
						"Seat": 5,
						"CarType": "TOYOTA PRIUSc",
						"CarTypePic": "priusC",
						"CarTypeName": "PRIUSc"
					},
					{
						"Seat": 5,
						"CarType": "TOYOTA SIENTA5人",
						"CarTypePic": "sienta",
						"CarTypeName": "SIENTA5人"
					},
					{
						"Seat": 5,
						"CarType": "TOYOTA VIOS",
						"CarTypePic": "vios",
						"CarTypeName": "VIOS"
					},
					{
						"Seat": 5,
						"CarType": "TOYOTA YARIS",
						"CarTypePic": "yaris",
						"CarTypeName": "YARIS"
					}
				]
			},
			{
				"Seat": 7,
				"CarInfos": [
					{
						"Seat": 7,
						"CarType": "TOYOTA SIENTA7人",
						"CarTypePic": "sienta",
						"CarTypeName": "SIENTA7人"
					}
				]
			}
		]
	}
}
```



## GetMapMedal 取得地圖徽章

### [/api/GetMapMedal/]

- 20210521發佈

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**


* 動作 [POST]
* input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| 無參數   |          |      |      |      |

* output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |
| MedalList    | 徽章明細           |  List  |               |

* MedalList 參數說明

| 參數名稱      | 參數說明 |  型態  | 範例     |
| ------------- | -------- | :----: | -------- |
| MileStone     | 徽章代碼 | string | newhand  |
| MileStoneName | 徽章名稱 | string | 新手上路 |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "MedalList": [
            {
                "MileStone": "newhand",
                "MileStoneName": "新手上路"
            },
            {
                "MileStone": "car_range_lv1",
                "MileStoneName": "出遊"
            }
        ]
    }
}
```


## AnyRent 取得路邊租還車輛

### [/api/AnyRent/]

- 20210531補上

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| ShowALL 	| 是否顯示全部  | Y | int | 0 |
| Latitude	| 緯度	| Y | double | 25.0688361 |
| Longitude | 經度	| Y | double | 121.5335611 |
| Radius 	| 半徑	| Y | double | 2.5 |

* input範例
```
{
    "ShowALL": "0",
    "Latitude": 25.060368,
    "Longitude": 121.520260,
    "Radius": 2.5
}
```

* output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| AnyRentObj | 資料物件           |        |               |


* AnyRentObj 參數說明

| 參數名稱      | 參數說明 |  型態  | 範例       |
| ------------- | -------- | :----: | ---------- |
| MonthlyRentId		| 訂閱制月租ID		| int | 123456 |
| MonProjNM			| 訂閱制月租名稱	| string | 測試_汽包機66-3	|
| CarWDHours		| 汽車平日時數	| double | 3.0	|
| CarHDHours		| 汽車假日時數	| double | 3.0 |
| MotoTotalMins		| 機車不分平假日分鐘數 | int | 300 |
| WDRateForCar		| 汽車平日優惠價 | double | 99.0  |
| HDRateForCar		| 汽車假日優惠價 | double | 168.0  |
| WDRateForMoto		| 機車平日優惠價 | dluble | 1.0  |
| HDRateForMoto		| 機車假日優惠價 | double | 1.2 |
| CarNo				| 車號			 | string | RCG-2305 |
| CarType			| 車型代碼		| string | PRIUSC |
| CarTypeName		| 車型名稱		| string | TOYOTA PRIUSc |
| CarOfArea			| 車輛地區		| string | 北區 |
| ProjectName		| 專案名稱		| string | 北區路邊汽車推廣專案 |
| Rental			| 每小時租金	| float | 168.0 |
| Mileage			| 每公里里程費		| float | 3.0 |
| Insurance			| 是否有安心服務 | int | 0 |
| InsurancePrice	| 安心服務每小時費用 | int | 50 |
| ShowSpecial		| 是否顯示活動文字 | int | 1 |
| SpecialInfo		| 活動文字			| string | |
| Latitude			| 緯度			| float | 25.0692639 |
| Longitude			| 經度			| float | 121.5308611 |
| Operator			| 供應商圖片		| string | supplierIrent |
| OperatorScore		| 供應商評價		| float | 5.0 |
| CarTypePic		| 車輛圖示名稱		| string | priusC |
| Seat				| 座位數			| int | 5 |
| ProjID			| 專案代碼			| string | P621 |


* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "AnyRentObj": [
            {
				"MonthlyRentId": 971,
                "MonProjNM": "測試_汽包機66-3",
                "CarWDHours": 3.0,
                "CarHDHours": 3.0,
                "MotoTotalMins": 300,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "CarNo": "RCF-7051",
                "CarType": "PRIUSC",
                "CarTypeName": "TOYOTA PRIUSc",
                "CarOfArea": "北區",
                "ProjectName": "北區路邊汽車推廣專案",
                "Rental": 168.0,
                "Mileage": 3.0,
                "Insurance": 1,
                "InsurancePrice": 50,
                "ShowSpecial": 0,
                "SpecialInfo": "",
                "Latitude": 25.0740556,
                "Longitude": 121.5172028,
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "CarTypePic": "priusC",
                "Seat": 5,
                "ProjID": "P621"
            },
            {
				"MonthlyRentId": 971,
                "MonProjNM": "測試_汽包機66-3",
                "CarWDHours": 3.0,
                "CarHDHours": 3.0,
                "MotoTotalMins": 300,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "CarNo": "RCF-9755",
                "CarType": "PRIUSC",
                "CarTypeName": "TOYOTA PRIUSc",
                "CarOfArea": "北區",
                "ProjectName": "北區路邊汽車推廣專案",
                "Rental": 168.0,
                "Mileage": 3.0,
                "Insurance": 1,
                "InsurancePrice": 50,
                "ShowSpecial": 0,
                "SpecialInfo": "",
                "Latitude": 25.0636778,
                "Longitude": 121.5425778,
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "CarTypePic": "priusC",
                "Seat": 5,
                "ProjID": "P621"
            }
		]
	}
}

```

----

## GetAnyRentProject 取得專案與資費(路邊)

### [/api/GetAnyRentProject/]

- 20210531補上

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| CarNo 	| 車號  | Y | string | RCG-0521 |
| SDate		| 預計取車時間	|  | string |  |
| EDate 	| 預計還車時間	|  | string |  |

* input範例
```
{
    "CarNo":"RCG-0521",
    "SDate":"",
    "EDate":""
}
```

* output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| GetAnyRentProjectObj | 資料物件           |        |               |


* GetAnyRentProjectObj 參數說明

| 參數名稱      | 參數說明 |  型態  | 範例       |
| ------------- | -------- | :----: | ---------- |
| StationID 		| 據點代碼		| string | X0SR |
| ProjID			| 專案代碼		| string | P621 |
| ProjName			| 專案名稱		| string | 北區路邊汽車推廣專案 |
| ProDesc			| 專案說明		| string | |
| CarBrend			| 車輛廠牌		| string | TOYOTA |
| CarType 			| 車輛型號		| string | PRIUSC |
| CarTypeName		| 車型名稱		| string | TOYOTA PRIUSc |
| CarTypePic		| 車型圖片		| string | priusC |
| Seat				| 座椅數		| int | 5 |
| Operator			| 業者ICON圖片	| string | supplierIrent |
| OperatorScore 	| 供應商評價	| float | 5.0 |
| Insurance			| 是否有安心服務 | int | 0 |
| InsurancePrice	| 安心服務每小時費用 | int | 50 |
| IsMinimum			| 是否是最低價	| int | 1 |
| Price				| 預估費用		| int | 159 |
| WorkdayPerHour	| 工作日每小時金額 | int | 99 |
| HolidayPerHour	| 假日每小時金額 | int | 168 |
| CarOfArea			| 車輛地區		| string | 北區 |
| Content			| 其他備註		| string |   |
| IsRent			| 是否可租		| int | 1 |
| IsFavStation		| 是否為喜好站點 | int | 1 |
| IsShowCard		| 是否顯示牌卡	| int | 1 |
| MonthlyRentId		| 訂閱制月租ID		| int | 123456 |
| CarWDHours		| 汽車平日時數	| double |	|
| CarHDHours		| 汽車假日時數	| double |  |
| MotoTotalMins		| 機車不分平假日分鐘數 | int |  |
| WDRateForCar		| 汽車平日優惠價 | double |   |
| HDRateForCar		| 汽車假日優惠價 | double |   |
| WDRateForMoto		| 機車平日優惠價 | dluble |   |
| HDRateForMoto		| 機車假日優惠價 | double |   |
| MonthStartDate	| 開始日			| string | |
| MonthEndDate		| 結束日			| string | |


* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "GetAnyRentProjectObj": [
            {
                "StationID": "X0SR",
                "ProjID": "P621",
                "ProjName": "北區路邊汽車推廣專案",
                "ProDesc": "1.本專案限iRent會員使用。\n2.本專案活動期間：即日起~2021/6/30。\n3.本專案優惠價：平日時租99元，日租990元；假日時租168元或198元，日租1,680元或1,980元(各車款價格詳見「關於iRent─租車費率」)。還車時依實際使用情形再收取里程費及eTag過路費。\n4.平日定義: 週一至週五(不含國定假日)。\n5.本專案租車限於系統公告之「台北市、新北市、基隆市、桃園市、新竹市、新竹縣特定服務區域內及桃園機場旁大園停車場(桃園市大園區中正東路437-1號)」取還車。\n6.大園停車場取/還車說明: 於大園停車場還車後，停車場業者將免費接駁至桃園國際機場，回國取車前亦可於機場電話聯繫停車場業者，由業者從機場接送至取車點【電話: (03)385-2888】。\n7.本專案還車限停放於台北市、新北市、基隆市、桃園市、新竹市、新竹縣路邊公有停車格(不含白線區域、累進費率、限時停車、時段性禁停、汽機車共用、身心障礙專用、孕婦及育有 6 歲以下兒童者專用、貨車專卸專用等特殊停車格)，以及特定路外停車場(開放還車停車場可於APP內查詢)，桃園國際機場限停放於大園停車場。違者將酌收車輛調度相關費用。\n8.預約訂單成立後每台車將保留30分鐘。若未在時間內完成取車，系統將自動取消該筆預約。\n9.實際費率以出車當日公告之活動內容為準。\n10.農曆春節期間採定價收費。\n11.春節期間不適用時數折抵及月租訂閱制方案。\n12.春節期間承租車輛，將依承租日數贈送iRent時數，滿2日贈送1小時(不累計)，採事後匯入。",
                "CarBrend": "TOYOTA",
                "CarType": "PRIUSC",
                "CarTypeName": "TOYOTA PRIUSc",
                "CarTypePic": "priusC",
                "Seat": 5,
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "Insurance": 1,
                "InsurancePerHour": 50,
                "IsMinimum": 1,
                "Price": 159,
                "WorkdayPerHour": 99,
                "HolidayPerHour": 168,
                "CarOfArea": "北區",
                "Content": "取/還車範圍：\n限台北市、新北市、基隆市、桃園市、新竹市、新竹縣服務區域內，以及桃園國際機場旁大園停車場(桃園市大園區中正東路437-1號)取/還車，詳細範圍可由地圖查詢。\n\n還車規範：\na.還車限停放於服務區域內合法路邊公有停車格(不含白線區域、累進費率、限時停車、時段性禁停、汽機車共用、身心障礙專用、孕婦及育有 6 歲以下兒童者專用、貨車專卸專用等特殊停車格) 、指定北區(北北基桃竹地區)路外停車場(開放還車停車場可於APP內查詢)\nb.請勿將車輛停放於私人停車場/車庫、紅線路段等違法區域及非指定停車場，違者將依「會員條款暨小客車租賃契約」向會員酌收車輛調度相關費用。\n\n桃園國際機場服務說明(大園停車場)：\n因疫情關係，目前僅提供出國送機服務。\n於大園停車場還車後，停車業者將免費接駁至桃園國際機場。(大園停車場電話：03-385-2888)",
                "IsRent": null,
                "IsFavStation": 0,
                "IsShowCard": 1,
                "MonthlyRentId": 0,
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 0.0,
                "WDRateForCar": 0.0,
                "HDRateForCar": 0.0,
                "WDRateForMoto": 0.0,
                "HDRateForMoto": 0.0,
                "MonthStartDate": "",
                "MonthEndDate": ""
            }
        ]
    }
}

```



## MotorRent 取得路邊租還機車

### [/api/MotorRent/]

- 20210531補上

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| ShowALL 	| 是否顯示全部  | Y | int | 0 |
| Latitude	| 緯度	| Y | double | 25.0688361 |
| Longitude | 經度	| Y | double | 121.5335611 |
| Radius 	| 半徑	| Y | double | 2.5 |

* input範例
```
{
    "ShowALL": "0",
    "Latitude": 25.060368,
    "Longitude": 121.520260,
    "Radius": 2.5
}
```


* output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| MotorRentObj | 資料物件           |        |               |


* MotorRentObj 參數說明

| 參數名稱      | 參數說明 |  型態  | 範例       |
| ------------- | -------- | :----: | ---------- |
| MonthlyRentId		| 訂閱制月租ID		| int | 123456 |
| MonProjNM			| 訂閱制月租名稱	| string | 	|
| CarWDHours		| 汽車平日時數	| double |	|
| CarHDHours		| 汽車假日時數	| double |  |
| MotoTotalMins		| 機車不分平假日分鐘數 | int |  |
| WDRateForCar		| 汽車平日優惠價 | double |   |
| HDRateForCar		| 汽車假日優惠價 | double |   |
| WDRateForMoto		| 機車平日優惠價 | dluble |   |
| HDRateForMoto		| 機車假日優惠價 | double |   |
| CarNo				| 車號			 | string | EWG-1235 |
| CarType			| 車型代碼		| string | MANY-110 |
| CarTypeName		| 車型名稱		| string | KYMCO MANY-110 |
| CarOfArea			| 車輛地區		| string | 北北桃 |
| ProjectName		| 專案名稱		| string | 10載便利北北桃 |
| Rental			| 每小時租金	| float | 168.0 |
| Mileage			| 每公里里程費		| float | 3.0 |
| Insurance			| 是否有安心服務 | int | 0 |
| InsurancePrice	| 安心服務每小時費用 | int | 50 |
| ShowSpecial		| 是否顯示活動文字 | int | 1 |
| SpecialInfo		| 活動文字			| string | |
| Power				| 機車電量			| float | 90.5 |
| RemainingMileage	| 預估里程			| float | 30.5 |
| Latitude			| 緯度			| float | 25.0692639 |
| Longitude			| 經度			| float | 121.5308611 |
| Operator			| 供應商圖片		| string | supplierIrent |
| OperatorScore		| 供應商評價		| float | 5.0 |
| CarTypePic		| 車輛圖示名稱		| string | priusC |
| Seat				| 座位數			| int | 5 |
| ProjID			| 專案代碼			| string | P621 |
| BaseMinutes		| 基本分鐘數		| int | 6 |
| BasePrice			| 基本費			| int | 10 |
| PerMinutesPrice	| 每分鐘幾元		| float | 	1.5 |



* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "MotorRentObj": [
            {
                "MonthlyRentId": 0,
                "MonProjNM": "",
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 0,
                "WDRateForCar": 0.0,
                "HDRateForCar": 0.0,
                "WDRateForMoto": 0.0,
                "HDRateForMoto": 0.0,
                "CarNo": "EWG-1235",
                "CarType": "MANY-110",
                "CarTypeName": "KYMCO MANY-110",
                "CarOfArea": "北北桃",
                "ProjectName": "10載便利北北桃",
                "Rental": 1.5,
                "Mileage": 2.5,
                "Insurance": 0,
                "InsurancePrice": 0,
                "ShowSpecial": 0,
                "SpecialInfo": "",
                "Power": 85.0,
                "RemainingMileage": 38.0,
                "Latitude": 25.0225389,
                "Longitude": 121.4804694,
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "ProjID": "P686",
                "BaseMinutes": 6,
                "BasePrice": 10,
                "PerMinutesPrice": 1.5
            },
            {
                "MonthlyRentId": 0,
                "MonProjNM": "",
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 0,
                "WDRateForCar": 0.0,
                "HDRateForCar": 0.0,
                "WDRateForMoto": 0.0,
                "HDRateForMoto": 0.0,
                "CarNo": "EWH-7726",
                "CarType": "MANY-110",
                "CarTypeName": "KYMCO MANY-110",
                "CarOfArea": "北北桃",
                "ProjectName": "10載便利北北桃",
                "Rental": 1.5,
                "Mileage": 2.5,
                "Insurance": 0,
                "InsurancePrice": 0,
                "ShowSpecial": 0,
                "SpecialInfo": "",
                "Power": 69.0,
                "RemainingMileage": 31.0,
                "Latitude": 25.0296278,
                "Longitude": 121.4779306,
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "ProjID": "P686",
                "BaseMinutes": 6,
                "BasePrice": 10,
                "PerMinutesPrice": 1.5
            }
		]
	}
}

```

----

## GetMotorRentProject 取得專案與資費(機車)

### [/api/GetMotorRentProject/]

- 20210531補上

- ASP.NET Web API (REST API)

- api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| CarNo 	| 車號  | Y | string | EWA-0127 |
| SDate		| 預計取車時間	|  | string |  |
| EDate 	| 預計還車時間	|  | string |  |

* input範例
```
{
    "CarNo":"EWA-0127",
    "SDate":"",
    "EDate":""
}
```

* output 回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| GetMotorProjectObj | 資料物件           |        |               |


* GetMotorProjectObj 參數說明

| 參數名稱      | 參數說明 |  型態  | 範例       |
| ------------- | -------- | :----: | ---------- |
| ProjID			| 專案代碼		| string | P686 |
| ProjName			| 專案名稱		| string | 10載便利北北桃 |
| ProDesc			| 專案說明		| string | |
| CarBrend			| 車輛廠牌		| string | KYMCO |
| CarType 			| 車輛型號		| string | IMOTO |
| CarTypeName		| 車型名稱		| string | KYMCO MANY-110 |
| CarTypePic		| 車型圖片		| string | iretScooter |
| Operator			| 業者ICON圖片	| string | supplierIrent |
| OperatorScore 	| 供應商評價	| float | 5.0 |
| Insurance			| 是否有安心服務 | int | 0 |
| InsurancePrice	| 安心服務每小時費用 | int | 50 |
| IsMinimum			| 是否是最低價	| int | 1 |
| BaseMinutes		| 基本分鐘數		| int | 6 |
| BasePrice			| 基本費			| int | 10 |
| PerMinutesPrice	| 每分鐘幾元		| float | 	1.5 |
| MaxPrice			| 每日金額上限		| int | 901 |
| CarOfArea			| 車輛地區		| string | 北北桃 |
| Content			| 其他備註		| string |   |
| Power				| 電量				| int | 50.0	|
| RemainingMileage	| 預估里程			| float | 30.5 |
| MonthlyRentId		| 訂閱制月租ID		| int | 123456 |
| MotoTotalMins		| 機車不分平假日分鐘數 | int |  |
| WDRateForMoto		| 機車平日優惠價 | dluble |   |
| HDRateForMoto		| 機車假日優惠價 | double |   |
| MonthStartDate	| 開始日			| string | |
| MonthEndDate		| 結束日			| string | |


* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "GetMotorProjectObj": [
            {
                "ProjID": "P686",
                "ProjName": "10載便利北北桃",
                "ProDesc": "1.本專案限iRent會員使用。\n2.本專案活動期間：2020/01/15~2021/6/30\n3.本專案優惠價：最低計費6分鐘(收費10元)，第7分鐘起每分鐘1.5元，日租上限為10小時。\n4.開放租還區域：台北市、新北市、桃園市部分區域，詳見APP內範圍顯示。\n5.還車規範：請停放於路邊公有機車停車格合法停車區域(不含白線區域、累進費率、限時停車、時段性禁停、汽機車共用、身心障礙專用等特殊停車格)，若違停遭到拖吊須自行負責承擔罰緩及拖吊費用。\n6.換電方式：若電量不足或需長途使用，可透過APP搜尋最近能源站進行自助換電。\n7.車內配備：車廂內備有兩頂安全帽(3/4罩、半罩各一)、擦車布、拋棄式衛生帽套，除衛生帽套外使用完請歸回原位，違者依法求償並停權處分。\n8.實際費率以出車當日公告之活動內容為準。",
                "CarBrend": "KYMCO",
                "CarType": "IMOTO",
                "CarTypeName": "KYMCO MANY-110",
                "CarTypePic": "iretScooter",
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "Insurance": 0,
                "InsurancePerHour": 0,
                "IsMinimum": 1,
                "BaseMinutes": 6,
                "BasePrice": 10,
                "PerMinutesPrice": 1.5,
                "MaxPrice": 901,
                "CarOfArea": "北北桃",
                "Content": "1.開放租還區域：\n台北市、新北市、桃園市部分區域，詳見APP內範圍顯示。\n2.還車規範：\n請停放於路邊公有機車停車格合法停車區域(限時停車格除外)，若違停遭到拖吊須自行負責承擔罰緩及拖吊費用。\n3.換電方式：\n若電量不足或需長途使用，可透過APP搜尋最近能源站進行自助換電。\n4.車內配備：\n車廂內備有兩頂安全帽(3/4罩、半罩各一)、擦車布、拋棄式衛生帽套，除衛生帽套外使用完請歸回原位，違者依法求償並停權處分。",
                "Power": 61.0,
                "RemainingMileage": 27.0,
                "MonthlyRentId": 0,
                "MotoTotalMins": 0.0,
                "WDRateForMoto": 0.0,
                "HDRateForMoto": 0.0,
                "MonthStartDate": "",
                "MonthEndDate": ""
            }
        ]
    }
}

```








----

# 取還車跟車機操控相關

## ChangeUUCard 變更悠遊卡

### [/api/ChangeUUCard/]

* 20210415發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**



* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H0002630 |

* input範例

```
{
    "OrderNo": "H0002630"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |
| HasBind      | 是否綁定           |  int   | 0:否 1:是     |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "HasBind": 1
    }
}
```
----

## GetPayDetail 取得租金明細(未完成，先暫存)

### [/api/GetPayDetail/]

* 20210527補資料

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H0002630 |
| Discount | 折抵汽車時數 | Y | int | 0 |
| MotorDiscount | 折抵機車分鐘數 | Y | int | 0 |


* input範例

```
{
    "OrderNo": "H0002630",
	"Discount": 0,
	"MotorDiscount": 0
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data參數說明

| 參數名稱    	| 參數說明           |  型態  | 範例          |
| ------------	| ------------------ | :----: | ------------- |
| CanUseDiscount		| 是否可使用點數折扣		| int 	| 1 	|
| CanUseMonthRent		| 是否可使用月租時間		| int 	| 1 	|
| IsMonthRent			| 是否為月租 				| int 	| 0 	|
| IsMotor				| 是否為機車				| int 	| 0 	|
| UseOrderPrice			| 使用訂金					| int	| 0		|
| ReturnOrderPrice		| 返還訂金					| int	| 0		|
| FineOrderPrice		| 沒收訂金					| int 	| 0		|
| Rent					| 資料物件					| Object | 		|
| CarRent				| 資料物件					| Object | 		|
| MotorRent				| 資料物件					| Object | 		|
| MonthRent				| 資料物件					| Object | 		|
| MonBase				| 集合物件					| List	 |		|
| ProType				| 專案類型 0:同站 3:路邊 4:機車 | int | 0 	|
| PayMode				| 計費模式 0:以時計費 1:以分計費 | int | 0 	|
| DiscountAlertMsg		| 不可使用折抵時的訊息提示 | string |  |
| NowSubsCards			| 集合物件					| List 	 |   	|

* Rent資料物件說明

| 參數名稱    	| 參數說明           |  型態  | 範例          |
| ------------	| ------------------ | :----: | ------------- |
| CarNo					| 車號			| string | RAA-1234		|
| BookingStartDate		| 實際取車時間	| string | 2021-05-26 08:21:00	|
| BookingEndDate		| 預計還車時間	| string | 2021-05-26 08:21:00	|
| RentalDate			| 實際還車時間	| string | 2021-05-26 08:21:00	|
| RentalTimeInterval	| 實際租用時數	| string | 60	|
| RedeemingTimeInterval	| 可折抵時數	| string | 942	|
| RedeemingTimeCarInterval | 可折抵時數(汽車) | string | 942 |
| RedeemingTimeMotorInterval | 可折抵時數(機車) | string | 331 |
| ActualRedeemableTimeInterval | 代表該「實際」可折抵的時數 | string | 60 |
| RemainRentalTimeInterval	| 代表折抵後的租用時數 | string | 0 |
| UseMonthlyTimeInterval | 月租專案時數折抵顯示 | string | 0 |
| UseNorTimeInterval	 | 一般時段時數折抵 	| string | 0 |
| RentBasicPrice 		| 每小時基本租金		| int | 0 |
| CarRental				| 車輛租金				| int | 0 |
| MileageRent			| 里程費用				| int | 0 |
| ETAGRental			| ETAG費用				| int | 0 |
| OvertimeRental		| 逾時費用				| int | 0 |
| TotalRental			| 總計					| int | 0 |
| ParkingFee			| 停車費用				| int | 0 |
| TransferPrice			| 轉乘費用				| int | 0 |
| InsurancePurePrice	| 安心服務				| int | 0 |
| InsuranceExtPrice		| 安心服務延長費用		| int | 0 |

* CarRent資料物件說明

| 參數名稱    	| 參數說明           |  型態  | 範例          |
| ------------	| ------------------ | :----: | ------------- |
| HourOfOneDay		| 多少小時算一天		| int 	| 10		|
| HoildayPrice		| 假日金額				| int	| 1680		|
| WorkdayPrice		| 平日金額				| int	| 990		|
| HoildayOfHourPrice | 假日每小時金額		| int	| 168		|
| WorkdayOfHourPrice | 平日每小時金額		| int	| 99		|
| MilUnit			| 每公里金額			| float	| 3.0		|

* MotorRent資料物件說明

| 參數名稱    	| 參數說明           |  型態  | 範例          |
| ------------	| ------------------ | :----: | ------------- |
| BaseMinutes 	| 基本時數			 | int 		| 0  |
| BaseMinutePrice | 基本費			 | int		| 0  |
| MinuteOfPrice	| 每分鐘價格		 | float	| 0.0  |


* MonthRent資料物件說明

| 參數名稱    	| 參數說明           |  型態  | 範例          |
| ------------	| ------------------ | :----: | ------------- |
| WorkdayRate	| 平日價格			 | float   |  0.0 	|
| HoildayRate	| 假日價格			 | float	| 0.0  |

* MonBase集合物件說明

| 參數名稱    	| 參數說明           |  型態  | 範例          |
| ------------	| ------------------ | :----: | ------------- |
| MonthlyRentId | 月租訂單編號		 | int  |  12345  |
| ProjNM		| 月租方案名稱		 | string |   |


* NowSubsCards集合物件說明

| 參數名稱    	| 參數說明           |  型態  | 範例          |
| ------------	| ------------------ | :----: | ------------- |
| MonthlyRentId | 月租編號			 | int  | 12345 		|
| ProjID		| 月租方案代碼		 | string | MR66		|
| MonProPeroid	| 總期數			| int	| 3		|
| ShortDays		| 短期總天數		| int	| 0		|
| MonProjNM		| 月租方案名稱		| string | MR66測試 |
| WorkDayHours	| 平日時數			| double | 0 |
| HolidayHours	| 假日時數			| double | 0 |
| MotoTotalMins | 機車分鐘數		| double | 0 |
| WorkDayRateForCar | 汽車平日優惠費率 | double | 99.0 |
| HoildayRateForCar | 汽車假日優惠費率 | double | 168.0 |
| WorkDayRateForMoto | 機車平日優惠費率 | double | 1.5 |
| HoildayRateForMoto | 機車假日優惠費率 | double | 2.0 |
| StartDate			| 月租起日		| datetime |  |
| EndDate			| 月租迄日		| datetime |  |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "CanUseDiscount": 1,
        "CanUseMonthRent": 1,
        "IsMonthRent": 0,
        "IsMotor": 0,
        "UseOrderPrice": 0,
        "ReturnOrderPrice": 0,
        "FineOrderPrice": 0,
        "Rent": {
            "CarNo": "RCG-0782",
            "BookingStartDate": "2021-05-26 08:21:00",
            "BookingEndDate": "2021-05-26 08:21:00",
            "RentalDate": "2021-05-26 08:21:00",
            "RentalTimeInterval": "60",
            "RedeemingTimeInterval": "942",
            "RedeemingTimeCarInterval": "942",
            "RedeemingTimeMotorInterval": "331",
            "ActualRedeemableTimeInterval": "60",
            "RemainRentalTimeInterval": "0",
            "UseMonthlyTimeInterval": "0",
            "UseNorTimeInterval": "60",
            "RentBasicPrice": 0,
            "CarRental": 0,
            "MileageRent": 0,
            "ETAGRental": 0,
            "OvertimeRental": 0,
            "TotalRental": 0,
            "ParkingFee": 0,
            "TransferPrice": 0,
            "InsurancePurePrice": 0,
            "InsuranceExtPrice": 0
        },
        "CarRent": {
            "HourOfOneDay": 10,
            "HoildayPrice": 1680,
            "WorkdayPrice": 990,
            "HoildayOfHourPrice": 168,
            "WorkdayOfHourPrice": 99,
            "MilUnit": 3.0
        },
        "MotorRent": {
            "BaseMinutes": 0,
            "BaseMinutePrice": 0,
            "MinuteOfPrice": 0.0
        },
        "MonthRent": {
            "WorkdayRate": 0.0,
            "HoildayRate": 0.0
        },
        "ProType": 3,
        "PayMode": 0,
        "DiscountAlertMsg": ""
    }
}
```

----
## BookingStart 汽車取車

### [/api/BookingStart/]

* 20210820補資料

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H0002630 |
| ED | 路邊租還可以重設結束時間 | N | string | 0 |
| SKBToken | SKB的token | Y | string | 0 |
| Insurance | 加購安心服務 | Y | int | 0:否;1:有 |

* input範例

```
{
    "OrderNo": "H10641049",
    "ED": "",
    "SKBToken": "",
    "Insurance": 0
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```

## BookingStartMotor 機車取車

### [/api/BookingStartMotor/]

* 20210820補資料

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H10641049 |

* input範例

```
{
    "OrderNo": "H10641049"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```


----

# 月租訂閱制相關


## GetMonthList 取得訂閱制月租列表/我的所有方案

###  [/api/GetMonthList/]

* 20210510發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                 | 必要 |  型態  | 範例                                                                                |
| ---------- | ------------------------ | :--: | :----: | ----------------------------------                                                  |
| IsMoto     | 是否為機車               |  N   |  int   | 0:否 1:是                                                                           |
| ReMode     | 回傳模式                 |  N   |  int   | 0:自動判定(若已有購買月租為我的所有方案2,若無則為月租列表1) 1:月租列表 2:我的所有方案 |

* input範例

```
{
    "IsMoto": 0,
    "ReMode": 1,
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Result       | 是否成功                |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼                  | string | 000000        |
| NeedRelogin  | 是否需重新登入          |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新      |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息                | string | Success       |
| Data         | 資料物件                |        |               |

* 資料物件說明,汽車牌卡(ReMode=1, IsMoto=0)

| 參數名稱    | 參數說明                    | 型態 | 範例 |
| --------    | --------                    | :--: | ---- |
| IsMotor     | 是否為機車                  | int  |  0   |
| NorMonCards | 汽車牌卡(資料物件list)      |      |      |
| MixMonCards | 城市車手牌卡(資料物件list)  |      |      |
| ReMode      | 模式(1:月租，2我的所有方案) | int  |  1   |


* 資料物件說明,機車牌卡(ReMode=1, IsMoto=1)

| 參數名稱    | 參數說明                    | 型態 | 範例 |
| --------    | --------                    | :--: | ---- |
| IsMotor     | 是否為機車                  | int  |   1  |
| NorMonCards | 機車牌卡(資料物件List)      |      |      |
| ReMode      | 模式(1:月租，2我的所有方案) | int  |   1  |

* 資料物件說明,我的所有方案(ReMode=2)

| 參數名稱    | 參數說明                    | 型態 | 範例 |
| --------    | --------                    | :--: | ---- |
| MyCar       | 汽車牌卡(資料物件obj)       |      |      |
| MyMoto      | 機車牌卡(資料物件obj)       |      |      |
| ReMode      | 模式(1:月租，2我的所有方案) | int  |  2   |


* NorMonCards, MixMonCards  參數說明

| 參數名稱      | 參數說明             |  型態  | 範例     |
| -----------   | ----------           | :----: | ---------|
| MonProjID     | 方案代碼(key)        | string | MR66     |
| MonProjNM     | 方案名稱      | string | MR66測試 |
| MonProPeriod  | 總期數(key)          | int    | 3        |
| ShortDays	    | 短期總天數(key)      | int    | 0        |
| PeriodPrice	| 方案價格             | int    | 7000     |
| IsMoto	    | 是否為機車0否1是     | int    | 0        |
| CarWDHours	| 汽車平日時數         | double | 3.0    |
| CarHDHours	| 汽車假日時數         | double | 3.0      |
| MotoTotalMins	| 機車不分平假日分鐘數 | int | 300   |
| WDRateForCar	| 汽車平日優惠費率 | double | 99.0 |
| HDRateForCar	| 汽車假日優惠費率 | double | 168.0 |
| WDRateForMoto	| 機車平日優惠費率 | double | 1.0 |
| HDRateForMoto	| 機車假日優惠費率 | double | 1.2 |
| IsDiscount	| 是否為優惠方案0否1是 | int    | 1        |
| IsPay	| 是否有繳費0否1是 | int | 1 |
| IsMix | 是否為城市車手 | int | 0 |

* MyCar, MyMoto 參數說明

| 參數名稱  | 參數說明       | 型態   | 範例                                   |
| --------- | -------------- | ------ | -------------------------------------- |
| 其餘參數  | 同NorMonCards  |        | 參考NorMonCards, MixMonCards  參數說明 |
| NxtPay	| 下期是否有繳費(0否1是) | int | 0 |
| StartDate | 訂閱制月租起日 | string | 05/18 00:00                            |
| EndDate   | 訂閱制月租迄日 | string | 06/16 23:59                            |

  

* Output範例,汽車牌卡(ReMode=1, IsMoto=0)

```
{
	"Result": "1",
	"ErrorCode": "000000",
	"NeedRelogin": 0,
	"NeedUpgrade": 0,
	"ErrorMessage": "Success",
	"Data": {{
	"Result": "1",
	"ErrorCode": "000000",
	"NeedRelogin": 0,
	"NeedUpgrade": 0,
	"ErrorMessage": "Success",
	"Data": {
		"IsMotor": 0,
		"NorMonCards": [
			{
				"MonProjID": "MR01",
				"MonProjNM": "汽車平日入門2期",
				"MonProPeriod": 2,
				"ShortDays": 0,
				"PeriodPrice": 149,
				"IsMoto": 0,
				"CarWDHours": 1.0,
				"CarHDHours": 0.0,
				"MotoTotalMins": 0,
				"WDRateForCar": 99.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 2.0,
				"HDRateForMoto": 2.0,
				"IsDiscount": 0,
				"IsPay": 0,
				"IsMix": 0
			},
			{
				"MonProjID": "MR02",
				"MonProjNM": "汽車平日低資費6期",
				"MonProPeriod": 6,
				"ShortDays": 0,
				"PeriodPrice": 2999,
				"IsMoto": 0,
				"CarWDHours": 33.0,
				"CarHDHours": 0.0,
				"MotoTotalMins": 0,
				"WDRateForCar": 90.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 1.8,
				"HDRateForMoto": 1.8,
				"IsDiscount": 0,
				"IsPay": 0,
				"IsMix": 0
			},
			{
				"MonProjID": "MR99",
				"MonProjNM": "測試_汽車平日6000",
				"MonProPeriod": 3,
				"ShortDays": 0,
				"PeriodPrice": 6000,
				"IsMoto": 0,
				"CarWDHours": 40.0,
				"CarHDHours": 0.0,
				"MotoTotalMins": 0,
				"WDRateForCar": 90.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 1.5,
				"HDRateForMoto": 1.5,
				"IsDiscount": 0,
				"IsPay": 0,
				"IsMix": 0
			},
			{
				"MonProjID": "MR100",
				"MonProjNM": "測試_汽平7000",
				"MonProPeriod": 3,
				"ShortDays": 0,
				"PeriodPrice": 7000,
				"IsMoto": 0,
				"CarWDHours": 50.0,
				"CarHDHours": 0.0,
				"MotoTotalMins": 0,
				"WDRateForCar": 90.0,
				"HDRateForCar": 160.0,
				"WDRateForMoto": 1.5,
				"HDRateForMoto": 1.5,
				"IsDiscount": 0,
				"IsPay": 0,
				"IsMix": 0
			},
			{
				"MonProjID": "MR101",
				"MonProjNM": "測試_汽平8000",
				"MonProPeriod": 3,
				"ShortDays": 0,
				"PeriodPrice": 8000,
				"IsMoto": 0,
				"CarWDHours": 60.0,
				"CarHDHours": 0.0,
				"MotoTotalMins": 0,
				"WDRateForCar": 90.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 1.5,
				"HDRateForMoto": 1.5,
				"IsDiscount": 0,
				"IsPay": 0,
				"IsMix": 0
			}
		],
		"MixMonCards": [
			{
				"MonProjID": "MR10",
				"MonProjNM": "汽車平日299入門方案",
				"MonProPeriod": 6,
				"ShortDays": 0,
				"PeriodPrice": 299,
				"IsMoto": 0,
				"CarWDHours": 2.0,
				"CarHDHours": 0.0,
				"MotoTotalMins": 30,
				"WDRateForCar": 99.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 2.0,
				"HDRateForMoto": 2.0,
				"IsDiscount": 0,
				"IsPay": 0,
				"IsMix": 1
			},
			{
				"MonProjID": "MR66",
				"MonProjNM": "測試_汽包機66-1",
				"MonProPeriod": 1,
				"ShortDays": 0,
				"PeriodPrice": 6000,
				"IsMoto": 0,
				"CarWDHours": 1.0,
				"CarHDHours": 1.0,
				"MotoTotalMins": 100,
				"WDRateForCar": 99.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 1.0,
				"HDRateForMoto": 1.2,
				"IsDiscount": 0,
				"IsPay": 0,
				"IsMix": 1
			},
			{
				"MonProjID": "MR102",
				"MonProjNM": "測試_汽包機102-3",
				"MonProPeriod": 3,
				"ShortDays": 0,
				"PeriodPrice": 7300,
				"IsMoto": 0,
				"CarWDHours": 4.0,
				"CarHDHours": 4.0,
				"MotoTotalMins": 400,
				"WDRateForCar": 99.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 1.0,
				"HDRateForMoto": 1.2,
				"IsDiscount": 0,
				"IsPay": 0,
				"IsMix": 1
			},
			{
				"MonProjID": "MR103",
				"MonProjNM": "測試_汽包機103-3",
				"MonProPeriod": 3,
				"ShortDays": 0,
				"PeriodPrice": 7800,
				"IsMoto": 0,
				"CarWDHours": 4.0,
				"CarHDHours": 3.0,
				"MotoTotalMins": 300,
				"WDRateForCar": 99.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 1.0,
				"HDRateForMoto": 1.2,
				"IsDiscount": 0,
				"IsPay": 0,
				"IsMix": 1
			}
		],
		"ReMode": 1
	}
}
```

* Output範例,機車牌卡(ReMode=1, IsMoto=1)

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "IsMotor": 1,
        "NorMonCards": [
            {
                "MonProjID": "MR03",
                "MonProjNM": "機車入門2期",
                "MonProPeriod": 2,
                "ShortDays": 0,
                "PeriodPrice": 99,
                "IsMoto": 1,
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 50,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.5,
                "HDRateForMoto": 1.5,
                "IsDiscount": 0,
                "IsPay": 0,
				"IsMix": 0
            },
            {
                "MonProjID": "MR04",
                "MonProjNM": "機車低資費6期",
                "MonProPeriod": 6,
                "ShortDays": 0,
                "PeriodPrice": 299,
                "IsMoto": 1,
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 200,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.3,
                "HDRateForMoto": 1.3,
                "IsDiscount": 0,
                "IsPay": 0,
				"IsMix": 0
            },
            {
                "MonProjID": "MR05",
                "MonProjNM": "機車中資費6期",
                "MonProPeriod": 6,
                "ShortDays": 0,
                "PeriodPrice": 599,
                "IsMoto": 1,
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 550,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.0,
                "IsDiscount": 0,
                "IsPay": 0,
				"IsMix": 0
            },
            {
                "MonProjID": "MR98",
                "MonProjNM": "測試_機車不分平假日899",
                "MonProPeriod": 3,
                "ShortDays": 0,
                "PeriodPrice": 899,
                "IsMoto": 1,
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 1000,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.2,
                "HDRateForMoto": 1.2,
                "IsDiscount": 0,
                "IsPay": 0,
				"IsMix": 0
            },
            {
                "MonProjID": "MR200",
                "MonProjNM": "測試_機車2000",
                "MonProPeriod": 3,
                "ShortDays": 0,
                "PeriodPrice": 2000,
                "IsMoto": 1,
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 600,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "IsDiscount": 0,
                "IsPay": 0,
				"IsMix": 0
            },
            {
                "MonProjID": "MR201",
                "MonProjNM": "測試_機車3000",
                "MonProPeriod": 3,
                "ShortDays": 0,
                "PeriodPrice": 3000,
                "IsMoto": 1,
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 800,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "IsDiscount": 0,
                "IsPay": 0,
				"IsMix": 0
            },
            {
                "MonProjID": "MR202",
                "MonProjNM": "測試_機車5000",
                "MonProPeriod": 3,
                "ShortDays": 0,
                "PeriodPrice": 5000,
                "IsMoto": 1,
                "CarWDHours": 0.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 1200,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "IsDiscount": 0,
                "IsPay": 0,
				"IsMix": 0
            }
        ],
        "ReMode": 1
    }
}
```

* Output範例,我的所有方案(ReMode=2)
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "MyCar": {
			"NxtPay": 0,
            "StartDate": "05/18 00:00",
            "EndDate": "06/16 23:59",
            "MonProjID": "MR66",
            "MonProjNM": "測試_汽包機66-3",
            "MonProPeriod": 3,
            "ShortDays": 0,
            "PeriodPrice": 7000,
            "IsMoto": 0,
            "CarWDHours": 3.0,
            "CarHDHours": 3.0,
            "MotoTotalMins": 300,
            "WDRateForCar": 99.0,
            "HDRateForCar": 168.0,
            "WDRateForMoto": 1.0,
            "HDRateForMoto": 1.2,
            "IsDiscount": 0,
            "IsPay": 1,
			"IsMix": 1
        },
        "MyMoto": {
			"NxtPay": 0,
            "StartDate": "05/18 00:00",
            "EndDate": "06/16 23:59",
            "MonProjID": "MR200",
            "MonProjNM": "測試_機車2000",
            "MonProPeriod": 3,
            "ShortDays": 0,
            "PeriodPrice": 2000,
            "IsMoto": 1,
            "CarWDHours": 0.0,
            "CarHDHours": 0.0,
            "MotoTotalMins": 600,
            "WDRateForCar": 99.0,
            "HDRateForCar": 168.0,
            "WDRateForMoto": 1.0,
            "HDRateForMoto": 1.2,
            "IsDiscount": 0,
            "IsPay": 1,
			"IsMix": 0
        },
        "ReMode": 2
    }
}
```

------



## GetMonthGroup 月租專案群組

### [/api/GetMonthGroup/]

* 20210510發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                 | 必要 |  型態   | 範例                              |
| ---------- | ------------------------ | :--: | :----:  | ----------------------------------|
| MonProjID  | 專案代碼                 |  Y   |  string | MR66                              |
| Mode		 | 使用模式(0:一般 1:變更合約 |  N   | string  | 0								 |

* input範例

```
{
    "MonProjID": "MR66",
	"Mode":0
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Result       | 是否成功                |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼                  | string | 000000        |
| NeedRelogin  | 是否需重新登入          |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新      |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息                | string | Success       |
| Data         | 資料物件                |        |               |

* 資料物件說明

| 參數名稱    | 參數說明                    | 型態    | 範例                  |
| --------    | --------                    | :--:    | ----------------------|
| MonProDisc  | 注意事項                    | string  |  汽包機66-1注意事項   |
| MonCards    | 汽機車牌卡(資料物件list)    |         |      |

* MonCards 參數說明

| 參數名稱      | 參數說明             |  型態  | 範例     |
| -----------   | ----------           | :----: | ---------|
| UseUntil		| 使用期限(Mode=1時，此欄位變為變更合約用的開始時間)  | string | 2022/02/28 00:00|
| MonProjID     | 方案代碼(key)        | string | MR66     |
| MonProjNM     | 方案名稱        | string | MR66測試 |
| MonProPeriod  | 總期數(key)          | int    | 3        |
| ShortDays	    | 短期總天數(key)      | int    | 0        |
| PeriodPrice	| 方案價格             | int    | 7000     |
| IsMoto	    | 是否為機車0否1是     | int    | 0        |
| CarWDHours	| 汽車平日時數         | double | 10       |
| CarHDHours	| 汽車假日時數         | double | 0        |
| MotoTotalMins	| 機車不分平假日分鐘數 | int | 120      |
| WDRateForCar	| 汽車平日優惠費率 | double | 99.0 |
| HDRateForCar	| 汽車假日優惠費率 | double | 168.0 |
| WDRateForMoto	| 機車平日優惠費率 | double | 1.0 |
| HDRateForMoto	| 機車假日優惠費率 | double | 1.2 |
| IsDiscount	| 是否為優惠方案(0否1是) | int    | 1        |
| IsPay	| 當期是否有繳費(0否1是) | int | 1 |
| IsMix | 是否為城市車手(0否1是) | int | 0 |
| PayPrice		| 信用卡授權金額	   | int	| 7000	   |

* Output範例
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "MonProDisc": "汽包機66-6注意事項",
        "MonCards": [
            {
				"UseUntil": "2022/02/28 00:00",
                "MonProjID": "MR66",
                "MonProjNM": "測試_汽包機66-6",
                "MonProPeriod": 6,
                "ShortDays": 0,
                "PeriodPrice": 9000,
                "IsMoto": 0,
                "CarWDHours": 6.0,
                "CarHDHours": 6.0,
                "MotoTotalMins": 600,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "IsDiscount": 0,
                "IsPay": 0,
				"IsMix": 1,
				"PayPrice":7000
            }
        ]
    }
}
```

------



## BuyNow/DoAddMonth 購買月租

### [/api/BuyNow/DoAddMonth]

* 20210510發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

* input 購買月租 (api/BuyNow/DoAddMonth)) 參數說明

| 參數名稱     | 參數說明              | 必要 |  型態  | 範例 |
| ------------ | --------------------- | :--: | :----: | ---- |
| MonProjID    | 專案編號(key          |  Y   | string | MR66 |
| MonProPeriod | 期數(key)             |  Y   |  int   | 3    |
| ShortDays    | 短天期(key)           |  Y   |  int   | 0    |
| SetSubsNxt   | 設定自動續約(0否,1是) |  N   |  int   | 0    |
| PayTypeId    | 付款方式              |  N   |  int   | 0    |
| InvoTypeId   | 發票方式              |  N   |  int   | 2    |



* input範例 (購買月租)

```
{
    "MonProjID":"MR66",
    "MonProPeriod":3,
    "ShortDays":0,
    "SetSubsNxt":1,
    "PayTypeId":0,
    "InvoTypeId":2
}
```


* Output回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Result       | 是否成功                |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼                  | string | 000000        |
| NeedRelogin  | 是否需重新登入          |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新      |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息                | string | Success       |
| Data         | 資料物件                |        |               |

* 資料物件說明

| 參數名稱  | 參數說明              | 型態 | 範例 |
| --------- | --------------------- | :--: | ---- |
| PayResult | 付費結果(0失敗 1成功) | int  | 0    |

* Output範例
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "PayResult": 1
    }
}
```
----

## BuyNow/DoUpMonth 月租升轉

### [/api/BuyNow/DoUpMonth]

* 20210510發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

* input 月租升轉 (api/BuyNow/DoUpMonth) 參數說明

| 參數名稱        | 參數說明          | 必要 | 型態   | 範例  |
| --------------- | ----------------- | ---- | ------ | ----- |
| MonProjID       | 專案編號(key)     | Y    | string | MR66  |
| MonProPeriod    | 期數(key)         | Y    | int    | 3     |
| ShortDays       | 短天期(key)       | Y    | int    | 0     |
| UP_MonProjID    | 升轉專案編號(key) | Y    | string | MR102 |
| UP_MonProPeriod | 升轉期數(key)     | Y    | int    | 3     |
| UP_ShortDays    | 升轉短天期(key)   | Y    | int    | 0     |
| PayTypeId       | 付款方式          | N    | int    | 0     |
| InvoTypeId      | 發票方式          | N    | int    | 2     |


* input範例 (月租升轉)

```
{
  "MonProjID":"MR66",
  "MonProPeriod":3,
  "ShortDays":0,
  "UP_MonProjID":"MR102",
  "UP_MonProPeriod":3,
  "UP_ShortDays":0,
  "PayTypeId":0,
  "InvoTypeId":2
}
```

 

* Output回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Result       | 是否成功                |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼                  | string | 000000        |
| NeedRelogin  | 是否需重新登入          |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新      |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息                | string | Success       |
| Data         | 資料物件                |        |               |

* 資料物件說明

| 參數名稱  | 參數說明              | 型態 | 範例 |
| --------- | --------------------- | :--: | ---- |
| PayResult | 付費結果(0失敗 1成功) | int  | 0    |

* Output範例
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "PayResult": 1
    }
}
```
----

## BuyNow/DoPayArrs 月租欠費

### [/api/BuyNow/DoPayArrs]

* 20210510發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

* input 月租欠費繳交(api/BuyNow/DoPayArrs))參數說明

| 參數名稱   | 參數說明 | 必要 | 型態 | 範例 |
| ---------- | -------- | ---- | ---- | ---- |
| PayTypeId  | 付款方式 | N    | int  | 0    |
| InvoTypeId | 發票方式 | N    | int  | 2    |


* input範例 (月租欠費繳交)

```
{ 
  "PayTypeId":0,
  "InvoTypeId":2
}
```

  

* Output回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Result       | 是否成功                |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼                  | string | 000000        |
| NeedRelogin  | 是否需重新登入          |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新      |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息                | string | Success       |
| Data         | 資料物件                |        |               |

* 資料物件說明

| 參數名稱  | 參數說明              | 型態 | 範例 |
| --------- | --------------------- | :--: | ---- |
| PayResult | 付費結果(0失敗 1成功) | int  | 0    |

* Output範例
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "PayResult": 1
    }
}
```

----


## GetMySubs 我的方案牌卡明細

### [/api/GetMySubs/]

* 20210511發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                 | 必要 |  型態   | 範例     				|
| ---------- | ------------------------ | :--: | :----:  | -------------------------|
| MonProjID  | 月租專案代碼                 |  Y   |  string | MR66                     |
| MonProPeriod | 總期數					|  Y   | int | 3 |
| ShortDays | 短期總天數,非短期則為0  |  Y   | int | 0 |

* input範例

```
{
    "MonProjID": "MR66",
	"MonProPeriod": 3,
	"ShortDays" : 0
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Result       | 是否成功                |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼                  | string | 000000        |
| NeedRelogin  | 是否需重新登入          |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新      |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息                | string | Success       |
| Data         | 資料物件                |        |               |

* 資料物件說明

| 參數名稱    | 參數說明                    | 型態    | 範例                  |
| --------    | --------                    | :--:    | ----------------------|
| Month       | 資料物件 | obj | |

* Month 參數說明

| 參數名稱      | 參數說明              |  型態  | 範例               |
| -----------   | ----------            | :----: | -------------------|
| MonProjID     | 月租專案代碼  | string | MR66 |
| MonProPeriod  | 總期數		| int | 3 |
| ShortDays		| 短天期天數    | int | 0 |
| MonProjNM		| 月租專案名稱  | string | 測試_汽包機66-3 |
| CarWDHours	| 汽車平日時數	| double | 3.0             |
| CarHDHours	| 汽車假日時數	| double | 3.0 |
| MotoTotalMins | 機車不分平假日分鐘數 | int | 300.0 |
| WDRateForCar | 汽車平日優惠價 | double | 99.0 |
| HDRateForCar | 汽車假日優惠價 | double | 168.0 |
| WDRateForMoto | 機車平日優惠價 | double | 1.0 |
| HDRateForMoto | 機車假日優惠價 | double | 1.2 |
| StartDate 	| 起日 			| string | 2021/05/18 00:00|
| EndDate 		| 迄日 			| string | 2021/06/16 23:59 |
| MonthStartDate | 全月租專案起日 | string | 2021/05/18 00:00|
| MonthEndDate | 全月租專案迄日 | string | 2021/08/16 23:59|
| NxtMonProPeriod | 下期續訂總期數 | string | 3 |
| IsMix | 是否為城市車手 (0否1是) | int | 1 |
| IsUpd | 是否已升級 (0否1是) | int | 0 |
| SubsNxt		| 是否自動續訂 (0否1是) | int | 1 |
| IsChange		| 是否變更下期合約 (0否1是) | int | 0 |
| IsPay 		| 是否當期有繳費 (0否1是) | int | 1 |
| IsMoto	    | 是否為機車0否1是     | int    | 0        |

* Output範例
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "Month": {
            "MonProjID": "MR66",
            "MonProPeriod": 3,
            "ShortDays": 0,
            "MonProjNM": "測試_汽包機66-3",
            "CarWDHours": 3.0,
            "CarHDHours": 3.0,
            "MotoTotalMins": 300,
            "WDRateForCar": 99.0,
            "HDRateForCar": 168.0,
            "WDRateForMoto": 1.0,
            "HDRateForMoto": 1.2,
            "StartDate": "2021/05/26 00:00",
            "EndDate": "2021/06/24 23:59",
            "MonthStartDate": "2021/05/26 00:00",
            "MonthEndDate": "2021/08/24 23:59",
            "NxtMonProPeriod": 3,
            "IsMix": 1,
            "IsUpd": 0,
            "SubsNxt": 1,
            "IsChange": 0,
            "IsPay": 1,
			"IsMoto": 0
        }
    }
}
```




## GetSubsCNT 取得合約明細

### [/api/GetSubsCNT/]

* 20210511發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                 | 必要 |  型態   | 範例     				|
| ---------- | ------------------------ | :--: | :----:  | -------------------------|
| MonProjID  | 月租專案代碼                 |  Y   |  string | MR66                     |
| MonProPeriod | 總期數					|  Y   | int | 6  |
| ShortDays | 短期總天數,非短期則為0    |  Y   | int | 0 |


* input範例

```
{
    "MonProjID": "MR66",
	"MonProPeriod": 3,
	"ShortDays" : 0
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Result       | 是否成功                |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼                  | string | 000000        |
| NeedRelogin  | 是否需重新登入          |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新      |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息                | string | Success       |
| Data         | 資料物件                |        |        　       |


* 資料物件說明

| 參數名稱    | 參數說明   | 型態    | 範例               |
| --------    | --------  | :--:    | ---------------------- |
| NowCard		| 資料物件(目前方案) | obj |  　 |
| NxtCard 		| 資料物件(下期合約) | obj |  　 |


* NowCard, NxtCard參數說明

| 參數名稱      | 參數說明              |  型態  | 範例               |
| -----------   | ----------            | :----: | ------------------- |
| MonProjID     | 月租專案代碼  | string | MR66 |
| MonProPeriod  | 總期數		| int | 3 |
| ShortDays		| 短天期天數    | int | 0 |
| MonProjNM		| 月租專案名稱  | string | 測試_汽包機66-3 |
| CarWDHours	| 汽車平日時數	| double | 3.0 |
| CarHDHours	| 汽車假日時數	| double | 3.0 |
| MotoTotalMins | 機車不分平假日分鐘數 	| int | 300.0 |
| WDRateForCar | 汽車平日優惠價 | double | 99.0 |
| HDRateForCar | 機車平日優惠價 | double | 168.0 |
| WDRateForMoto | 機車平日優惠價 | double | 1.0 |
| HDRateForMoto | 機車假日優惠價 | double | 1.2 |
| StartDate | 起日 			| string | 2021/05/18 00:00 |
| EndDate	| 迄日 			| string | 2021/08/16 23:59|
| MonProDisc  	| 注意事項      | string  | 汽包機66-3注意事項 |
| IsMix		| 是否為城市車手 | int | 1 |
| MonthStartDate | 全月租專案起日 | string | 2021/05/18 00:00|
| MonthEndDate | 全月租專案迄日 | string | 2021/08/16 23:59|
| NxtMonProPeriod | 下期續訂總期數 | string | 3 |
| IsUpd | 是否已升級 (0否1是) | int | 0 |
| SubsNxt		| 是否自動續訂 (0否1是) | int | 1 |
| IsChange		| 是否變更下期合約 (0否1是) | int | 0 |
| IsPay 		| 是否當期有繳費 (0否1是) | int | 1 |
| IsMoto	    | 是否為機車0否1是     | int    | 0        |


* Output範例
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "NowCard": {
            "MonProjID": "MR66",
            "MonProPeriod": 3,
            "ShortDays": 0,
            "MonProjNM": "測試_汽包機66-3",
            "CarWDHours": 3.0,
            "CarHDHours": 3.0,
            "MotoTotalMins": 300.0,
            "WDRateForCar": 99.0,
            "HDRateForCar": 168.0,
            "WDRateForMoto": 1.0,
            "HDRateForMoto": 1.2,
            "StartDate": "2021/05/18 00:00",
            "EndDate": "2021/08/16 23:59",
            "MonProDisc": "汽包機66-3注意事項",
			"IsMix": 1,
			"MonthStartDate": "2021/05/26 00:00",
            "MonthEndDate": "2021/08/24 23:59",
            "NxtMonProPeriod": 3,
            "IsUpd": 0,
            "SubsNxt": 1,
            "IsChange": 0,
            "IsPay": 1,
			"IsMoto": 0
        },
        "NxtCard": {
            "IsChange": 0,
            "MonProjID": "MR66",
            "MonProPeriod": 3,
            "ShortDays": 0,
            "MonProjNM": "測試_汽包機66-3",
            "CarWDHours": 3.0,
            "CarHDHours": 3.0,
            "MotoTotalMins": 300.0,
            "WDRateForCar": 99.0,
            "HDRateForCar": 168.0,
            "WDRateForMoto": 1.0,
            "HDRateForMoto": 1.2,
            "StartDate": "2021/08/16 00:00",
            "EndDate": "2021/11/14 23:59",
            "MonProDisc": "汽包機66-3注意事項",
			"IsMix": 1,
			"MonthStartDate": "2021/05/26 00:00",
            "MonthEndDate": "2021/08/24 23:59",
            "NxtMonProPeriod": 3,
            "IsUpd": 0,
            "SubsNxt": 1,
            "IsPay": 1,
			"IsMoto": 0
        }
    }
}
```

---

## GetChgSubsList 變更下期續約列表

### [/api/GetChgSubsList]

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                   | 必要 |  型態  | 範例                           |
| ---------- | -------------------------- | :--: | :----: | ------------------------------ |
| MonProjID     | 方案代碼(key)              |  Y   | string | MR66                          |
| MonProPeriod | 總期數(key)               |  Y  | int    | 3                     |
| ShortDays    | 短期總天數(key)            |  Y  | int    | 0                              |


* input範例
```
{
    "MonProjID"::"MR66",
    "MonProPeriod:3,
    "ShortDays":"0"
}
```

* Output回傳參數說明

| 參數名稱      | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| MyCard       | 資料物件(目前訂閱)        | obj |                 |
| OtrCards     | 資料物件(其他方案)            | list |  　  |


* 資料物件說明(MyCard)

| 參數名稱      | 參數說明             |  型態  | 範例     |
| -----------   | ----------           | :----: | ---------|
| MonProjID     | 方案代碼(key)        | string | MR66     |
| MonProPeriod  | 總期數(key)          | int    | 3        |
| ShortDays	    | 短期總天數(key)      | int    | 0        |
| MonProjNM	| 方案名稱 | string | 測試_汽包機66-3 |
| PeriodPrice	| 方案價格            | int    | 7000     |
| CarWDHours	| 汽車平日時數         | double | 3.0    |
| CarHDHours	| 汽車假日時數         | double | 3.0      |
| MotoTotalMins	| 機車不分平假日分鐘數   | int | 120      |
| WDRateForCar	| 汽車平日優惠費率 | double | 99.0 |
| HDRateForCar	| 假日平日優惠費率 | double | 168.0 |
| WDRateForMoto	| 機車平日優惠費率 | double | 1.0 |
| HDRateForMoto	| 機車假日優惠費率 | double | 1.2 |
| IsDiscount	| 是否為優惠方案0否1是  | int    | 0       |
| IsMix			| 是否為城市車手  | int | 1 |
| IsMoto		| 是否為機車		| int | 0 |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "MyCard": {
            "MonProjID": "MR66",
            "MonProPeriod": 3,
            "ShortDays": 0,
            "MonProjNM": "測試_汽包機66-3",
            "PeriodPrice": 7000,
            "CarWDHours": 3.0,
            "CarHDHours": 3.0,
            "MotoTotalMins": 300,
            "WDRateForCar": 99.0,
            "HDRateForCar": 168.0,
            "WDRateForMoto": 1.0,
            "HDRateForMoto": 1.2,
            "IsDiscount": 0,
			"IsMix": 1,
			"IsMoto": 0
        },
        "OtrCards": [
            {
                "MonProjID": "MR102",
                "MonProPeriod": 3,
                "ShortDays": 0,
                "MonProjNM": "測試_汽包機102-3",
                "PeriodPrice": 7300,
                "CarWDHours": 4.0,
                "CarHDHours": 4.0,
                "MotoTotalMins": 400.0,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "IsDiscount": 0,
				"IsMix":1,
				"IsMoto": 0
            },
            {
                "MonProjID": "MR103",
                "MonProPeriod": 3,
                "ShortDays": 0,
                "MonProjNM": "測試_汽包機103-3",
                "PeriodPrice": 7800,
                "CarWDHours": 4.0,
                "CarHDHours": 3.0,
                "MotoTotalMins": 300.0,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "IsDiscount": 0,
				"IsMix":1,
				"IsMoto": 0
            }
        ]
    }
}
```

---

## GetUpSubsList 取得訂閱制升轉列表

### [/api/GetUpSubsList]

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]
  
* input傳入參數說明

| 參數名稱   | 參數說明                   | 必要 |  型態  | 範例                           |
| ---------- | -------------------------- | :--: | :----: | ------------------------------ |
| MonProjID     | 方案代碼(key)              |  Y   | string | MR66                          |
| MonProPeriod | 總期數(key)               |  Y  | int    | 3                      |
| ShortDays    | 短期總天數(key)            |  Y  | int    | 0                              |


* input範例
```
{
    "MonProjID"::"MR66",
    "MonProPeriod:3,
    "ShortDays":"0",
}
```


* Output回傳參數說明

| 參數名稱      | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| NorCards     | 資料物件(只有機車時數或只有機車時數) | list |                 |
| MixCards     | 資料物件(同時有汽車及機車時數)     | list |  　       |


* 資料物件說明(NorCards/MixCards)

| 參數名稱      | 參數說明             |  型態  | 範例     |
| -----------   | ----------           | :----: | ---------|
| MonProjID     | 方案代碼(key)        | string | MR66     |
| MonProPeriod  | 總期數(key)          | int    | 3        |
| ShortDays	    | 短期總天數(key)      | int    | 0        |
| MonProjNM	| 專案名稱 | string | MR66測試 |
| PeriodPrice	| 方案價格             | int    | 7000     |
| CarWDHours	| 汽車平日時數         | double | 10       |
| CarHDHours	| 汽車假日時數         | double | 0        |
| MotoTotalMins	| 機車不分平假日分鐘數 | int | 120      |
| WDRateForCar	| 汽車平日優惠價格 | double | 99.0 |
| HDRateForCar	| 汽車假日優惠價格 | double | 168.0 |
| WDRateForMoto	| 機車平日優惠價格 | double | 1.0 |
| HDRateForMoto	| 機車假日優惠價格 | double | 1.2 |
| IsDiscount	| 是否為優惠方案0否1是 | int    | 1        |
| IsMix			| 是否為城市車手 | int | 1 |
| AddPrice		| 加價購金額	| int | 1000 |
| IsMoto		| 是否為機車	| int | 0 |
| UseUntil		| 使用期限		| string | 2021/08/25 23:59 |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
		"NorCards": [
            {
                "MonProjID": "MR101",
                "MonProPeriod": 3,
                "ShortDays": 0,
                "MonProjNM": "測試_汽平8000",
                "PeriodPrice": 8000,
                "CarWDHours": 60.0,
                "CarHDHours": 0.0,
                "MotoTotalMins": 0,
                "WDRateForCar": 90.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.5,
                "HDRateForMoto": 1.5,
                "IsDiscount": 0,
				"IsMix" : 0,
				"AddPrice": 1000,
				"IsMoto": 0,
				"UseUntil": "2021/08/25 23:59"
            }	
		],
		"MixCards": [{
				"MonProjID": "MR102",
				"MonProPeriod": 3,
				"ShortDays": 0,
				"MonProjNM": "測試_汽包機102-3",
				"PeriodPrice": 7300,
				"CarWDHours": 4.0,
				"CarHDHours": 4.0,
				"MotoTotalMins": 400,
				"WDRateForCar": 99.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 1.0,
				"HDRateForMoto": 1.2,
				"IsDiscount": 0,
				"IsMix" : 1,
				"AddPrice": 2000,
				"IsMoto": 0,
				"UseUntil": "2021/08/25 23:59"
			}, 
			{
				"MonProjID": "MR103",
				"MonProPeriod": 3,
				"ShortDays": 0,
				"MonProjNM": "測試_汽包機103-3",
				"PeriodPrice": 7800,
				"CarWDHours": 4.0,
				"CarHDHours": 3.0,
				"MotoTotalMins": 300,
				"WDRateForCar": 99.0,
				"HDRateForCar": 168.0,
				"WDRateForMoto": 1.0,
				"HDRateForMoto": 1.2,
				"IsDiscount": 0,
				"IsMix":1,
				"AddPrice": 3000,
				"IsMoto": 0,
				"UseUntil": "2021/08/25 23:59"
			}
		]
	}
}
```

---

## GetSubsHist 訂閱制歷史紀錄

### [/api/GetSubsHist/DoGetSubsHist]

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

  不需傳入參數

* Output回傳參數說明

| 參數名稱      | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Hists       | 資料物件                  | list |  　     |



* 資料物件說明(Hists)

| 參數名稱    | 參數說明                 | 型態    | 範例                  |
| --------     | --------             | :--:    |----------------------|
| MonProjID    | 方案代碼(key)         | string  | MR66                 |
| MonProPeriod | 總期數(key)           | int     | 3                    |
| ShortDays    | 短期總天數(key)        | int     | 0                    |
| MonProjNM    | 車型名稱               | string  | MR66測試              |
| PeriodPrice  | 方案價格               | int     | 7000                 |
| CarWDHours   | 汽車平日時數            | double  | 3.0                 |
| CarHDHours   | 汽車假日時數            | double  | 3.0                |
| MotoTotalMins| 機車不分平假日分鐘數     | int | 120                  |
| WDRateForCar | 汽車平日優惠價格 | double | 99.0 |
| HDRateForCar | 汽車假日優惠價格 | double | 168.0 |
| WDRateForMoto | 機車平日優惠價格 | double | 1.0 |
| HDRateForMoto | 機車假日優惠價格 | double | 1.2 |
| PayDate | 付款日期 | string | 2021/05/21 11:09 |
| IsMoto       | 是否為機車0否1是        | int     | 0                    |
| StartDate  | 月租起日 | string | 2021/05/18 |
| EndDate      | 月租迄日 | string | 2021/06/17 |
| PerNo        | 付款期數 | int     | 1 |
| MonthlyRentId| 月租Id | int64   | 911 |
| InvType      | 發票方式 | string  | 捐贈碼 |
| unified_business_no| 統一編號 | string  | 12345678 |
| invoiceCode  | 發票號碼 | string  | AB12345678 |
| invoice_date | 發票日期 | string  | 2021/05/18 |
| invoice_price| 發票金額 | int     | 7000 |
| IsMix | 是否為城市車手0否1是 | int | 0 |
| CARRIERID 	| 載具號碼 | string | /SI2Q4FQ |
| NPOBAN		| 捐贈碼  | string  | 1234 |
| InvoiceType	| 發票方式(1:捐贈2:會員載具 3:二聯 4:三聯 5:手機條碼 6:自然人憑證 | int | 2 |
| NPOBAN_Name	| 捐贈單位 | string | 財團法人中華民國兒童癌症基金會 |





* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "Hists": [
            {
                "MonProjID": "MR66",
                "MonProPeriod": 3,
                "ShortDays": 0,
                "MonProjNM": "測試_汽包機66-3",
                "PeriodPrice": 7000,
                "CarWDHours": 3.0,
                "CarHDHours": 3.0,
                "MotoTotalMins": 300,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "PayDate": "2021/05/17 23:00",
                "IsMoto": 0,
                "StartDate": "2021/05/18",
                "EndDate": "2021/06/17",
                "PerNo": 1,
                "MonthlyRentId": 911,
                "InvType": "捐贈碼",
                "unified_business_no": "12345678",
                "invoiceCode": "A12345678",
                "invoice_date": "2021/05/19",
                "invoice_price": 7000,
                "IsMix":0,
				"CARRIERID": "",
                "NPOBAN": "88888",
				"InvoiceType": 1,
				"NPOBAN_Name": "財團法人中華民國兒童癌症基金會"
            },
            {
                "MonProjID": "MR66",
                "MonProPeriod": 3,
                "ShortDays": 0,
                "MonProjNM": "測試_汽包機66-3",
                "PeriodPrice": 7000,
                "CarWDHours": 3.0,
                "CarHDHours": 3.0,
                "MotoTotalMins": 300,
                "WDRateForCar": 99.0,
                "HDRateForCar": 168.0,
                "WDRateForMoto": 1.0,
                "HDRateForMoto": 1.2,
                "PayDate": "2021/07/16 23:00",
                "IsMoto": 0,
                "StartDate": "2021/07/17",
                "EndDate": "2021/08/16",
                "PerNo": 3,
                "MonthlyRentId": 913,
                "InvType": "會員載具",
                "unified_business_no": "12345678",
                "invoiceCode": "A12345678",
                "invoice_date": "2021/05/19",
                "invoice_price": 7000,
                "IsMix":0,
				"CARRIERID": "",
                "NPOBAN": "",
				"InvoiceType": 2,
				"NPOBAN_Name": ""
            }
        ]
    }
}
```

---

## GetSubsHist-del 訂閱制歷史紀錄-刪除

### [/api/GetSubsHist/DoDelSubsHist]

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱          | 參數說明                   | 必要 |  型態  | 範例                           |
| ------------------| -------------------------- | :--: | :----: | ------------------------------ |
| MonthlyRentId     |                            |  Y   | int    | 912 |


* input範例
```
{
     "MonthlyRentId":912,
}
```

* Output回傳參數說明

| 參數名稱  | 參數說明               | 型態 | 範例 |
| --------- | ---------------------- | ---- | ---- |
| DelResult | 刪除結果(0:失敗1:成功) | int  | 1    |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "DelResult": 1
    }
}
```

---

## GetArrsSubsList 訂閱制欠費查詢

### [/api/GetArrsSubsList/DoGetArrsSubsList]

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

  不須傳入參數


* Output回傳參數說明

| 參數名稱      | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| TotalArresPrice | 欠費總計 | int |                 |
| Cards     | 欠費列表 | list |                 |
| Cards-StartDate | 月租起日         | string |  　 |
| Cards-EndDate | 月租迄日 | string | |
| Cards-ProjNm | 月租專案名稱 | string | |
| Cards-CarTypePic | 車型對照檔名 | string | priusC |
| Cards-Arrs | 欠費明細 | list | |



* 資料物件說明(Arrs)

| 參數名稱    | 參數說明               | 型態   | 範例                  |
| --------   | --------             | :--:   |----------------------|
| Period     | 繳費期數     | int    |                      |
| ArresPrice | 繳費金額       | int    |  　            |



* Output範例


```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
		"TotalArresPrice": 27000,
		"Cards": [
			{
				"StartDate": "2021/05/19",
				"EndDate": "2021/08/17",
				"ProjNm": "測試_機車2000",
				"CarTypePic": "iretScooter",
				"Arrs": [
					{
						"Period": 1,
						"ArresPrice": 2000
					},
					{
						"Period": 2,
						"ArresPrice": 2000
					},
					{
						"Period": 3,
						"ArresPrice": 2000
					}
				]
			},
			{
				"StartDate": "2021/05/18",
				"EndDate": "2021/08/16",
				"ProjNm": "測試_汽包機66-3",
				"CarTypePic": "priusC",
				"Arrs": [
					{
						"Period": 1,
						"ArresPrice": 7000
					},
					{
						"Period": 2,
						"ArresPrice": 7000
					},
					{
						"Period": 3,
						"ArresPrice": 7000
					}
				]
			}
		]
	}
}
```

---

## SetSubsNxt 設定自動續約

### [/api/SetSubsNxt]

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱          | 參數說明                   | 必要 |  型態  | 範例                           |
| ------------------| -------------------------- | :--: | :----: | ------------------------------ |
| MonProjID    | 專案編號(key)          |  Y   | string | MR66 |
| MonProPeriod | 期數(key)             |  Y   |  int   | 3    |
| ShortDays    | 短天期(key)           |  Y   |  int   | 0    |
| AutoSubs	   | 設定自動續約(0否,1是) |  N   |  int   | 0    |


* input範例
```
{
     "MonProjID":"MR66",
	 "MonProPeriod":3,
	 "ShortDays":0,
	 "AutoSubs":1
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Result       | 是否成功                |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼                  | string | 000000        |
| NeedRelogin  | 是否需重新登入          |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新      |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息                | string | Success       |
| Data         | 資料物件                |        |        　       |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```


# 預約以及訂單相關



## OrderDetail 訂單明細

### [/api/OrderDetail/]

* 20210819修改

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H10455246 |

* input範例

```
{
    "OrderNo": "H10455246"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| OrderNo      | 訂單編號           | string | H10455246     |
| ContactURL   | 合約網址			| string | 				 |
| Operator     | 營運商				| string | supplierIrent |
| CarTypePic   | 車輛圖片			| string | iretScooter   |
| CarNo		   | 車號				| string | RAA-1122      |
| Seat		   | 座椅數				| int    | 5			 |
| CarBrend     | 汽車廠牌			| string | KYMCO		 |
| CarTypeName  | 車型名稱			| string | MANY-110		 |
| StationName  | 據點名稱			| string | iRent路邊租還[機車]_台北 |
| OperatorScore| 評分				| float  | 5.0			 |
| ProjName	   | 專案名稱			| string | 10載便利北北桃 |
| CarRentBill  | 車輛租金			| int	 | 2000			 |
| TotalHours   | 使用時數			| string | 0天0時0分	 |
| MonthlyHours | 月租折抵			| string | 0天0時0分	 |
| GiftPoint	   | 折抵時數			| string | 0天0時0分	 |
| PayHours	   | 計費時數			| string | 0天0時0分	 |
| MileageBill  | 里程費				| int	 | 100			 |
| InsuranceBill| 安心服務費			| int 	 | 100 			 |
| EtagBill     | eTag費用			| int	 | 10			 |
| OverTimeBill | 逾時費				| int	 | 100			 |
| ParkingBill  | 代收停車費			| int 	 | 100			 |
| TransDiscount |轉乘優惠折抵		| int 	 | 100			 |
| TotalBill    | 總金額				| int 	 | 1000			 |
| InvoiceType  | 發票類型			| string | 1:愛心碼 2:email 3:二聯 4:三聯 5:手機條碼 6:自然人憑證 |
| CARRIERID    | 載具條碼			| string | 				 |
| NPOBAN  	   | 捐贈碼				| string |  			 |
| NPOBAN_Name  | 捐贈協會名稱		| string | 				 |
| Unified_business_no | 統編		| string | 50885758		 |
| InvoiceNo	   | 發票號碼			| string | AA12345678    |
| InvoiceDate  | 發票日期			| string | 2021-03-01 	 |
| InvoiceBill  | 發票金額			| int    | 1000			 |
| InvoiceURL   | 發票網址			| string | 				 |
| StartTime    | 開始時間			| string | 2021-05-14 00:02 |
| EndTime	   | 結束時間			| string | 2021-05-14 00:02 |
| Millage	   | 里程				| float  | 1234.5		 |
| CarOfArea	   | 據點區域			| string | 北北桃		 |
| DiscountAmount | 優惠折抵金額 	| int 	 | 100			 |
| DiscountName | 折抵專案名稱		| string | 				 |
| CtrlBill	   | 營損-車輛調度費	| int 	 | 0			 |
| ClearBill    | 營損-清潔費		| int    | 0			 |
| EquipBill	   | 營損-物品損壞		| int 	 | 0			 |
| ParkingBill2 | 營損-非約定停車費  | int    | 0			 |
| TowingBill   | 營損-拖吊費		| int    | 0			 |
| OtherBill	   | 營損-其他費用		| int    | 0			 |
| UseOrderPrice | 使用訂金  		| int    | 0 			 |
| ReturnOrderPrice | 返還訂金		| int 	 | 0 			 |
| ChangePoint  | 換電時數			| int 	 | 0			 |
| ChangeTimes  | 換電次數			| int    | 0			 |
| RSOC_S	   | 取車電量			| float  | 0			 |
| RSOC_E 	   | 還車電量			| float  | 0			 |
| RewardPoint  | 獎勵時數			| int    | 0			 |
| TotalRewardPoint | 總回饋時數		| int    | 0 			 |
| RenterType | 共同承租人類型      | int    | 1:主要承租人  2:共同承租人  |




* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
		"OrderNo": "H10455246",
        "ContactURL": "",
        "Operator": "supplierIrent",
        "CarTypePic": "iretScooter",
        "CarNo": "EWA-0132",
        "Seat": 2,
        "CarBrend": "KYMCO",
        "CarTypeName": "MANY-110",
        "StationName": "iRent路邊租還[機車]_台北",
        "OperatorScore": 5.0,
        "ProjName": "10載便利北北桃",
        "CarRentBill": 0,
        "TotalHours": "0天0時0分",
        "MonthlyHours": "0天0時0分",
        "GiftPoint": "0天0時6分",
        "PayHours": "0天0時0分",
        "MileageBill": 0,
        "InsuranceBill": 0,
        "EtagBill": 0,
        "OverTimeBill": 0,
        "ParkingBill": 0,
        "TransDiscount": 0,
        "TotalBill": 0,
        "InvoiceType": 2,
        "CARRIERID": "",
        "NPOBAN": "",
        "NPOBAN_Name": "",
        "Unified_business_no": "",
        "InvoiceNo": "",
        "InvoiceDate": "",
        "InvoiceBill": 0,
        "InvoiceURL": "",
        "StartTime": "2021-05-14 00:02",
        "EndTime": "2021-05-14 00:02",
        "Millage": 0.0,
        "CarOfArea": "北北桃",
        "DiscountAmount": 0,
        "DiscountName": "",
        "CtrlBill": 0,
        "ClearBill": 0,
        "EquipBill": 0,
        "ParkingBill2": 0,
        "TowingBill": 0,
        "OtherBill": 0,
        "UseOrderPrice": 0,
        "ReturnOrderPrice": 0,
		"ChangePoint" : 0,
		"ChangeTimes" : 0,
		"RSOC_S": 90.0,
		"RSOC_E": 80.0,
		"RewardPoint": 0,
		"TotalRewardPoint" : 0,
        "RenterType" 1
    
}
```


------

## Booking 預約

### [/api/Booking/]

* 20210609補上

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| ProjID	| 專案代碼 |  Y   | string | P735 |
| SDate		| 預計取車時間 | N | string |  |
| EDate		| 預計還車時間 | N | string |  |
| CarNo 	| 車號		| Y | string | RCG-0521 |
| CarType	| 車型代碼	| Y | string | |
| Insurance | 是否加購安心服務 | Y | int | 1 |
| StationID | 據點代碼 | Y | string | X0II |
| MonId		| 選擇的訂閱制月租 | Y | int | 123456 |


* input範例 -同站預約

```
{
	"ProjID": "P735",
	"SDate": "2021-05-11 10:30:00",
    "EDate": "2021-05-11 11:30:00",
    "CarNo": "",
    "CarType": "COROLLA CROSS",
    "Insurance": 0,
    "StationID": "XXXX"
    "MonId": 0
}
```

* input範例 -機車預約
```
{
	"ProjID": "P686",
	"SDate": "",
	"EDate": "",
	"CarNo": "EWJ-8339",
	"CarType": "MANY-110",
	"Insurance": 0,
	"StationID": ""
	"MonId": 0
}
```


* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| OrderNo      | 訂單編號           | string | H10455246     |
| LastPickTime   | 最晚的取車時間	| string | 20210608020120 |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "OrderNo": "H10791575",
        "LastPickTime": "20210608020120"
    }
}
```
------

## BookingQuery 訂單列表

### [/api/BookingQuery/]

* 202108020補上

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo    | 訂單編號 |  N   | string | H12044254 |

* input範例 -不傳入參數

```

```

* input範例 -傳入參數

```
{
    "OrderNo":H12044254
}
```


* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| OrderObj      | 訂單明細物件           | list |      |

* OrderObj回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| StationInfo      | 據點資訊物件         | object |      |
| Operator |       營運商     | string | supplierIrent       |
| OperatorScore |     評分       | float | 5.0       |
| CarTypePic |   車輛圖片         | string | yaris       |
| CarNo |        車號    | string | RDH-2905 |
| CarBrend |     廠牌       | string | TOYOTA      |
| CarTypeName |    車型名稱        | string | YARIS      |
| Seat |      座椅數      | int | 5      |
| ParkingSection |   停車格位置         | string|       |
| IsMotor |    是否為機車        | int |  0:否 1:是    |
| CarOfArea |      車輛圖顯示地區      | string | 台北市       |
| CarLatitude |      車緯度      | decimal |25.0726200   |
| CarLongitude |     車經度       | decimal | 121.5423700 |
| MotorPowerBaseObj |     機車電力資訊    | object |  當ProjType=4時才有值   |
| ProjType | 專案類型<br>0:同站 3:路邊 4:機車 | int | 0 |
| ProjName | 專案名稱 | string | 同站汽車110起推廣專案 |
| WorkdayPerHour | 平日每小時費用 | int | 110 |
| HolidayPerHour | 假日每小時費用 | int | 168 |
| MaxPrice | 每日上限 | int | 0 |
| MaxPriceH | 假日上限 | int | 0 |
| MotorBasePriceObj | 機車費用 | object | 當ProjType=4時才有值 |
| OrderStatus | 訂單狀態<br>-1:前車未還（未到站） <br/>0:可取車<br/>1:用車中<br/>2:延長用車中<br/>3:準備還車<br/>4:逾時<br/>5:還車流程中（未完成還車） | int | 1 |
| OrderNo | 訂單編號 | string | H12044254 |
| StartTime | 預計取車時間 | string | 2021-08-30 11:00:00 |
| PickTime | 實際取車時間 | string | 2021-08-30 10:51:16 |
| ReturnTime | 實際還車時間 | string | 2021-08-30 11:27:04 |
| StopPickTime | 取車截止時間 | string | 2021-08-30 11:15:00 |
| StopTime | 預計還車時間 | string | 2021-08-30 12:00:00 |
| OpenDoorDeadLine | 使用期限 | string | 2021-08-30 11:42:34 |
| CarRentBill | 預估租金 | int | 110 |
| MileagePerKM | 每一公里里程費 | float | 3.1 |
| MileageBill | 預估里程費 | int | 62 |
| Insurance | 是否可以使用安心服務 | int | 1:可 0:否 |
| InsurancePerHour | 安心保險每小時 | int | 50 |
| InsuranceBill | 預估安心保險費用 | int | 0 |
| TransDiscount | 轉乘優惠 | int | 0 |
| Bill | 預估總金額 | int | 172 |
| DailyMaxHour | 單日計費上限時數 | int | 10 |
| CAR_MGT_STATUS | 取還車狀態<br>0 = 尚未取車<br/>1 = 已經上傳出車照片<br/>2 = 已經簽名出車單<br/>3 = 已經信用卡認證<br/>4 = 已經取車(記錄起始時間)<br/>11 = 已經紀錄還車時間<br/>12 = 已經上傳還車角度照片<br/>13 = 已經上傳還車車損照片<br/>14 = 已經簽名還車單<br/>15 = 已經信用卡付款<br/>16 = 已經檢查車輛完成並已經解除卡號 | int | 4 |
| AppStatus | 1:尚未到取車時間(取車時間半小時前)<br/>2:立即換車(取車前半小時，前車尚未完成還車)<br/>3:開始使用(取車時間半小時前)<br/>4:開始使用-提示最晚取車時間(取車時間後~最晚取車時間)<br/>5:操作車輛(取車後) 取車時間改實際取車時間<br/>6:操作車輛(準備還車)<br/>7:物品遺漏(再開一次車門)<br/>8:鎖門並還車(一次性開門申請後) | int | 6 |
| RenterType | 承租人類型<br>1:主要承租人 2:共同承租人 | int | 1 |
* StationInfo回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| StationID      | 據點代碼         | string | X0IN  |
| StationName |       據點名稱     | string | iRent-Toyota濱江營業所站 |
| Tel |     電話       | string | 02-2516-3816 |
| ADDR |   地址         | string | 台北市中山區濱江街269號 |
| Latitude |        緯度    | string | 25.072589 |
| Longitude |     經度       | string | 121.542617 |
| Content |    其他說明        | string | YARIS      |
| IsRent |            | string | null |
| ContentForAPP |   據點描述（app顯示）         | string| 1.濱江營業所前方停車場\n2.固定招牌旁車位\n3.自由進出\n4.請勿停在專屬車位外  |
| IsRequiredForReturn |    還車位置資訊必填        | int |  0   |
| StationPic |      據點照片      | list |        |

* StationPic回傳參數說明

| 參數名稱       | 參數說明 |  型態  | 範例                                                         |
| -------------- | -------- | :----: | ------------------------------------------------------------ |
| StationPic     | 據點照片 | string | https://irentv2data.blob.core.windows.net/station/X0IN_1_20210209000000.png |
| PicDescription | 據點說明 | string | 停車場位置\n                                                 |

* MotorPowerBaseObj回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Power      | 剩餘電量         | float |      |
| RemainingMileage |       剩餘里程     | float |        |

* MotorBasePriceObj回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| BaseMinutes   | 剩餘電量         | int |           |
| BasePrice     |  剩餘里程        | int |           |
| PerMinutesPrice | 剩餘電量       | float |         |
| MaxPrice       | 剩餘里程        | int |           |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "OrderObj": [
            {
                "StationInfo": {
                    "StationID": "X0IN",
                    "StationName": "iRent-Toyota濱江營業所站",
                    "Tel": "02-2516-3816",
                    "ADDR": "台北市中山區濱江街269號",
                    "Latitude": 25.072589,
                    "Longitude": 121.542617,
                    "Content": "",
                    "IsRent": null,
                    "ContentForAPP": "1.濱江營業所前方停車場\n2.固定招牌旁車位\n3.自由進出\n4.請勿停在專屬車位外",
                    "IsRequiredForReturn": 0,
                    "StationPic": [
                        {
                            "StationPic": "https://irentv2data.blob.core.windows.net/station/X0IN_1_20210209000000.png",
                            "PicDescription": "停車場位置\n"
                        },
                        {
                            "StationPic": "https://irentv2data.blob.core.windows.net/station/X0IN_2_20210209000000.png",
                            "PicDescription": "停車位置\n"
                        }
                    ]
                },
                "Operator": "supplierIrent",
                "OperatorScore": 5.0,
                "CarTypePic": "yaris",
                "CarNo": "RDH-2905  ",
                "CarBrend": "TOYOTA",
                "CarTypeName": "YARIS",
                "Seat": 5,
                "ParkingSection": "",
                "IsMotor": 0,
                "CarOfArea": "台北市",
                "CarLatitude": 25.0726200,
                "CarLongitude": 121.5424000,
                "MotorPowerBaseObj": null,
                "ProjType": 0,
                "ProjName": "同站汽車110起推廣專案",
                "WorkdayPerHour": 110,
                "HolidayPerHour": 168,
                "MaxPrice": 0,
                "MaxPriceH": 0,
                "MotorBasePriceObj": null,
                "OrderStatus": 5,
                "OrderNo": "H12289921",
                "StartTime": "2021-08-30 11:00:00",
                "PickTime": "2021-08-30 10:51:16",
                "ReturnTime": "2021-08-30 11:27:04",
                "StopPickTime": "2021-08-30 11:15:00",
                "StopTime": "2021-08-30 12:00:00",
                "OpenDoorDeadLine": "2021-08-30 11:42:34",
                "CarRentBill": 110,
                "MileagePerKM": 3.1,
                "MileageBill": 62,
                "Insurance": 1,
                "InsurancePerHour": 50,
                "InsuranceBill": 0,
                "TransDiscount": 0,
                "Bill": 172,
                "DailyMaxHour": 10,
                "CAR_MGT_STATUS": 16,
                "AppStatus": 7,
                "RenterType": 1
            }
        ]
    }
}
```

----

## BookingFinishQuery 完成的訂單查詢

### [/api/BookingFinishQuery/]

* 202108020補上

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| NowPage    | 現在頁碼 |  N   | int | 1 |
| ShowOneYear    | 顯示一整年 (202101 該參數已無作用) |  Y   | int | 0:否 20XX代表取出該年度的所有訂單 |

* input範例

```
{
    "NowPage" : 1,
    "ShowOneYear" : 1
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| OrderFinishObjs      | 完成的訂單清單           | list |      |

* OrderFinishObjs回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| RentYear      | 年分         | int |      |
| OrderNo |       訂單編號     | string | supplierIrent       |
| CarNo |     車號       | string | 5.0       |
| ProjType |   專案類型         | int | 0:同站 3:路邊 4:機車       |
| RentDateTime |        取車時間 月日時分    | string |        |
| TotalRentTime |     總租用時數       | string |       |
| Bill |    總租金        | string | YARIS      |
| UniCode |      統編      | string | 5      |
| StationName |            | string|       |
| CarOfArea |    車輛圖顯示地區        | string |  0:否 1:是    |
| CarTypePic |      車輛圖片      | string | 台北市       |
| IsMotor |      是否為機車      | int |1:是 0:否  |
| IsJointOrder | 是否為共同承租訂單      | int | 1:是 0:否  |

* Output範例

```
 {
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "TotalPage": 6,
        "OrderFinishObjs": [
            {
                "RentYear": 2021,
                "OrderNo": "H12027037",
                "CarNo": "EWH-7321",
                "ProjType": 4,
                "RentDateTime": "08月19日 15:28",
                "TotalRentTime": "0天0時2分",
                "Bill": 0,
                "UniCode": "",
                "StationName": "iRent路邊租還[機車]_台北",
                "CarOfArea": "北北桃",
                "CarTypePic": "iretScooter",
                "IsMotor": 1,
                "IsJointOrder":"0"
            },
            {
                "RentYear": 2021,
                "OrderNo": "H11700564",
                "CarNo": "EWJ-1018",
                "ProjType": 4,
                "RentDateTime": "08月04日 13:17",
                "TotalRentTime": "0天0時4分",
                "Bill": 12,
                "UniCode": "",
                "StationName": "iRent路邊租還[機車]_台北",
                "CarOfArea": "北北桃",
                "CarTypePic": "iretScooter",
                "IsMotor": 1,
                "IsJointOrder":"0"
            }
        ]
    }
}
```

-----

## BookingDelete 刪除訂單

### [/api/BookingDelete/]

* 20210820補上

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H0000029 |


* input範例

```
{
    "OrderNo" : "H0000029"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```

---

## GetOrderInsuranceInfo 訂單安心服務資格及價格查詢

### [/api/GetOrderInsuranceInfo/]

* 20210825新增

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H0000029 |


* input範例

```
{
    "OrderNo" : "H0000029"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Insurance      | 可否使用安心服務           | int | 0:否 1:是      |
| MainInsurancePerHour   | 主承租人每小時安心服務價格  | int | 50 |
| JointInsurancePerHour  | 單一共同承租人每小時安心服務價格  | int | 若該訂單沒有共同承租邀請對象，該欄位為0 |
| JointAlertMessage  | 共同承租提示訊息| string | 若該訂單沒有未回應的共同承租邀被邀請人，該欄位為空字串 |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data":
    {
        "Insurance": 1,
        "MainInsurancePerHour": 50,
        "JointInsurancePerHour": 20,
        "JointAlertMessage": "還有人沒有回覆邀請喔!快通知對方開啟通知中心確認"
    }
}
```


---

# 車輛調度停車場相關

## GetMotorParkingData 取得機車調度停車場

### [/api/GetMotorParkingData/]

* 20210528新增

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例        |
| --------- | ---------- | :--: | :----: | ----------- |
| ShowALL   | 顯示全部   |  Y   | int    | 0:不要 1:要 |
| Latitude  | 緯度       |  Y   | double | 25.068740   |
| Longitude | 經度       |  Y   | double | 121.531652  |
| Mode      | 停車場類型 |  Y   | int    | 0:一般(調度) 1:特約(車麻吉及其他) 2:全部顯示 |
| Radius    | 半徑       |  Y   | double | 3.5  |

* input範例

```
{
    "ShowALL": 0,
    "Latitude": 25.068740,
    "Longitude": 121.531652,
    "Mode": 0,
    "Radius": 5
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| ParkingType  | 停車場類型         | int    | 0:一般 1:特約(車麻吉及其他)     |
| ParkingName  | 停車場名稱         | string | 信義國小地下停車場     |
| ParkingAddress  | 停車場地址      | string | 台北市信義區松勤街60號地下     |
| Latitude     | 緯度               |  decimal | 25.068740   |
| Longitude    | 經度               |  decimal | 121.531652  |
| OpenTime     | 開始時間           |  DateTime | 2020-12-16T00:00:00  |
| Longitude    | 結束時間           |  DateTime | 2099-12-31T00:00:00  |





* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "ParkingObj": [
            {
                "ParkingType": 0,
                "ParkingName": "台北承德一站停車場",
                "ParkingAddress": "台北市大同區承德路二段206號",
                "Longitude": 121.518037,
                "Latitude": 25.061744,
                "OpenTime": "2020-12-16T00:00:00",
                "CloseTime": "2099-12-31T00:00:00"
            },
            {
                "ParkingType": 0,
                "ParkingName": "台北新生停車場",
                "ParkingAddress": "臺北市中山區民族東路與吉林路口(新生公園溫水游泳池旁)",
                "Longitude": 121.531652,
                "Latitude": 25.06874,
                "OpenTime": "2020-12-16T00:00:00",
                "CloseTime": "2099-12-31T00:00:00"
            },
            {
                "ParkingType": 0,
                "ParkingName": "大龍國小地下停車場",
                "ParkingAddress": "台北市大同區哈密街47號地下",
                "Longitude": 121.517376,
                "Latitude": 25.074141,
                "OpenTime": "2020-12-16T00:00:00",
                "CloseTime": "2099-12-31T00:00:00"
            },
            {
                "ParkingType": 0,
                "ParkingName": "成淵高中地下停車場",
                "ParkingAddress": "台北市大同區承德路二段235號地下[車牌辨識]",
                "Longitude": 121.520076,
                "Latitude": 25.060341,
                "OpenTime": "2020-12-16T00:00:00",
                "CloseTime": "2099-12-31T00:00:00"
            }
        ]
    }
}
```

---

## GetParkingData 取得汽車調度停車場

### [/api/GetParkingData/]

* 20210602新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例        |
| --------- | ---------- | :--: | :----: | ----------- |
| ShowALL   | 顯示全部   |  Y   | int    | 0:不要 1:要 |
| Latitude  | 緯度       |  Y   | double | 25.068740   |
| Longitude | 經度       |  Y   | double | 121.531652  |
| Mode      | 停車場類型 |  Y   | int    | 0:一般(調度) 1:特約(車麻吉及其他) 2:全部顯示 |
| Radius    | 半徑       |  Y   | double | 3.5  |

* input範例

```
{
    "ShowALL": 0,
    "Latitude": 25.068740,
    "Longitude": 121.531652,
    "Mode": 0,
    "Radius": 5
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| ParkingType  | 停車場類型         | int    | 0:一般 1:特約(車麻吉及其他)     |
| ParkingName  | 停車場名稱         | string | 信義國小地下停車場     |
| ParkingAddress  | 停車場地址      | string | 台北市信義區松勤街60號地下     |
| Latitude     | 緯度               |  decimal | 25.068740   |
| Longitude    | 經度               |  decimal | 121.531652  |
| OpenTime     | 開始時間           |  DateTime | 2020-12-16T00:00:00  |
| Longitude    | 結束時間           |  DateTime | 2099-12-31T00:00:00  |





* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "ParkingObj": [
            {
                "ParkingType": 0,
                "ParkingName": "台北承德一站停車場",
                "ParkingAddress": "台北市大同區承德路二段206號",
                "Longitude": 121.518037,
                "Latitude": 25.061744,
                "OpenTime": "2020-12-16T00:00:00",
                "CloseTime": "2099-12-31T00:00:00"
            },
            {
                "ParkingType": 0,
                "ParkingName": "台北新生停車場",
                "ParkingAddress": "臺北市中山區民族東路與吉林路口(新生公園溫水游泳池旁)",
                "Longitude": 121.531652,
                "Latitude": 25.06874,
                "OpenTime": "2020-12-16T00:00:00",
                "CloseTime": "2099-12-31T00:00:00"
            },
            {
                "ParkingType": 0,
                "ParkingName": "大龍國小地下停車場",
                "ParkingAddress": "台北市大同區哈密街47號地下",
                "Longitude": 121.517376,
                "Latitude": 25.074141,
                "OpenTime": "2020-12-16T00:00:00",
                "CloseTime": "2099-12-31T00:00:00"
            },
            {
                "ParkingType": 0,
                "ParkingName": "成淵高中地下停車場",
                "ParkingAddress": "台北市大同區承德路二段235號地下[車牌辨識]",
                "Longitude": 121.520076,
                "Latitude": 25.060341,
                "OpenTime": "2020-12-16T00:00:00",
                "CloseTime": "2099-12-31T00:00:00"
            }
        ]
    }
}
```
----

# iRent共同承租人機制

## JointRentInviteeListQuery 共同承租人邀請清單查詢 
### [/api/JointRentInviteeListQuery/]

* 20210819新增

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例        |
| --------- | ---------- | :--: | :----: | ----------- |
| OrderNo   | 訂單編號   |  Y   | string    | H10791575 |


* input範例

```
{
    "OrderNo": "H10791575",
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| OrderNo  | 訂單編號         | string    | H10791575     |
| Invitees | 被邀請人明細 | list | |

* Invitees 參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| QueryId | 邀請時輸入的ID或手機 |string | 0911001001 |
| InviteeId  | 被邀請人ID         | string | A140584782     |
| InviteeName  | 被邀請人姓名      | string | 王曉明    |
| InvitationStatus     | 邀請狀態               |  string | Y:已接受 N:已拒絕 F:已取消 S:邀請中   |





* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "OrderNo":"H10791575"
        "Invitees": [
            {
                "QueryId" : "0911001001",
                "InviteeId" : "A140584782",
                "InviteeName" : "王一明",
                "InvitationStatus" : "Y"
            },
            {
                "QueryId" : "0911001002",
                "InviteeId": "A140584783",
                "InviteeName": "王二明",
                "InvitationStatus": "N"
            },
            {
                "QueryId" : "A140584784",
                "InviteeId": "A140584784",
                "InviteeName": "王三明",
                "InvitationStatus": "F"
            },
            {
                "QueryId" : "A140584785",
                "InviteeId": "A140584785",
                "InviteeName": "王四明",
                "InvitationStatus": "S"
            }
        ]
    }
}
```
## JointRentInviteeVerify 共同承租人邀請檢核
### [/api/JointRentInviteeVerify/]

* 20210819新增

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例        |
| --------- | ---------- | :--: | :----: | ----------- |
| QueryId | 要邀請的ID或手機   |  Y   | string    | 0911001001 |
| OrderNo  | 訂單編號       |  Y   | string | H10791575   |


* input範例

```
{
    "QueryId": "0911001001",
    "OrderNo": "H10791575",
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| OrderNo  | 訂單編號         | string    | H10791575     |
| InviteeId | 被邀請人ID | string | A140584785 |
| QueryId | 要邀請的ID或手機(原input參數) | string | 0911001001 |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "OrderNo":"H10791575",
        "InviteeId":"A140584785",
        "QueryId":"0911001001"
    }
}
```

* 錯誤代碼

| 錯誤代碼 | 說明                                       |
| -------- | ------------------------------------------ |
| ERR919   | 對方不能租車，請對方確認會員狀態哦！       |
| ERR920   | 同時段有合約或預約，不能邀請哦！           |
| ERR921   | 已至邀請人數上限，請手動移除非邀請對象哦！ |

## JointRentInvitation 案件共同承租人邀請

### [/api/JointRentInvitation/]

* 20210819新增

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例        |
| --------- | ---------- | :--: | :----: | ----------- |
| OrderNo  | 訂單編號       |  Y   | string | H10791575   |
| InviteeId | 要邀請的ID   |  Y   | string    | A140584785 |
| QueryId | 要邀請的ID或手機(原input參數) |  Y   | string | 0911001001 |


* input範例

```
{
    "OrderNo": "H10791575",
    "InviteeId": "A140584785",
    "QueryId":"0911001001"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```

* 錯誤代碼

| 錯誤代碼 | 說明                               |
| -------- | ---------------------------------- |
| ERR928   | 已存在於共同承租邀請清單，新增失敗 |
| ERR929   | 無法進行共同承租邀請               |

## JointRentInviteeModify 案件共同承租人邀請狀態維護

### [/api/JointRentInviteeModify/]

* 20210819新增

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例        |
| --------- | ---------- | :--: | :----: | ----------- |
| OrderNo  | 訂單編號       |  Y   | string | H10791575   |
| InviteeId   | 被邀請的ID   |  Y   | string    | A140584785 |
| ActionType  |  行為  |  Y   | string    | F:取消  D:刪除 S:重邀|


* input範例

```
{
    "OrderNo": "H10791575",
    "InviteeId": "A140584785",
    "ActionType":"F"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```

* 錯誤代碼

| 錯誤代碼 | 說明 |
| ------- | ------- |
| ERR919 | 對方不能租車，請對方確認會員狀態哦！ |
| ERR920 | 同時段有合約或預約，不能邀請哦！ |
| ERR921 | 已至邀請人數上限，請手動移除非邀請對象哦！ |
| ERR924 | 無法取消共同承租邀請 |
| ERR925 | 無法進行共同承租重新邀請 |
| ERR926 | 無法從共同承租清單移除 |


## JointRentIviteeFeedBack 案件共同承租人回應邀請
### [/api/JointRentIviteeFeedBack/]
* 20210819新增

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例        |
| --------- | ---------- | :--: | :----: | ----------- |
| OrderNo  | 訂單編號       |  Y   | string | H10791575   |
| InviteeId   | 被邀請的ID   |  Y   | string    | A140584785 |
| FeedbackType  |  邀請回覆  |  Y   | string    | Y:同意  N:拒絕 |


* input範例

```
{
    "OrderNo": "H10791575",
    "InviteeId": "A140584785",
    "FeedbackType":"Y"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {}
}
```

* 錯誤代碼

| 錯誤代碼 | 說明                                       |
| -------- | ------------------------------------------ |
| ERR919   | 對方不能租車，請對方確認會員狀態哦！       |
| ERR920   | 同時段有合約或預約，不能邀請哦！           |
| ERR921   | 已至邀請人數上限，請手動移除非邀請對象哦！ |
| ERR927   | 非邀請中的合約無法進行操作                 |
| ERR929   | 共同承租回應邀請更新失敗                   |


# 推播相關

## News 活動通知

### [/api/News/]

* 20210908新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

  不須傳入參數

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data 回傳參數說明

| 參數名稱   | 參數說明              |  型態  | 範例       |
| ---------- | --------------------- | :----: | ---------- |
| NewsObj	 | 活動通知				 |    	  |            |

* 活動通知參數說明

| 參數名稱   | 參數說明              |  型態  | 範例       |
| ---------- | --------------------- | :----: | ---------- |
| NewsID	 | 活動通知流水號		 | int	  | 1000	   |
| IsNews	 | 是否為新訊息(1:是0:否)| int 	  | 1		   |
| IsEDM		 | 是否為廣告EDM(1:是0:否)| int   | 1 		   |
| ActionType | 1:和運 2:系統 3:活動 4:優惠 | int | 4	   |
| Title 	 | 主旨					 | string | 9/7凌晨進行定期維護 |
| Message 	 | 內容					 | string | 		   |
| URL		 | 外連URL				 | string | 		   |
| isTop		 | 是否置頂				 | int 	  | 0		   |
| readFlg	 | 1:已讀 2:未讀		 | int 	  | 1		   |




* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "NewsObj": []
    }
}

```

## NewsRead 活動通知讀取

### [/api/NewsRead/]

* 20210908新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例        |
| --------- | ---------- | :--: | :----: | ----------- |
| NewsID    | 活動通知流水號 | Y | int | 10000 |

* input範例

```
{
    "NewsID": "10000"
}
```


* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
    }
}

```

## PersonNotice 個人訊息

### [/api/PersonNotice/]

* 20210908新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

  不須傳入參數

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data 回傳參數說明

| 參數名稱   | 參數說明              |  型態  | 範例       |
| ---------- | --------------------- | :----: | ---------- |
| PersonNoticeObj	 | 個人訊息				 |    	  |            |

* 個人訊息參數說明

| 參數名稱   | 參數說明              |  型態  | 範例       |
| ---------- | --------------------- | :----: | ---------- |
| NotificationID | 個人訊息流水號		 | int	  | 1000	   |
| PushTime	 | 推播時間				 | int 	  | 1		   |
| Title 	 | 主旨					 | string | 9/7凌晨進行定期維護 |
| Message 	 | 內容					 | string | 		   |
| URL		 | 連結URL				 | string | 		   |
| isTop		 | 是否置頂				 | int 	  | 0		   |
| readFlg	 | 1:已讀 2:未讀		 | int 	  | 1		   |




* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "NewsObj": []
    }
}

```


# 電子錢包相關

## CreditAndWalletQuery

### [/api/CreditAndWalletQuery/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態    | 範例             |
| --------- | ---------- | :--: | :----:   | ---------------- |

* input範例

```
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Data 回傳參數說明

| 參數名稱     | 參數說明            |  型態  | 範例          |
| ------------ | ------------------  | :----: | ------------- |
| HasBind      | 是否有綁定(0:無,1有)| int    | 1             |
| HasWallet    | 是否有錢包(0:無,1有)| int    | 1             |
| TotalAmount  | 錢包剩餘金額        | int    | 20000         |
| BindListObj  | 綁卡列表            | List   |               |

* BindListObj 回傳參數說明

| 參數名稱         | 參數說明                            | 型態   | 範例          |
| ---------------- | ----------------------------------  | :----: | ------------- |
| BankNo           | 銀行帳號                            | string | 待確認        |
| CardNumber       | 信用卡卡號                          | string | 待確認        |
| CardName         | 信用卡自訂名稱                      | string | 待確認        |
| AvailableAmount  | 剩餘額度                            | string | 待確認        |
| CardToken        | 替代性信用卡卡號或替代表銀行卡號    | string | 待確認        |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
		"HasBind": 1,
		"BindListObj": [
		  {
		    "BankNo": "待確認",
			"CardNumber": "待確認",
			"CardName": "待確認待確認",
			"AvailableAmount": "50000",
			"CardToken": "待確認"
		  }
		],
		"SEQNO": 9,
		"HasWallet": 1,
		"TotalAmount": 20000	
    }
}
```

----

## WalletStoreTradeTransHistory

### [/api/WalletStoreTradeTransHistory/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態    | 範例             |
| --------- | ---------- | :--: | :----:   | ---------------- |
| SD        | 查詢起日   |  Y   | DateTime | 2021-08-01 00:00 |
| ED        | 查詢迄日   |  Y   | DateTime | 2021-08-30 00:00 |

* input範例

```
{
    "SD":"2021-08-01 00:00",
    "ED":"2021-08-30 00:00"
}
```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |
| Data-TradeHis| 錢包歷史紀錄       |  List  |               |

* Data-TradeHis 回傳參數說明

| 參數名稱    | 參數說明           |  型態  | 範例          |
| ----------- | ------------------ | :----: | ------------- |
| ORGID       | 組織代號(公司代碼) | string | 01            |
| IDNO        | 身分證號           | string | A123456789    |
| SEQNO       | 帳款流水號         |  int   | 1             |
| F_INFNO     | 上游批號           | string | TaishinNo09   |
| TradeYear   | 交易年分           |  int   | 2021          |
| TradeDate   | 交易日期           | string | 08/17         |
| TradeTime   | 交易時間           | string | 20:05         |
| TradeTypeNm | 交易類別           | string | 錢包提領      |
| TradeNote   | 交易類別註記       | string | 電子錢包提領  |
| TradeAMT    | 交易金額           |  int   | 1000          |
| ShowFLG     | APP上是否顯示      |  int   | 0:隱藏,1:顯示 |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "TradeHis": [
            {
                "ORGID": "01",
                "IDNO": "A123456789",
                "SEQNO": 9,
                "F_INFNO": "TaishinNo09",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "20:05",
                "TradeTypeNm": "錢包提領",
                "TradeNote": "電子錢包提領",
                "TradeAMT": -6000,
                "ShowFLG": 1
            },
            {
                "ORGID": "01",
                "IDNO": "A123456789",
                "SEQNO": 8,
                "F_INFNO": "TaishinNo10",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "錢包付款",
                "TradeNote": "H11216402",
                "TradeAMT": -800,
                "ShowFLG": 1
            },
            {
                "ORGID": "01",
                "IDNO": "A123456789",
                "SEQNO": 7,
                "F_INFNO": "TaishinNo08",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "錢包付款",
                "TradeNote": "訂閱制",
                "TradeAMT": -2999,
                "ShowFLG": 1
            },
            {
                "ORGID": "01",
                "IDNO": "A123456789",
                "SEQNO": 6,
                "F_INFNO": "TaishinNo06",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "轉贈",
                "TradeNote": "轉贈人 陳O安",
                "TradeAMT": 2000,
                "ShowFLG": 1
            },
            {
                "ORGID": "01",
                "IDNO": "A123456789",
                "SEQNO": 5,
                "F_INFNO": "TaishinNo05",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "合約退款",
                "TradeNote": "合約退款",
                "TradeAMT": 500,
                "ShowFLG": 1
            },
            {
                "ORGID": "01",
                "IDNO": "A123456789",
                "SEQNO": 4,
                "F_INFNO": "TaishinNo04",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "錢包加值(虛擬帳號轉帳)",
                "TradeNote": "虛擬帳號轉帳",
                "TradeAMT": 3000,
                "ShowFLG": 1
            },
            {
                "ORGID": "01",
                "IDNO": "A123456789",
                "SEQNO": 3,
                "F_INFNO": "TaishinNo03",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "錢包加值(超商繳款)",
                "TradeNote": "超商繳款",
                "TradeAMT": 8000,
                "ShowFLG": 1
            },
            {
                "ORGID": "01",
                "IDNO": "A123456789",
                "SEQNO": 2,
                "F_INFNO": "TaishinNO01",
                "TradeYear": 2021,
                "TradeDate": "08/13",
                "TradeTime": "00:00",
                "TradeTypeNm": "錢包付款",
                "TradeNote": "H11845561",
                "TradeAMT": -1000,
                "ShowFLG": 1
            },
            {
                "ORGID": "01",
                "IDNO": "A123456789",
                "SEQNO": 1,
                "F_INFNO": "TaishinNo02",
                "TradeYear": 2021,
                "TradeDate": "08/13",
                "TradeTime": "00:00",
                "TradeTypeNm": "錢包加值(網路刷卡)",
                "TradeNote": "信用卡*6906",
                "TradeAMT": 30000,
                "ShowFLG": 1
            }
        ]
    }
}
```

----

## WalletStoreTradeHistoryHidden

### [/api/WalletStoreTradeHistoryHidden/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明            | 必要 |  型態    | 範例          |
| --------- | ------------------- |      | :--:     | ------------- | 
| ORGID     | 組織代號(公司代碼)  |      | string   | 0             |
| SEQNO     | 帳款流水號(by會員)  |      | int      | 1             |
| F_INFNO   | 財務-上游批號(IR編) |      | string   | TaishinNO01   |


* input範例

```
{
  "ORGID" : "01",
  "SEQNO" : 1,
  "F_INFNO" : "TaishinNO01"
}

```

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           |        |               |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {}
}
```

----

## GetWalletStoredMoneySet

### [/api/GetWalletStoredMoneySet/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明                              | 必要 |  型態    | 範例          |
| --------- | ------------------------------------  |      | :--:     | ------------- | 
| StoreType | 儲值方式(1信用卡,2虛擬帳號,3超商繳費) |      | int      | 1             |

* input範例

```
{
  "StoreType" : 01
}

```

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |
| Data-StoredMoneySet | 儲值設定資訊       |  List  |               |

* Data-StoredMoneySet 回傳參數說明

| 參數名稱        | 參數說明                              |   型態    | 範例          |
| --------------- | ------------------------------------- |  :----:   | ------------- |
| StoreType       | 儲值方式(1信用卡,2虛擬帳號,3超商繳費) | int       | 100           |
| StoreTypeDetail | 明細方式(全家:family,7-11:seven)      | string    | family           |
| WalletBalance   | 錢包餘額        　                    | int       | 100           |
| Rechargeable    | 尚可儲值         　                   | int       | 49900         |
| StoreLimit      | 單次儲值最低                          | int       | 50000         |
| StoreMax        | 單次儲值最高                          | int       | 100           |
| QuickBtns       | 快速按鈕                              | List<int> |               |
| defSet          | 預設選取(1是0否)                      | int       |               |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	   {
	     "StoreType":3,
		 "StoreTypeDetail": "family",
	     "WalletBalance": 100,
		 "Rechargeable": 49900,
		 "StoreMax": 50000,
		 "QuickBtns": [100,1000,5000],
		 "defSet": 1
	   },
	   {
	     "StoreType":3,
		 "StoreTypeDetail": "seven",
	     "WalletBalance": 1000,
		 "Rechargeable": 19000,
		 "StoreMax": 20000,
		 "QuickBtns": [100,500,1000],
		 "defSet": 0
	   },	   
	}
}
```

----

## WalletStoredMoney

### [/api/WalletStoredMoney/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                              | 必要 |  型態    | 範例          |
| ---------- | ------------------------------------  |      | :--:     | ------ | 
| StoreMoney | 儲值金額                              |      | int      | 100    |

* input範例

```
{
  "StoreMoney" : 100
}

```

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data-StoredMoneySet 回傳參數說明

| 參數名稱        | 參數說明                              |   型態    | 範例             |
| --------------- | ------------------------------------- |  :----:   | ---------------- |
| StroeResult     | 儲值結果(1成功,0失敗)                 | int       | 1                |
| StoreMoney      | 儲值金額        　                    | int       | 100              |
| WalletBalance   | 錢包餘額         　                   | int       | 100              |
| Rechargeable    | 尚可儲值                              | int       | 49900            |
| StoreTime       | 儲值時間                              | string    | 2020/03/31 11:24 |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	   {
	     "StroeResult": 1,
	     "StoreMoney": 500,
		 "WalletBalance": 2500,
		 "StoreMax": 47500,
		 "QuickBtns": [100,1000,5000]
	   }
	}
}

```

----

## WalletStoreVisualAccount

### [/api/WalletStoreVisualAccount/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                              | 必要 |  型態    | 範例          |
| ---------- | ------------------------------------  |      | :--:     | ------ | 
| StoreMoney | 儲值金額                              |      | int      | 100    |

* input範例

```
{
  "StoreMoney" : 100
}

```

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data-StoredMoneySet 回傳參數說明

| 參數名稱        | 參數說明                              |   型態    | 範例             |
| --------------- | ------------------------------------- |  :----:   | ---------------- |
| StroeResult     | 儲值結果(1成功,0失敗)                 | int       | 1                |
| StoreMoney      | 儲值金額        　                    | int       | 100              |
| WalletBalance   | 錢包餘額         　                   | int       | 100              |
| Rechargeable    | 尚可儲值                              | int       | 49900            |
| PayDeadline     | 繳費期限                              | string    | 2021/03/31 23:19 |
| VirtualAccount  | 轉入虛擬帳號    　                    | string    | (812)1234567812345678 |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	   {
	     "StroeResult": 1,
	     "StoreMoney": 500,
		 "WalletBalance": 2500,
		 "Rechargeable" : 47500,
         "PayDeadline"		 
	   }
	}
}

```

----

## WalletStoreShop

### [/api/WalletStoreShop/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                              | 必要 |  型態    | 範例          |
| ---------- | ------------------------------------  |      | :--:     | ------ | 
| StoreMoney | 儲值金額                              |      | int      | 100    |

* input範例

```
{
  "StoreMoney" : 100
}

```

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data-StoredMoneySet 回傳參數說明

| 參數名稱        | 參數說明                              |   型態    | 範例                  |
| --------------- | ------------------------------------- |  :----:   | --------------------- |
| StroeResult     | 儲值結果(1成功,0失敗)                 | int       | 1                     |
| StoreMoney      | 儲值金額        　                    | int       | 100                   |
| WalletBalance   | 錢包餘額         　                   | int       | 100                   |
| Rechargeable    | 尚可儲值                              | int       | 49900                 |
| PayWithinHours  | 幾小時內繳費                          | int       | 3              |
| PayDeadline     | 繳費期限                              | string    | 02:59:59              |
| ShopBarCode1    | 超商條碼1    　                       | string    | 1003908SJ             |
| ShopBarCode2    | 超商條碼2    　                       | string    | 20944SE031003908SUEPJ |
| ShopBarCode3    | 超商條碼3    　                       | string    | 10023984HPDJ3908SJ    |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	   {
	     "StroeResult": 1,
	     "StoreMoney": 100,
		 "WalletBalance": 100,
		 "Rechargeable" : 49900,
		 "PayWithinHours" : 3,
         "PayDeadline" : "02:59:59",
		 "ShopBarCode1" : "1003908SJ",
		 "ShopBarCode2" : "20944SE031003908SUEPJ",
		 "ShopBarCode3" : "10023984HPDJ3908SJ"
	   }
	}
}

```

----

## GetPayInfoReturnCar

### [/api/GetPayInfoReturnCar/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱             | 參數說明        | 必要 |  型態    | 範例   |
| -------------------- | --------------- |      | :--:     | ------ | 

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |
| Data-PayModes 　　　| 付款方式           |  List  |               |

* Data-PayModes 回傳參數說明

| 參數名稱        | 參數說明                              |   型態    | 範例                  |
| --------------- | ------------------------------------- |  :----:   | --------------------- |
| CheckoutMode    | 付款方式(1信用卡,2錢包)               | int       | 1                     |
| CheckoutNM      | 付款方式名稱        　                | string    | 卡號,錢包全額支付...  |
| CheckoutNote    | 付款方式備註        　                | int       | 餘額 $50元            |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	   {
	     "CheckoutMode": 1,
	     "CheckoutNM": "錢包全額支付",
		 "CheckoutNote": "餘額 $6,350"
	   }
	}
}

```

----

## DoPayReturnCar

### [/api/DoPayReturnCar/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱             | 參數說明                 | 必要 |  型態    | 範例   |
| -------------------- | ------------------------ |      | :--:     | ------ | 
|  TradeAMT            |  付款金額                |  Y   |   int    |        |
|  CheckoutMode        |  付款方式(1信用卡,2錢包) |  Y   |   int    |        |

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data 回傳參數說明

| 參數名稱        | 參數說明                              |   型態    | 範例                  |
| --------------- | ------------------------------------- |  :----:   | --------------------- |
| CheckoutModes   | 付款結果 (1成功0失敗)                 | int       | 1                     |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	    "CheckoutModes":[
	    {
	      "CheckoutMode": 1,
	      "CheckoutNM": "錢包全額支付",
		  "CheckoutNote": "餘額 $50"
	    },	
	    {
	      "CheckoutMode": 2,
	      "CheckoutNM": " *1234",
		  "CheckoutNote": "餘額 $50"
	    }		
	  ]
	}
}

```

----

## ChoseCheckoutModeShow

### [/api/ChoseCheckoutModeShow/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱             | 參數說明                 | 必要 |  型態    | 範例   |
| -------------------- | ------------------------ |      | :--:     | ------ | 
* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |
| Data-CheckoutModes  | 付款方式列表       | List   |               |


* Data - CheckoutModes 回傳參數說明

| 參數名稱        | 參數說明                              |   型態    | 範例                  |
| --------------- | ------------------------------------- |  :----:   | --------------------- |
| CheckoutMode    | 付款方式(1信用卡,2錢包)               |   int     | 1                     |
| CheckoutNM      | 付款名稱(1和雲錢包,信用卡)            |   string  | 和雲錢包              |
| CheckoutNote    | 付款說明                              |   string  | 餘額不足              |
| IsDef           | 是否預設選取                          |   int     | 1                     |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	    "PayModes":[
	    {
	      "CheckoutMode": 1,
	      "CheckoutNM": "信用卡",
		  "CheckoutNote": "",
		  "IsDef":0
	    },	
	    {
	      "CheckoutMode": 2,
	      "CheckoutNM": "和雲錢包",
		  "CheckoutNote": "餘額不足",
		  "IsDef":1
	    }		
	  ]
	}
}

```

----

## ChoseCheckoutModeSet

### [/api/ChoseCheckoutModeSet/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱             | 參數說明                 | 必要 |  型態    | 範例   |
| -------------------- | ------------------------ |      | :--:     | ------ | 
| CheckoutMode         | 付款方式(1信用卡,2錢包)  |  Y   | int      | ------ | 

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |


* Data - CheckoutModes 回傳參數說明

| 參數名稱        | 參數說明                              |   型態    | 範例                  |
| --------------- | ------------------------------------- |  :----:   | --------------------- |
| SetResult       | 設定果1成功,0失敗)                    |   int     | 1                     |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	    "SetResult":1
	}
}

```

----

## GetPayInfoArrears

### [/api/GetPayInfoReturnCar/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱             | 參數說明        | 必要 |  型態    | 範例   |
| -------------------- | --------------- |      | :--:     | ------ | 

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data-CheckoutMode回傳參數說明

| 參數名稱     | 參數說明                |  型態  | 範例                         |
| ------------ | ----------------------- | :----: | ---------------------------- |
| CheckoutMode | 付款方式(1信用卡,2錢包) |  int   | 1                            |
| CheckoutNM   | 付款方式名稱            | string | 錢包餘額不足,錢包全額支付... |
| CheckoutNote | 付款方式備註            |  int   | 餘額 $6,350元                |
| IsDef        | 預設選取(1是0否)        |  int   | 1                            |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	   "CheckoutMode":[	   
           {
             "CheckoutMode": 1,
             "CheckoutNM": "*1234",
             "CheckoutNote": "餘額 $6,350",
             "IsDef":0
           },	   
           {
             "CheckoutMode": 2,
             "CheckoutNM": "錢包餘額不足",
             "CheckoutNote": "餘額 $6,350",
             "IsDef":1
           }	   
	   ]
	}
}

```

----

## DoPayArrears

### [/api/DoPayArrears/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱             | 參數說明                 | 必要 |  型態    | 範例   |
| -------------------- | ------------------------ |      | :--:     | ------ | 
|  TradeAMT            |  付款金額                |  Y   |   int    |        |
|  CheckoutMode        |  付款方式(1信用卡,2錢包) |  Y   |   int    |        |

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data 回傳參數說明

| 參數名稱        | 參數說明                              |   型態    | 範例                  |
| --------------- | ------------------------------------- |  :----:   | --------------------- |
| PayResult       | 付款結果 (1成功0失敗)                 |   int     | 1                     |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	    "PayResult":1
	}
}

```

----

## WalletTransferCheck

### [/api/WalletTransferCheck/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明         | 必要 |  型態  | 範例                   |
| ---------- | ---------------- | ---- | :----: | ---------------------- |
| IDNO_Phone | 身分證或手機號碼 | Y    | string | A123456789, 0958123456 |
| Amount     | 轉贈金額         | Y    |  int   | 1000                   |

* input範例

```
{
  "IDNO_Phone" : "A123456789", 
  "Amount" : 1000
}

```

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data 回傳參數說明

| 參數名稱   | 參數說明              |  型態  | 範例  |
| ---------- | --------------------- | :----: | ----- |
| CkResult   | 驗證結果 (1成功0失敗) |  int   | 1     |
| Name_Phone | 名稱或電話號碼        | string | 姓O名 |
| Amount     | 轉贈金額              |  int   | 1000  |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	    "CkResult":1,
	    "Name_Phone": "姓O名",
	    "Amount":1000
	}
}

```

----

## WalletInfoCheck

### [/api/WalletInfoCheck/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明         | 必要 |  型態  | 範例                   |
| ---------- | ---------------- | ---- | :----: | ---------------------- |
| IDNO_Phone | 身分證或手機號碼 | Y    | string | A123456789, 0958123456 |

* input範例

```
{
  "IDNO_Phone" : "A123456789", 
}

```

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data 回傳參數說明

| 參數名稱     | 參數說明              |  型態   | 範例  |
| ------------ | --------------------- | :----:  | ----- |
| CkResult     | 驗證結果 (1成功0失敗) |  int    | 1     |
| WalletAmount | 錢包剩餘金額          |  int    | 1000  |
| MonTransIn   | 當月入賬總金額        |  int    | 2000  |
| Name_Phone   | 名稱或電話號碼        |  string | 姓O名  |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	    "CkResult":1,
	    "WalletAmount": 1000,
	    "MonTransIn": 2000,
		"Name_Phone": "姓O名"
	}
}

```

----

## WalletTransferStoredValue

### [/api/WalletTransferStoredValue/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明         | 必要 |  型態  | 範例       |
| ---------- | ---------------- | ---- | :----: | ---------- |
| IDNO_Phone | 身分證或手機號碼 | Y    | string | A123456789 |
| Amount     | 轉贈金額         | Y    |  int   | 1000       |

* input範例

```
{
  "IDNO_Phone" : "A123456789", 
  "Amount" : 1000
}

```

* Output回傳參數說明

| 參數名稱      　　　| 參數說明           |  型態  | 範例          |
| ------------------- | ------------------ | :----: | ------------- |
| Result        　　　| 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode     　　　| 錯誤碼             | string | 000000        |
| NeedRelogin   　　　| 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade   　　　| 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage  　　　| 錯誤訊息           | string | Success       |
| Data          　　　| 資料物件           |        |               |

* Data 回傳參數說明

| 參數名稱   | 參數說明              |  型態  | 範例       |
| ---------- | --------------------- | :----: | ---------- |
| TranResult | 轉贈結果 (1成功0失敗) |  int   | 1          |
| Name_Phone | 名稱或電話號碼        | string | A123456789 |
| Amount     | 轉贈金額              |  int   | 1000       |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	    "TranResult":1,
	    "Name_Phone":"A123456789",
	    "Amount":1000
	}
}

```



