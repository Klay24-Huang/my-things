版本: 1.0

# iRentApi2 WebAPI

## 目錄

註冊相關

- [Register_Step2 設定密碼](#Register_Step2)
- [RegisterMemberData 設定會員資料](#RegisterMemberData)

登入相關

- [Login 登入](#Login)
- [RefrashToken 更新Token](#RefrashToken)
- [CheckAppVersion 檢查APP版本](#CheckAppVersion)

會員相關

- [GetMemberStatus 取得會員狀態](#GetMemberStatus)
- [GetMemberScore 取得會員積分](#GetMemberScore)
- [SetMemberScoreDetail 修改會員積分明細](#SetMemberScoreDetail)
- [GetMemberMedal 取得會員徽章](#GetMemberMedal)
- [SetMemberCMK 更新會員條款](#SetMemberCMK)
- [TransWebMemCMK 拋轉官網會員同意資料](#TransWebMemCMK)
- [GetMemberRedPoint 取得會員紅點通知](#GetMemberRedPoint)
- [GiftTransferCheck 會員轉贈對象查詢](#GiftTransferCheck)
- [MemberUnbind 會員解綁](#MemberUnbind)

首頁地圖相關

- [GetBanner 取得廣告資訊](#GetBanner)
- [GetMapMedal 取得地圖徽章](#GetMapMedal)
- [GetFavoriteStation 取得常用站點](#GetFavoriteStation)
- [SetFavoriteStation 設定常用站點](#SetFavoriteStation)
- [NormalRent 取得同站租還站點](#NormalRent)
- [GetCarType 同站以據點取出車型](#GetCarType)
- [GetProject 取得專案與資費(同站)](#GetProject)
- [GetCarTypeGroupList 取得車型清單](#GetCarTypeGroupList)
- [GetPolygon 取得電子柵欄](#GetPolygon)
- [AnyRent 取得路邊租還車輛](#AnyRent)
- [GetAnyRentProject 取得專案與資費(路邊)](#GetAnyRentProject)
- [MotorRent 取得路邊租還機車](#MotorRent)
- [GetMotorRentProject 取得專案與資費(機車)](#GetMotorRentProject)
- [GetEstimate 資費明細(預估租金)](#GetEstimate)

預約以及訂單相關

- [Booking 預約](#Booking)
- [BookingQuery 訂單列表](#BookingQuery)
- [BookingFinishQuery 完成的訂單查詢](#BookingFinishQuery)
- [BookingDelete 刪除訂單](#BookingDelete)
- [OrderDetail 訂單明細](#OrderDetail)
- [GetOrderInsuranceInfo 訂單安心服務資格及價格查詢](#GetOrderInsuranceInfo)
- [GetCancelOrderList 取得取消訂單列表](#GetCancelOrderList)

取還車跟車機操控相關

- [ChangeUUCard 變更悠遊卡](#ChangeUUCard)
- [BookingStart 汽車取車](#BookingStart)
- [BookingStartMotor 機車取車](#BookingStartMotor)
- [BookingExtend 延長用車](#BookingExtend)
- [ReturnCar 還車](#ReturnCar)
- [GetPayDetail 取得租金明細](#GetPayDetail)
- [CreditAuth 付款與還款](#CreditAuth)

月租訂閱制相關

- [GetMonthList 取得訂閱制月租列表](#GetMonthList)
- [GetMonthGroup 訂閱制月租專案群組](#GetMonthGroup)
- [BuyNow/AddMonth 月租購買](#BuyNowAddMonth)
- [BuyNow/UpMonth 月租升轉](#BuyNowUpMonth)
- [BuyNow/PayArrs 欠費繳交](#BuyNowPayArrs)
- [BuyNowTool/AddMonth 月租購買工具](#BuyNowToolAddMonth)
- [BuyNowTool/UpMonth 月租升轉工具](#BuyNowToolUpMonth)
- [GetMySubs 我的方案牌卡明細](#GetMySubs)
- [GetSubsCNT 取得合約明細](#GetSubsCNT)
- [GetChgSubsList 變更下期續約列表](#GetChgSubsList)
- [GetUpSubsList 訂閱制升轉列表](#GetUpSubsList)
- [GetSubsHist 訂閱制歷史紀錄](#GetSubsHist)
- [GetSubsHist-del 訂閱制歷史紀錄-刪除](#GetSubsHist-del)
- [GetArrsSubsList 訂閱制欠費查詢](#GetArrsSubsList)
- [SetSubsNxt 設定自動續約](#SetSubsNxt)

車輛調度停車場

- [GetMotorParkingData 取得機車調度停車場](#GetMotorParkingData)
- [GetParkingData 取得汽車調度停車場](#GetParkingData)

推播相關

- [News 活動通知](#News)
- [NewsRead 活動通知讀取](#NewsRead)
- [PersonNotice 個人訊息](#PersonNotice)
- [PersonNoticeRead 個人訊息讀取](#PersonNoticeRead)
- [TestPushService 測試推播工具](#TestPushService)
- [NoticeRead 推播通知讀取](#NoticeRead)

共同承租人機制

- [JointRentInviteeListQuery 共同承租人邀請清單查詢](#JointRentInviteeListQuery)
- [JointRentInviteeVerify 共同承租人邀請檢核](#JointRentInviteeVerify)
- [JointRentInvitation 案件共同承租人邀請](#JointRentInvitation)
- [JointRentInviteeModify 案件共同承租人邀請狀態維護](#JointRentInviteeModify)
- [JointRentIviteeFeedBack 案件共同承租人回應邀請](#JointRentIviteeFeedBack)

電子錢包相關

- [CreditAndWalletQuery 查詢綁卡跟錢包](#CreditAndWalletQuery)
- [WalletStoreTradeTransHistory 錢包歷史紀錄查詢](#WalletStoreTradeTransHistory)
- [WalletStoreTradeHistoryHidden 錢包歷程-儲值交易紀錄隱藏](#WalletStoreTradeHistoryHidden)
- [GetWalletStoredMoneySet 錢包儲值-設定資訊](#GetWalletStoredMoneySet)
- [WalletStoredByCredit 錢包儲值-信用卡](#WalletStoredByCredit)
- [WalletStoreVisualAccount 錢包儲值-虛擬帳號](#WalletStoreVisualAccount)
- [WalletStoreShop 錢包儲值-商店條碼](#WalletStoreShop)
- [GetPayInfo 取得付款方式](#GetPayInfo)
- [WalletTransferStoredValue 錢包轉贈](#WalletTransferStoredValue)
- [WalletTransferCheck 轉贈對象確認](#WalletTransferTargetCheck)
- [SetDefPayMode 設定預設支付方式](#SetDefPayMode)
- [AutoStoreSetting 自動儲值設定](#AutoStoreSetting)
- [WalletWithdrowInvoice 寫入手續費發票](#WalletWithdrowInvoice)


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

20210819 共同承租人邀請檢核API,案件共同承租人邀請API,案件共同承租人邀請狀態維護API,案件共同承租人回應邀請API

20210819 歷史訂單明細(OrderDetail)增加欄位承租人類型(RenterType)

20210820 拋轉官網會員同意資料(TransWebMemCMK)欄位格式調整,補訂單清單查詢API,訂單清單查詢(BookingQuery)增加欄位承租人類型(RenterType)

20210820 補汽車取車API,汽車取車(BookingStart)增加欄位略過未回應的被邀請人(SkipNoFeedbackInvitees)

20210820 補機車取車API,機車取車(BookingStartMotor)增加欄位略過未回應的被邀請人(SkipNoFeedbackInvitees)

20210820 補完成的訂單查詢API,完成的訂單查詢(BookingFinishQuery)增加欄位是否為共同承租訂單(IsJointOrder)

20210820 補刪除訂單API

20210825 汽車取車(BookingStart)移除欄位略過未回應的被邀請人(SkipNoFeedbackInvitees)

20210825 機車取車(BookingStartMotor)移除欄位略過未回應的被邀請人(SkipNoFeedbackInvitees)

20210825 共同承租人邀請清單查詢(JointRentInviteeListQuery)新增欄位邀請時輸入的ID或手機(QueryId)

20210830 訂單列表(BookingQuery)欄位修正

20210831 共同承租人邀請(JointRentInvitation) 回應邀請(JointRentIviteeFeedBack ) 參數名稱&錯誤代碼更新

20210901 機車取車(BookingStartMotor) input修正

20210901 共同承租人回應邀請(JointRentIviteeFeedBack) 欄位值修正

20210901 調整電子錢包相關API輸入輸出欄位

20210902 電子錢包錢包歷史紀錄查詢(WalletStoreTradeTransHistory)、 錢包歷程-儲值交易紀錄隱藏 (WalletStoreTradeHistoryHidden) 欄位調整，相關API欄位參數型態值調整

20210906 取得租金明細(GetPayDetail)欄位修正

20210907 增加推播相關

20210909 共同承租人回應邀請(JointRentIviteeFeedBack) Input欄位調整&錯誤代碼修正

20210909 電子錢包取得付款方式(GetPayInfo)變更API輸出欄位

20210909 補上付款與還款API(CreditAuth)，並變更輸入欄位

20210910 取得會員狀態(GetMemberStatus)增加是否顯示購買牌卡

20210910 補上錢包儲值-設定資訊(GetWalletStoredMoneySet)錯誤代碼

20210910 電子錢包取得付款方式(GetPayInfo)增加輸出使用者目前已綁定的付費方式數量(PayModeBindCount)

20210913 共同承租人邀請檢核(JointRentInviteeVerify) 新增錯誤代碼

20210914 錢包儲值-信用卡(WalletStoredByCredit)補上錯誤代碼

20210916 移除錢包儲值-設定資訊(GetWalletStoredMoneySet)錯誤代碼

20210921 取得會員狀態(GetMemberStatus)增加是否有推播訊息

20210922 推播相關增加推播通知讀取(NoticeRead)

20210922 查詢綁卡跟錢包(CreditAndWalletQuery)增加輸出欄位

20210928 共同承租人邀請檢核(JointRentInviteeVerify) 新增錯誤代碼

20210928 新增設定預設支付方式(SetDefPayMode)

20210928 錢包儲值-虛擬帳號(WalletStoreVisualAccount) 參數調整、錢包儲值-商店條碼(WalletStoreShop) 新增輸入欄位

20210929 自動儲值設定(AutoStoreSetting)參數調整

20211007 錢包儲值-商店條碼(WalletStoreShop) 範例調整

20211008 預約(Booking) 輸出參數調整

20211008 共同承租人邀請檢核(JointRentInviteeVerify)  狀態維護 (JointRentInviteeModify)  回應邀請(JointRentIviteeFeedBack) 移除錯誤代碼ERR920

20211012 Booking,BookingStart,BookingStartMotor,ReturnCar增加手機經緯度

20211015 錢包儲值-虛擬帳號(WalletStoreVisualAccount) 新增參數 銀行代碼BankCode

20211021 次序調整、內容修正、API位置統一放置頂端

20211021 新增取得會員紅點通知(GetMemberRedPoint)

20211028 補延長用車(BookingExtend)、補預約(Booking)、汽車取車(BookingStart)錯誤代碼

20211102 更新 汽車取車(BookingStart)、延長用車(BookingExtend)錯誤代碼

20211103 新增取得取消訂單列表(GetCancelOrderList)

20211104 資費明細(預估租金)補上，WalletTransferStoredValue,WalletStoredByCredit,WalletStoreVisualAccount,WalletStoreShop 移除掉Data內的Result

20211116 錢包儲值-信用卡(WalletStoredByCredit)新增API Input參數，錢包儲值-商店條碼 虛擬帳號  調整輸出參數

20211117 取得會員狀態(GetMemberStatus)新增和泰OneID綁定狀態

20211118 查詢綁卡跟錢包(CreditAndWalletQuery)新增和泰PAY相關欄位

20211122 NormalRent跟GetProject 的IsRent=A調整

20211201 錢包儲值-設定資訊(GetWalletStoredMoneySet) 調整StoreType參數

20211209 新增會員解綁(MemberUnbind)

20211228 錢包儲值-信用卡(WalletStoredByCredit)移除錯誤代碼

20220121 補月租購買(BuyNowAddMonth)、月租升轉(BuyNowUpMonth)、欠費繳交(BuyNowPayArrs)、設定自動續約(SetSubsNxt)錯誤代碼

20220207 新增月租購買(BuyNowToolAddMonth)、升轉工具(BuyNowToolUpMonth)

20220221 查詢綁卡跟錢包(CreditAndWalletQuery)新增機車預扣款金額

20220222 預約(Booking)新增機車錢包餘額不足錯誤代碼

20220301 機車取車(BookingStartMotor)補上output參數說明&錯誤代碼、預約(Booking)移除ERR294錯誤代碼

20220315 付款與還款(CreditAuth)增加Input參數

# API位置

| 裝置    | 正式環境                            | 測試環境                                 |
| ------- | ----------------------------------- | ---------------------------------------- |
| iOS     | https://irentcar-app.azurefd.net/   | https://irentcar-app-test.azurefd.net/   |
| ANDROID | https://irent-app-jpw.ai-irent.net/ | https://irentcar-app-test.azurefd.net/ |

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


----------

# 註冊相關

## Register_Step2 設定密碼

### [/api/Register_Step2/]

- 20210811發佈

- ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

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
| Data         | 資料物件           | object |               |

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱 | 參數說明     | 型態 | 範例 |
| -------- | ------------ | :--: | ---- |
| Token    | Token列表    | List |      |
| UserData | 會員資料列表 | List |      |

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
| PayMode        | 付費方式 0:信用卡 1:錢包 4:Hotaipay                                 |  int   | 0                                                            |
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

## RefrashToken 更新Token
### [/api/RefrashToken/]

* 20210322發佈

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱 | 參數說明  | 型態 | 範例 |
| -------- | --------- | :--: | ---- |
| Token    | Token列表 | List |      |

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

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱     | 參數說明                | 型態 | 範例 |
| ------------ | ----------------------- | :--: | ---- |
| MandatoryUPD | 強制更新 (1=強更，0=否) | int  | 1    |

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

# 會員相關

## GetMemberStatus 取得會員狀態

### [/api/GetMemberStatus/]

* 20210322發佈

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱   | 參數說明     | 型態 | 範例 |
| ---------- | ------------ | :--: | ---- |
| StatusData | 會員狀態列表 | List |      |

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
| HasNoticeMsg    | 是否有推播訊息 (Y:是 N:否)                                   | string | Y          |
| AuthStatus      | 預授權條款狀態 (Y:重新確認 N:不需重新確認)                   | string | Y          |
| BindHotai       | 和泰OneID綁定狀態 (Y：綁定 N：未綁)                          | string | N          |


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
            "IsShowBuy": "Y",
			"HasNoticeMsg": "Y",
            "AuthStatus": "Y",
            "BindHotai": "N"
        }
    }
}
```

## GetMemberScore 取得會員積分

### [/api/GetMemberScore/]

- 20210519發佈

- ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱   | 參數說明 | 型態 | 範例 |
| ---------- | -------- | :--: | ---- |
| Score      | 會員積分 | int  | 100  |
| TotalPage  | 總頁數   | int  | 1    |
| DetailList | 積分歷程 | List |      |

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱  | 參數說明 | 型態 | 範例 |
| --------- | -------- | :--: | ---- |
| MedalList | 徽章明細 | List |      |

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

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**


* 動作 [POST]
* Input 傳入參數說明

| 參數名稱  | 參數說明                                     | 必要 |  型態  | 範例 |
| --------- | -------------------------------------------- | :--: | :----: | ---- |
| CHKStatus | 是否同意 (Y：同意 / N：不同意)               |  Y   | string | Y    |
| SeqNo     | 流水號<br>1:會員條款狀態<br>2:預授權條款狀態 |  Y   |  int   | 0    |

* Input範例

```
{
    "CHKStatus": "Y",
    "SeqNo": 2
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
| Data         | 資料物件           | object |               |

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
| Data         | 資料物件           | object |               |

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

## GetMemberRedPoint 取得會員紅點通知

### [/api/GetMemberRedPoint/]

- 20211020發佈

- ASP.NET Web API (REST API)

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**


* 動作 [POST]
* input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| 無參數   |          |      |      |      |

* output 回傳參數說明

| 參數名稱     | 參數說明                       |  型態  | 範例    |
| ------------ | ------------------------------ | :----: | ------- |
| Result       | 是否成功 (0:失敗 1:成功)       |  int   | 1       |
| ErrorCode    | 錯誤碼                         | string | 000000  |
| NeedRelogin  | 是否需重新登入 (0:否 1:是)     |  int   | 0       |
| NeedUpgrade  | 是否需要至商店更新 (0:否 1:是) |  int   | 0       |
| ErrorMessage | 錯誤訊息                       | string | Success |
| Data         | 資料物件                       | object |         |
| RedPointList | 紅點清單                       |  List  |         |

* RedPointList參數說明

| 參數名稱 | 參數說明                                                     | 型態 | 範例 |
| -------- | ------------------------------------------------------------ | :--: | ---- |
| RedNo    | 紅點序號<br>1:漢堡點<br/>2:我的成就<br/>3:徽章<br/>4:積分<br/>5:小鈴鐺 | int  | 1    |
| FLAG     | 紅點FLAG (0:隱藏 1:顯示)                                     | int  | 1    |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "RedPointList": [
            {
                "RedNo": 1,
                "FLAG": 0
            },
            {
                "RedNo": 2,
                "FLAG": 0
            },
            {
                "RedNo": 3,
                "FLAG": 0
            },
            {
                "RedNo": 4,
                "FLAG": 0
            }
        ]
    }
}
```


## GiftTransferCheck 會員轉贈對象查詢

### [/api/GiftTransferCheck/]

- 20211020發佈

- ASP.NET Web API (REST API)

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**


* 動作 [POST]
* input 傳入參數說明

| 參數名稱 |    參數說明    | 必要 | 型態   | 範例       |
| -------- | -------------- | :--: | :--:   | --------   |
| IDNO     | 轉贈對象身分證 |  Y   | string | F123456789 |
| Amount   | 金額  	        |  N   | int    |            |

* output 回傳參數說明

| 參數名稱     | 參數說明                       |  型態  | 範例    |
| ------------ | ------------------------------ | :----: | ------- |
| Result       | 是否成功 (0:失敗 1:成功)       |  int   | 1       |
| ErrorCode    | 錯誤碼                         | string | 000000  |
| NeedRelogin  | 是否需重新登入 (0:否 1:是)     |  int   | 0       |
| NeedUpgrade  | 是否需要至商店更新 (0:否 1:是) |  int   | 0       |
| ErrorMessage | 錯誤訊息                       | string | Success |
| Data         | 資料物件                       | object |         |

* Data參數說明

| 參數名稱 | 參數說明                               |  型態   | 範例      |
| -------- | -------------------------------------  | :-----: | ----------|
| Name     | 轉贈對象名稱							| string  | 吳X耆 	  |
| PhoneNo  | 電話號碼					            | string  | 0912345678|
| Amount   | 金額(無用)				                | int  	  | 0         |

* Output 範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "Name": "吳*耆",
        "PhoneNo": "0912345678",
        "Amount": 0
    }
}
```

## MemberUnbind 會員解綁

### [/api/MemberUnbind/]

- 20211209發佈

- ASP.NET Web API (REST API)

- 傳送跟接收採JSON格式

- HEADER帶入AccessToken**(必填)**


* 動作 [POST]
* Input 傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| 無參數   |          |      |      |      |

* Output 回傳參數說明

| 參數名稱     | 參數說明                       |  型態  | 範例    |
| ------------ | ------------------------------ | :----: | ------- |
| Result       | 是否成功 (0:失敗 1:成功)       |  int   | 1       |
| ErrorCode    | 錯誤碼                         | string | 000000  |
| NeedRelogin  | 是否需重新登入 (0:否 1:是)     |  int   | 0       |
| NeedUpgrade  | 是否需要至商店更新 (0:否 1:是) |  int   | 0       |
| ErrorMessage | 錯誤訊息                       | string | Success |
| Data         | 資料物件                       | object |         |

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

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                                                     | 說明 |
| -------- | ------------------------------------------------------------ | ---- |
| ERR986   | 您尚未完成還車，請完成還車後再刪除iRent會員。                |      |
| ERR987   | 您尚有預約中的訂單，請取消訂單後再刪除iRent會員。            |      |
| ERR988   | 您有未繳費用尚未完成繳納，請先至「未繳費用」中進行繳費後再刪除iRent會員。 |      |
| ERR989   | 您的錢包尚有餘額，請完成退款手續後再刪除iRent會員。          |      |
| ERR990   | 您近期有用車合約，為便於您繳付可能產生的費用(如停車費,Etag)，請於還車的三個月後再刪除iRent會員。 |      |

# 首頁地圖相關

## GetBanner 取得廣告資訊

### [/api/GetBanner/]

* 20210316發佈

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱  | 參數說明     | 型態 | 範例 |
| --------- | ------------ | :--: | ---- |
| BannerObj | 廣告資訊列表 | List |      |

* BannerObj 參數說明

| 參數名稱    | 參數說明   |  型態  | 範例                                                    |
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
            }
        ]
    }
}
```

## GetMapMedal 取得地圖徽章

### [/api/GetMapMedal/]

- 20210521發佈

- ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱  | 參數說明 | 型態 | 範例 |
| --------- | -------- | :--: | ---- |
| MedalList | 徽章明細 | List |      |

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

## GetFavoriteStation取得常用站點

### [/api/GetFavoriteStation/]

* 20210315發佈

* ASP.NET Web API (REST API)

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
| Data | 資料物件 | object | |

* Data回傳參數說明

| 參數名稱    | 參數說明     | 型態 | 範例 |
| ----------- | ------------ | :--: | ---- |
| FavoriteObj | 常用站點列表 | List |      |

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
## SetFavoriteStation設定常用站點
### [/api/SetFavoriteStation/]

* 20210315發佈

* ASP.NET Web API (REST API)

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
| Data | 資料物件 | object |  |


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

## NormalRent 取得同站租還站點

### [/api/NormalRent/]

* 20210324修改

* 20210407修改 - input移除掉Seats 

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* 動作 [GET]

* input傳入參數說明

| 參數名稱   | 參數說明     | 必要 |     型態     | 範例      |
| ---------- | ------------ | :--: | :----------: | --------- |
| ShowALL    | 是否顯示全部 |  Y   |     int      | 0:否 1:是 |
| Latitude   | 緯度         |      |    float     |           |
| Longitude  | 經度         |      |    float     |           |
| Radius     | 半徑         |      |    float     |           |
| * CarTypes | 車型清單     |  N   | array string |           |


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

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱      | 參數說明 | 型態 | 範例 |
| ------------- | -------- | :--: | ---- |
| NormalRentObj | 站點列表 | List |      |

* NormalRentObj 參數說明

| 參數名稱            | 參數說明            |  型態  | 範例              |
| ------------------- | ------------------- | :----: | ----------------- |
| StationID           | 據點代碼            | string | X0II              |
| StationName         | 據點名稱            | string | 濱江站            |
| Tel                 | 電話                | string | 02-12345678       |
| ADDR                | 地址                | string | 台北市松江路999號 |
| Latitude            | 緯度                | float  |                   |
| Longitude           | 經度                | float  |                   |
| Content             | 其他說明            | string |                   |
| ContentForAPP       | 據點描述(app顯示用) | string |                   |
| IsRequiredForReturn | 還車位置資訊必填    |  int   | 0:否 1:是         |
| StationPic          | 據點照片            |  List  |                   |
| IsRent              | 是否可租            | string | Y/N/A(魔鬼剋星)   |


* StationPic參數說明

| 參數名稱       | 參數說明 |  型態  | 範例 |
| -------------- | -------- | :----: | ---- |
| StationPic     | 據點照片 | string |      |
| PicDescription | 據點說明 | string |      |


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

## GetCarType同站以據點取出車型

### [/api/GetCarType/]

* 20210315修改 - 增加是否為常用據點欄位

* 20210324修改 - 增加搜尋使用欄位CarTypes,Seats 

* 20210407修改 - input移除掉Seats 

* 20210408修改 - 增加IsRent欄位

* ASP.NET Web API (REST API)

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
| Data | 資料物件 | object | |

* Data回傳參數說明

| 參數名稱      | 參數說明       | 型態 | 範例      |
| ------------- | -------------- | :--: | --------- |
| IsFavStation  | 是否為常用據點 | int  | 0:否 1:是 |
| GetCarTypeObj | 車型牌卡清單   | List |           |

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


## GetProject取得專案與資費(同站)
### [/api/GetProject/]

* 20210315修改 - 增加是否為常用據點欄位
* 20210324修改 - 增加搜尋欄位 
* 20210407修改 - input移除掉Seats 

* ASP.NET Web API (REST API)

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
| Data | 資料物件 | object | |

* Data回傳參數說明

| 參數名稱      | 參數說明                      |  型態   | 範例  |
| ------------- | ----------------------------- | :-----: | ----- |
| HasRentCard   | 是否有可租的卡片 **無用欄位** | boolean | false |
| GetProjectObj | 回傳清單                      |  List   |       |


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
| IsRent | 是否有車可租(BY據點) | string | Y/N/A(魔鬼剋星) |
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
| IsRent | 是否可租 | string | Y/N/A(魔鬼剋星) |
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

## GetCarTypeGroupList 取得車型清單

### [/api/GetCarTypeGroupList/]

* 20210331發佈

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱   | 參數說明 | 型態 | 範例 |
| ---------- | -------- | :--: | ---- |
| SeatGroups |          | List |      |


* SeatGroups資料物件說明

| 參數名稱 | 參數說明 | 型態 | 範例 |
| -------- | -------- | :--: | ---- |
| Seat     | 座椅數   | int  | 4    |
| CarInfos | 資料物件 |      |      |


* CarInfos 參數說明

| 參數名稱    | 參數說明 |  型態  | 範例         |
| ----------- | -------- | :----: | ------------ |
| Seat        | 座椅數   |  int   | 4            |
| CarType     | 車型名稱 | string | TOYOTA Altis |
| CarTypePic  | 車型圖片 | string | altis        |
| CarTypeName | 車型代碼 | string | ALTIS        |


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

## GetPolygon取得電子柵欄

### [/api/GetPolygon/]

* ASP.NET Web API (REST API)

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
| Data | 資料物件 | object | |

* Data回傳參數說明

| 參數名稱    | 參數說明           | 型態 | 範例                      |
| ----------- | ------------------ | :--: | ------------------------- |
| PolygonType | 電子柵欄模式       | int  | 0:優惠的取車;1:優惠的還車 |
| PolygonObj  | 電子柵欄經緯度清單 | List |                           |

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


## AnyRent 取得路邊租還車輛

### [/api/AnyRent/]

- 20210531補上

- ASP.NET Web API (REST API)

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
| Data | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱   | 參數說明     | 型態 | 範例 |
| ---------- | ------------ | :--: | ---- |
| AnyRentObj | 路邊租還清單 | list |      |


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

## GetAnyRentProject 取得專案與資費(路邊)

### [/api/GetAnyRentProject/]

- 20210531補上

- ASP.NET Web API (REST API)

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
| Data | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱             | 參數說明         | 型態 | 範例 |
| -------------------- | ---------------- | :--: | ---- |
| GetAnyRentProjectObj | 路邊租還專案清單 | list |      |


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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱     | 參數說明     | 型態 | 範例 |
| ------------ | ------------ | :--: | ---- |
| MotorRentObj | 路邊機車清單 | list |      |


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

## GetMotorRentProject 取得專案與資費(機車)

### [/api/GetMotorRentProject/]

- 20210531補上

- ASP.NET Web API (REST API)

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
| Data | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱           | 參數說明         | 型態 | 範例 |
| ------------------ | ---------------- | :--: | ---- |
| GetMotorProjectObj | 路邊機車專案清單 | List |      |


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

## GetEstimate 資費明細(預估租金)

### [/api/GetEstimate/]

* 20211104補上

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明         | 必要 |  型態  | 範例        |
| --------- | ---------------- | :--: | :----: | ----------- |
| ProjID    | 專案代碼         |  Y   | string | P735        |
| CarNo     | 車號             |  Y   | string | RCG-0521    |
| CarType   | 車型代碼         |  Y   | string |             |
| SDate     | 預計取車時間     |  N   | string |             |
| EDate     | 預計還車時間     |  N   | string |             |
| Insurance | 是否加購安心服務 |  Y   |  int   | 1           |
| MonId     | 選擇的訂閱制月租 |  Y   |  int   | 123456      |

* input範例(同站)

```
{
	"ProjID": "R220",
	"CarNo": "",
	"CarType": "ALTIS",
	"SDate": "2021-12-28 10:50:00",
	"EDate": "2021-12-28 11:50:00",
	"Insurance": 0,
	"MonId": 0
}
```

* input範例(路邊)

```
{
	"ProjID": "R221",
	"CarNo": "RCR-6795",
	"CarType": "",
	"SDate": "2021-12-28 10:50:00",
	"EDate": "2021-12-29 10:50:00",
	"Insurance": 0,
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
| Data         | 資料物件           | object |               |


* Data回傳參數說明

| 參數名稱           | 參數說明         | 型態 | 範例 |
| ------------------ | ---------------- | :--: | ---- |
| CarRentBill		 | 租金				| int  | 3795 |
| MileageBill		 | 里程費			| int  | 1280  |
| MileagePerKM		 | 里程每公里費用	| double | 3.2 |
| InsurancePerHour	 | 安心服務每小時   | int  | 50 |
| InsuranceBill 	 | 安心服務費用		| int | 1000 |
| TransDiscount		 | 轉乘優惠			| int | 0 |
| Bill				 | 總計				| int | 6075 |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "CarRentBill": 3795,
        "MileageBill": 1280,
        "MileagePerKM": 3.2,
        "InsurancePerHour": 50,
        "InsuranceBill": 1000,
        "TransDiscount": 0,
        "Bill": 6075
    }
}
```



# 預約以及訂單相關

## Booking 預約

### [/api/Booking/]

* 20210609補上

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明         | 必要 |  型態  | 範例        |
| --------- | ---------------- | :--: | :----: | ----------- |
| ProjID    | 專案代碼         |  Y   | string | P735        |
| SDate     | 預計取車時間     |  N   | string |             |
| EDate     | 預計還車時間     |  N   | string |             |
| CarNo     | 車號             |  Y   | string | RCG-0521    |
| CarType   | 車型代碼         |  Y   | string |             |
| Insurance | 是否加購安心服務 |  Y   |  int   | 1           |
| StationID | 據點代碼         |  Y   | string | X0II        |
| MonId     | 選擇的訂閱制月租 |  Y   |  int   | 123456      |
| PhoneLat  | 手機定位點(緯度) |  N   | double | 25.0212444  |
| PhoneLon  | 手機定位點(經度) |  N   | double | 121.4780778 |

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
    "MonId": 0,
	"PhoneLat": 25.0212444,
	"PhoneLon": 121.4780778
}
```

* input範例-路邊汽車預約

```
{
    "ProjID": "R221",
    "SDate": "",
    "EDate": "",
    "CarNo": "RDD-6775",
    "CarType": "PRIUSC",
    "Insurance": 0,
    "StationID": "",
    "MonId": 0,
	"PhoneLat": 25.0212444,
	"PhoneLon": 121.4780778
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
	"MonId": 0,
	"PhoneLat": 25.0212444,
	"PhoneLon": 121.4780778
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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱     | 參數說明                     |  型態  | 範例           |
| ------------ | ---------------------------- | :----: | -------------- |
| OrderNo      | 訂單編號                     | string | H10455246      |
| LastPickTime | 最晚的取車時間               | string | 20210608020120 |
| WalletNotice | 僅綁錢包通知0:不顯示  1:顯示 |  int   | 1              |

[^註]: WalletNotice參數 For電子錢包


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "OrderNo": "H13270976",
        "LastPickTime": "20211008161318",
        "WalletNotice": 1
    }
}

{
    "Result": "0",
    "ErrorCode": "ERR602",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "因授權失敗未完成預約，請檢查卡片餘額或是重新綁卡",
    "Data": {}
}
```

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                                                   | 說明                                                       |
| -------- | ---------------------------------------------------------- | ---------------------------------------------------------- |
| ERR156   | 目前預約數合計5筆，無法再預約                              | 三種類型預約合計已經有5筆，又再預約                        |
| ERR157   | 目前同站租還預約數合計3筆，無法再預約                      | 目前同站租還預約數合計3筆，又再預約                        |
| ERR158   | 目前路邊租還預約數合計1筆，無法再預約                      | 目前路邊租還預約數合計1筆5筆，又再預約                     |
| ERR159   | 目前機車預約數合計1筆，無法再預約                          | 目前機車預約數合計已經有1筆，又再預約                      |
| ERR160   | 預約時間有重疊                                             | 目前同站租還有重疊的預約                                   |
| ERR161   | 預約失敗                                                   | 同站租還預約不到車                                         |
| ERR162   | 預約失敗                                                   | 路邊租還預約不到車                                         |
| ERR163   | 預約失敗                                                   | 機車預約不到車                                             |
| ERR164   | 找不到此專案                                               | 預約時找不到這個專案代碼                                   |
| ERR165   | 找不到此專案                                               | 預約時找不到這個車號                                       |
| ERR233   | 尚有費用未繳，請先至未繳費用完成付款                       | 欠費的狀態不可以預約，繳完後才可以預約                     |
| ERR241   | 目前限3日以上的春節期間預約                                | 目前限3日以上的春節期間預約                                |
| ERR242   | 目前限1日以上的春節期間預約                                | 目前限1日以上的春節期間預約                                |
| ERR243   | 此據點因即將暫停營業恕無法接受您的預約，請重新選擇其他據點 | 此據點因即將暫停營業恕無法接受您的預約，請重新選擇其他據點 |
| ERR248   | 尚未開通汽車服務，請至會員中心確認                         | 尚未開通汽車服務，請至會員中心確認                         |
| ERR287   | 你的會員積分低於50分，故暫時無法租用車輛                   | 你的會員積分低於50分，故暫時無法租用車輛                   |
| ERR730   | 查詢綁定卡號失敗                                           | 查詢綁定卡號失敗                                           |
| ERR905   | 11/10 02:00~06:00系統維護暫停服務                          | 定維時使用                                                 |
| ERR602   | 因取授權失敗未完成預約，請檢查卡片餘額或是重新綁卡         | 因取授權失敗未完成預約                                     |
| ERR292   | 請先設定支付方式，才可以預約機車哦！                       | 請先設定支付方式，才可以預約機車哦！                       |
| ERR934   | 錢包餘額不足                                               | 錢包餘額不足                                               |

------

## BookingQuery 訂單列表

### [/api/BookingQuery/]

* 202108020補上

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明(可不傳參數)

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例      |
| -------- | -------- | :--: | :----: | --------- |
| OrderNo  | 訂單編號 |  N   | string | H12044254 |

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

| 參數名稱 | 參數說明     | 型態 | 範例 |
| -------- | ------------ | :--: | ---- |
| OrderObj | 訂單明細物件 | list |      |

* OrderObj回傳參數說明

| 參數名稱          | 參數說明                                                     |  型態   | 範例                  |
| ----------------- | ------------------------------------------------------------ | :-----: | --------------------- |
| StationInfo       | 據點資訊物件                                                 | object  |                       |
| Operator          | 營運商                                                       | string  | supplierIrent         |
| OperatorScore     | 評分                                                         |  float  | 5.0                   |
| CarTypePic        | 車輛圖片                                                     | string  | yaris                 |
| CarNo             | 車號                                                         | string  | RDH-2905              |
| CarBrend          | 廠牌                                                         | string  | TOYOTA                |
| CarTypeName       | 車型名稱                                                     | string  | YARIS                 |
| Seat              | 座椅數                                                       |   int   | 5                     |
| ParkingSection    | 停車格位置                                                   | string  |                       |
| IsMotor           | 是否為機車                                                   |   int   | 0:否 1:是             |
| CarOfArea         | 車輛圖顯示地區                                               | string  | 台北市                |
| CarLatitude       | 車緯度                                                       | decimal | 25.0726200            |
| CarLongitude      | 車經度                                                       | decimal | 121.5423700           |
| MotorPowerBaseObj | 機車電力資訊                                                 | object  | 當ProjType=4時才有值  |
| ProjType          | 專案類型<br>0:同站 3:路邊 4:機車                             |   int   | 0                     |
| ProjName          | 專案名稱                                                     | string  | 同站汽車110起推廣專案 |
| WorkdayPerHour    | 平日每小時費用                                               |   int   | 110                   |
| HolidayPerHour    | 假日每小時費用                                               |   int   | 168                   |
| MaxPrice          | 每日上限                                                     |   int   | 0                     |
| MaxPriceH         | 假日上限                                                     |   int   | 0                     |
| MotorBasePriceObj | 機車費用                                                     | object  | 當ProjType=4時才有值  |
| OrderStatus       | 訂單狀態<br>-1:前車未還（未到站） <br/>0:可取車<br/>1:用車中<br/>2:延長用車中<br/>3:準備還車<br/>4:逾時<br/>5:還車流程中（未完成還車） |   int   | 1                     |
| OrderNo           | 訂單編號                                                     | string  | H12044254             |
| StartTime         | 預計取車時間                                                 | string  | 2021-08-30 11:00:00   |
| PickTime          | 實際取車時間                                                 | string  | 2021-08-30 10:51:16   |
| ReturnTime        | 實際還車時間                                                 | string  | 2021-08-30 11:27:04   |
| StopPickTime      | 取車截止時間                                                 | string  | 2021-08-30 11:15:00   |
| StopTime          | 預計還車時間                                                 | string  | 2021-08-30 12:00:00   |
| OpenDoorDeadLine  | 使用期限                                                     | string  | 2021-08-30 11:42:34   |
| CarRentBill       | 預估租金                                                     |   int   | 110                   |
| MileagePerKM      | 每一公里里程費                                               |  float  | 3.1                   |
| MileageBill       | 預估里程費                                                   |   int   | 62                    |
| Insurance         | 是否可以使用安心服務(1:可 0:否)                              |   int   | 0                     |
| InsurancePerHour  | 安心保險每小時                                               |   int   | 50                    |
| InsuranceBill     | 預估安心保險費用                                             |   int   | 0                     |
| TransDiscount     | 轉乘優惠                                                     |   int   | 0                     |
| Bill              | 預估總金額                                                   |   int   | 172                   |
| DailyMaxHour      | 單日計費上限時數                                             |   int   | 10                    |
| CAR_MGT_STATUS    | 取還車狀態<br>0 = 尚未取車<br/>1 = 已經上傳出車照片<br/>2 = 已經簽名出車單<br/>3 = 已經信用卡認證<br/>4 = 已經取車(記錄起始時間)<br/>11 = 已經紀錄還車時間<br/>12 = 已經上傳還車角度照片<br/>13 = 已經上傳還車車損照片<br/>14 = 已經簽名還車單<br/>15 = 已經信用卡付款<br/>16 = 已經檢查車輛完成並已經解除卡號 |   int   | 4                     |
| AppStatus         | 1:尚未到取車時間(取車時間半小時前)<br/>2:立即換車(取車前半小時，前車尚未完成還車)<br/>3:開始使用(取車時間半小時前)<br/>4:開始使用-提示最晚取車時間(取車時間後~最晚取車時間)<br/>5:操作車輛(取車後) 取車時間改實際取車時間<br/>6:操作車輛(準備還車)<br/>7:物品遺漏(再開一次車門)<br/>8:鎖門並還車(一次性開門申請後) |   int   | 6                     |
| RenterType        | 承租人類型<br>1:主要承租人 2:共同承租人                      |   int   | 1                     |

* StationInfo回傳參數說明

| 參數名稱            | 參數說明            |  型態  | 範例                                                         |
| ------------------- | ------------------- | :----: | ------------------------------------------------------------ |
| StationID           | 據點代碼            | string | X0IN                                                         |
| StationName         | 據點名稱            | string | iRent-Toyota濱江營業所站                                     |
| Tel                 | 電話                | string | 02-2516-3816                                                 |
| ADDR                | 地址                | string | 台北市中山區濱江街269號                                      |
| Latitude            | 緯度                | string | 25.072589                                                    |
| Longitude           | 經度                | string | 121.542617                                                   |
| Content             | 其他說明            | string | YARIS                                                        |
| IsRent              |                     | string | null                                                         |
| ContentForAPP       | 據點描述（app顯示） | string | 1.濱江營業所前方停車場\n2.固定招牌旁車位\n3.自由進出\n4.請勿停在專屬車位外 |
| IsRequiredForReturn | 還車位置資訊必填    |  int   | 0                                                            |
| StationPic          | 據點照片            |  list  |                                                              |

* StationPic回傳參數說明

| 參數名稱       | 參數說明 |  型態  | 範例                                                         |
| -------------- | -------- | :----: | ------------------------------------------------------------ |
| StationPic     | 據點照片 | string | https://irentv2data.blob.core.windows.net/station/X0IN_1_20210209000000.png |
| PicDescription | 據點說明 | string | 停車場位置\n                                                 |

* MotorPowerBaseObj回傳參數說明

| 參數名稱         | 參數說明 | 型態  | 範例 |
| ---------------- | -------- | :---: | ---- |
| Power            | 剩餘電量 | float |      |
| RemainingMileage | 剩餘里程 | float |      |

* MotorBasePriceObj回傳參數說明

| 參數名稱        | 參數說明 | 型態  | 範例 |
| --------------- | -------- | :---: | ---- |
| BaseMinutes     | 剩餘電量 |  int  |      |
| BasePrice       | 剩餘里程 |  int  |      |
| PerMinutesPrice | 剩餘電量 | float |      |
| MaxPrice        | 剩餘里程 |  int  |      |

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

## BookingFinishQuery 完成的訂單查詢

### [/api/BookingFinishQuery/]

* 202108020補上

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱    | 參數說明                           | 必要 | 型態 | 範例                              |
| ----------- | ---------------------------------- | :--: | :--: | --------------------------------- |
| NowPage     | 現在頁碼                           |  N   | int  | 1                                 |
| ShowOneYear | 顯示一整年 (202101 該參數已無作用) |  Y   | int  | 0:否 20XX代表取出該年度的所有訂單 |

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱        | 參數說明       | 型態 | 範例 |
| --------------- | -------------- | :--: | ---- |
| TotalPage       | 總頁數         | int  | 6    |
| OrderFinishObjs | 完成的訂單清單 | list |      |

* OrderFinishObjs回傳參數說明

| 參數名稱      | 參數說明           |  型態  | 範例                 |
| ------------- | ------------------ | :----: | -------------------- |
| RentYear      | 年分               |  int   |                      |
| OrderNo       | 訂單編號           | string | supplierIrent        |
| CarNo         | 車號               | string | 5.0                  |
| ProjType      | 專案類型           |  int   | 0:同站 3:路邊 4:機車 |
| RentDateTime  | 取車時間 月日時分  | string |                      |
| TotalRentTime | 總租用時數         | string |                      |
| Bill          | 總租金             | string | YARIS                |
| UniCode       | 統編               | string | 5                    |
| StationName   |                    | string |                      |
| CarOfArea     | 車輛圖顯示地區     | string | 0:否 1:是            |
| CarTypePic    | 車輛圖片           | string | 台北市               |
| IsMotor       | 是否為機車         |  int   | 1:是 0:否            |
| IsJointOrder  | 是否為共同承租訂單 |  int   | 1:是 0:否            |

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

## BookingDelete 刪除訂單

### [/api/BookingDelete/]

* 20210820補上

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |


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

## OrderDetail 訂單明細

### [/api/OrderDetail/]

* 20210819修改

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例      |
| -------- | -------- | :--: | :----: | --------- |
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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱            | 參數說明          |  型態  | 範例                                                   |
| ------------------- | ----------------- | :----: | ------------------------------------------------------ |
| OrderNo             | 訂單編號          | string | H10455246                                              |
| ContactURL          | 合約網址          | string |                                                        |
| Operator            | 營運商            | string | supplierIrent                                          |
| CarTypePic          | 車輛圖片          | string | iretScooter                                            |
| CarNo               | 車號              | string | RAA-1122                                               |
| Seat                | 座椅數            |  int   | 5                                                      |
| CarBrend            | 汽車廠牌          | string | KYMCO                                                  |
| CarTypeName         | 車型名稱          | string | MANY-110                                               |
| StationName         | 據點名稱          | string | iRent路邊租還[機車]_台北                               |
| OperatorScore       | 評分              | float  | 5.0                                                    |
| ProjName            | 專案名稱          | string | 10載便利北北桃                                         |
| CarRentBill         | 車輛租金          |  int   | 2000                                                   |
| TotalHours          | 使用時數          | string | 0天0時0分                                              |
| MonthlyHours        | 月租折抵          | string | 0天0時0分                                              |
| GiftPoint           | 折抵時數          | string | 0天0時0分                                              |
| PayHours            | 計費時數          | string | 0天0時0分                                              |
| MileageBill         | 里程費            |  int   | 100                                                    |
| InsuranceBill       | 安心服務費        |  int   | 100                                                    |
| EtagBill            | eTag費用          |  int   | 10                                                     |
| OverTimeBill        | 逾時費            |  int   | 100                                                    |
| ParkingBill         | 代收停車費        |  int   | 100                                                    |
| TransDiscount       | 轉乘優惠折抵      |  int   | 100                                                    |
| TotalBill           | 總金額            |  int   | 1000                                                   |
| InvoiceType         | 發票類型          | string | 1:愛心碼 2:email 3:二聯 4:三聯 5:手機條碼 6:自然人憑證 |
| CARRIERID           | 載具條碼          | string |                                                        |
| NPOBAN              | 捐贈碼            | string |                                                        |
| NPOBAN_Name         | 捐贈協會名稱      | string |                                                        |
| Unified_business_no | 統編              | string | 50885758                                               |
| InvoiceNo           | 發票號碼          | string | AA12345678                                             |
| InvoiceDate         | 發票日期          | string | 2021-03-01                                             |
| InvoiceBill         | 發票金額          |  int   | 1000                                                   |
| InvoiceURL          | 發票網址          | string |                                                        |
| StartTime           | 開始時間          | string | 2021-05-14 00:02                                       |
| EndTime             | 結束時間          | string | 2021-05-14 00:02                                       |
| Millage             | 里程              | float  | 1234.5                                                 |
| CarOfArea           | 據點區域          | string | 北北桃                                                 |
| DiscountAmount      | 優惠折抵金額      |  int   | 100                                                    |
| DiscountName        | 折抵專案名稱      | string |                                                        |
| CtrlBill            | 營損-車輛調度費   |  int   | 0                                                      |
| ClearBill           | 營損-清潔費       |  int   | 0                                                      |
| EquipBill           | 營損-物品損壞     |  int   | 0                                                      |
| ParkingBill2        | 營損-非約定停車費 |  int   | 0                                                      |
| TowingBill          | 營損-拖吊費       |  int   | 0                                                      |
| OtherBill           | 營損-其他費用     |  int   | 0                                                      |
| UseOrderPrice       | 使用訂金          |  int   | 0                                                      |
| ReturnOrderPrice    | 返還訂金          |  int   | 0                                                      |
| ChangePoint         | 換電時數          |  int   | 0                                                      |
| ChangeTimes         | 換電次數          |  int   | 0                                                      |
| RSOC_S              | 取車電量          | float  | 0                                                      |
| RSOC_E              | 還車電量          | float  | 0                                                      |
| RewardPoint         | 獎勵時數          |  int   | 0                                                      |
| TotalRewardPoint    | 總回饋時數        |  int   | 0                                                      |
| RenterType          | 共同承租人類型    |  int   | 1:主要承租人  2:共同承租人                             |


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
}
```

## GetOrderInsuranceInfo 訂單安心服務資格及價格查詢

### [/api/GetOrderInsuranceInfo/]

* 20210825新增

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱              | 參數說明                                                     |  型態  | 範例                                                   |
| --------------------- | ------------------------------------------------------------ | :----: | ------------------------------------------------------ |
| Insurance             | 可否使用安心服務<br>0:不可使用<BR>1:可使用，預約沒選<br>2:可使用，預約有選 |  int   | 0                                                      |
| MainInsurancePerHour  | 主承租人每小時安心服務價格                                   |  int   | 50                                                     |
| JointInsurancePerHour | 單一共同承租人每小時安心服務價格                             |  int   | 若該訂單沒有共同承租邀請對象，該欄位為0                |
| JointAlertMessage     | 共同承租提示訊息                                             | string | 若該訂單沒有未回應的共同承租邀被邀請人，該欄位為空字串 |


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

## GetCancelOrderList 取得取消訂單列表

### [/api/GetCancelOrderList/]

* 20211103新增

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| NowPage  | 現在頁碼 |  Y   | string | H0000029 |


* input範例

```
{
    "NowPage" : "1"
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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱              | 參數說明                                                     |  型態  | 範例                                                   |
| --------------------- | ------------------------------------------------------------ | :----: | ------------------------------------------------------ |
| order_number          | 訂單編號                                                     |string  |H11538852|
| CarNo  | 車號                                                  |  int   | RDE-6193                                                     |
| Price | 預估租金                             |  int   | 200               |
| ProjID     | 專案代碼                                             | string | R220 |
| SD     | 預計取車時間                                             | datetime | 2021-07-27T12:10:00 |
| ED     | 預計還車時間                                             | datetime | 2021-07-27T13:10:00 |
| Seat     | 座椅數                                             | int | 4 |
| CarBrend     | 車子品牌                                             | string | TOYOTA |
| Score     | 分數                                             | float | 5.0 |
| OperatorICon     | ?                                             | string | supplierIrent |
| CarTypeImg     | 車型圖                                             | string | priusC |
| CarTypeName     | 車型名稱                                           | string | PRIUSc |
| PRONAME     | 專案名稱                                             | string | 同站汽車110起推廣專案 |
| MilOfHours     | 預估每小時多少公里                                | int | 20 |
| MilageUnit     | 每公里多少錢                                             | float | 3.1 |
| Milage     | 預估里程費                                             | int | 62 |
| CarOfArea     | 站別類型                                             | string | 同站 |
| StationName     | 站別名稱                                             | string | iRent礁溪轉運站-內站 |
| IsMotor     | 是否為機車(0:否、1:是)                                             | int | 0 |
| WeekdayPrice     | 平日價                                             | float | 2300.0 |
| HoildayPrice     | 假日售價                                             | float | 2300.0 |
| WeekdayPriceByMinutes     | 假日售價                                             | float | 0.0 |
| HoildayPriceByMinutes     | 假日售價                                             | float | 0.0 |
| CarRentBill     | 預估租金                                             | int | 110 |
| InsuranceBill     | 預估安心保險費用                                             | int | 0 |
| TransDiscount     | 轉乘優惠                                             | int | 0 |
| MileageBill     | 預估里程費                                             | int | 62 |
| Bill     | 預估總金額                                             | int | 172 |
| cancel_status     | 取消狀態文字(目前分為"已取消"、"授權失敗已取消")   | string | "授權失敗已取消" |



* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "TotalPage": 2,
        "CancelObj": [
            {
                "order_number": "H11538852",
                "CarNo": "RDE-6193",
                "Price": 110,
                "ProjID": "R220",
                "SD": "2021-07-27T12:10:00",
                "ED": "2021-07-27T13:10:00",
                "Seat": 5,
                "CarBrend": "TOYOTA",
                "Score": 5.0,
                "OperatorICon": "supplierIrent",
                "CarTypeImg": "priusC",
                "CarTypeName": "PRIUSc",
                "PRONAME": "同站汽車110起推廣專案",
                "MilOfHours": 20,
                "MilageUnit": 3.1,
                "Milage": 62,
                "CarOfArea": "同站",
                "StationName": "iRent礁溪轉運站-內站",
                "IsMotor": 0,
                "WeekdayPrice": 2300.0,
                "HoildayPrice": 2300.0,
                "WeekdayPriceByMinutes": 0.0,
                "HoildayPriceByMinutes": 0.0,
                "CarRentBill": 110,
                "InsuranceBill": 0,
                "TransDiscount": 0,
                "MileageBill": 62,
                "Bill": 172,
                "cancel_status": "已取消"
            },
            {
                "order_number": "H11524658",
                "CarNo": "RDF-0272",
                "Price": 110,
                "ProjID": "R220",
                "SD": "2021-07-26T16:10:00",
                "ED": "2021-07-26T17:10:00",
                "Seat": 5,
                "CarBrend": "TOYOTA",
                "Score": 5.0,
                "OperatorICon": "supplierIrent",
                "CarTypeImg": "priusC",
                "CarTypeName": "PRIUSc",
                "PRONAME": "同站汽車110起推廣專案",
                "MilOfHours": 20,
                "MilageUnit": 3.1,
                "Milage": 62,
                "CarOfArea": "同站",
                "StationName": "iRent礁溪火車站",
                "IsMotor": 0,
                "WeekdayPrice": 2300.0,
                "HoildayPrice": 2300.0,
                "WeekdayPriceByMinutes": 0.0,
                "HoildayPriceByMinutes": 0.0,
                "CarRentBill": 110,
                "InsuranceBill": 0,
                "TransDiscount": 0,
                "MileageBill": 62,
                "Bill": 172,
                "cancel_status": "已取消"
            }
        ]
    }
}
```

# 取還車跟車機操控相關

## ChangeUUCard 變更悠遊卡

### [/api/ChangeUUCard/]

* 20210415發佈

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱 | 參數說明 | 型態 | 範例      |
| -------- | -------- | :--: | --------- |
| HasBind  | 是否綁定 | int  | 0:否 1:是 |

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
## BookingStart 汽車取車

### [/api/BookingStart/]

* 20210820補資料

* ASP.NET Web API (REST API)

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
| PhoneLat | 手機定位點(緯度) | N | double | 25.0212444 |
| PhoneLon | 手機定位點(經度) | N | double | 121.4780778 |


* input範例

```
{
    "OrderNo": "H10641049",
    "ED": "",
    "SKBToken": "",
    "Insurance": 0,
	"PhoneLat": 25.0212444,
	"PhoneLon": 121.4780778
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
| Data         | 資料物件           | object |               |

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

| 錯誤代碼 | 錯誤訊息                                           | 說明                                     |
| -------- | -------------------------------------------------- | ---------------------------------------- |
| ERR171   | 超過取車時間或此訂單已失效                         | 取車時超過取車時效或已被取消             |
| ERR172   | 目前尚有合約使用中，請確認是否有未完成還車         | 取車時尚有訂單未完成                     |
| ERR173   | 預計還車時間不正確                                 | 取車時修改的預計還車時間，格式不正確     |
| ERR174   | 預計還車時間需大於現在                             | 取車時修改的預計還車時間小於現在時間     |
| ERR234   | 尚有費用未繳，請先至未繳費用完成付款               | 欠費的狀態不可以取車，繳完後才可以取車   |
| ERR239   | 會員狀態審核不通過不可取車                         | 會員狀態審核不通過不可取車               |
| ERR240   | 前車未還，請聯絡客服                               | 前車未還，請聯絡客服                     |
| ERR287   | 你的會員積分低於50分，故暫時無法租用車輛           | 你的會員積分低於50分，故暫時無法租用車輛 |
| ERR468   | 車機回報資訊異常，請重新再試                       |                                          |
| ERR603   | 因取授權失敗未完成取車，請檢查卡片餘額或是重新綁卡 | 因取授權失敗未完成取車                   |
| ERR730   | 查詢綁定卡號失敗                                   | 查詢綁定卡號失敗                         |

------

## BookingStartMotor 機車取車

### [/api/BookingStartMotor/]

* 20210820補資料

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H10641049 |
| PhoneLat | 手機定位點(緯度) | N | double | 25.0212444 |
| PhoneLon | 手機定位點(經度) | N | double | 121.4780778 |

* input範例

```
{
    "OrderNo": "H10641049",
	"PhoneLat": 25.0212444,
	"PhoneLon": 121.4780778
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
| Data         | 資料物件           | object |               |

- Data參數說明

| 參數名稱     | 參數說明        |  型態  | 範例         |
| ------------ | --------------- | :----: | ------------ |
| BLEDEVICEID  | 藍芽device name | string | iMoto_bd21   |
| BLEDEVICEPWD | 藍芽密碼        | string | QUEzMDE3ODUy |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
       "BLEDEVICEID":"iMoto_bd21",
       "BLEDEVICEPWD":"QUEzMDE3ODUy"    
    }
}
```

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                                 | 說明                                     |
| -------- | ---------------------------------------- | ---------------------------------------- |
| ERR171   | 超過取車時間或此訂單已失效               | 取車時超過取車時效或已被取消             |
| ERR239   | 會員狀態審核不通過不可取車               | 會員狀態審核不通過不可取車               |
| ERR240   | 前車未還，請聯絡客服                     | 前車未還，請聯絡客服                     |
| ERR287   | 你的會員積分低於50分，故暫時無法租用車輛 | 你的會員積分低於50分，故暫時無法租用車輛 |
| ERR292   | 請先設定支付方式，才可以預約機車哦！     | 請先設定支付方式，才可以預約機車哦！     |

## BookingExtend延長用車

### [/api/BookingExtend/]

* 20211028補資料

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例                |
| -------- | -------- | :--: | :----: | ------------------- |
| OrderNo  | 訂單編號 |  Y   | string | H11766161           |
| ED       | 還車時間 |  Y   | string | 2021-10-28 15:00:00 |

* input範例

```
{
    "OrderNo" : "H11766161",
    "ED" : "2021-10-28 15:00:00"
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
| Data         | 資料物件           | object |               |

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

{
    "Result": "0",
    "ErrorCode": "ERR604",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "延長用車取授權未成功，請盡速檢查卡片餘額或是重新綁卡",
    "Data": {}
}
```

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                                                     | 說明                                   |
| :------- | ------------------------------------------------------------ | -------------------------------------- |
| ERR176   | 延長用車時間不正確                                           | 延長用車時間參數不正確                 |
| ERR177   | 延長用車時間需大於現在                                       | 延長用車時間比現在時間小               |
| ERR178   | 延長用車時間需大於原預計還車時間                             | 延長用車時間比原預計還車時間小         |
| ERR179   | 用車時間合計不能超過七天                                     | 延長用車後，合計此合約用車時間大於七天 |
| ERR180   | 此訂單不符合延長用車或找不到此訂單。                         | 資料表查詢不到此筆訂單或不符合延長用車 |
| ERR181   | 此車輛已經其他預約，無法延長用車。                           | 此車輛已經其他預約，無法延長用車。     |
| ERR182   | 您延長用車時間重疊到您之後的預約用車時間，請先取消重疊的訂單再做延長。 | 延長用車時間重疊到之後的預約用車時間   |
| ERR237   | 延長用車時間最少1小時                                        | 延長用車時間最少1小時                  |
| ERR604   | 延長用車取授權未成功，請盡速檢查卡片餘額或是重新綁卡         | 延長用車取授權未成功                   |
------
## ReturnCar 還車

### [/api/ReturnCar/]

* 20211012 調整

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H10641049 |
| PhoneLat | 手機定位點(緯度) | N | double | 25.0212444 |
| PhoneLon | 手機定位點(經度) | N | double | 121.4780778 |


* input範例

```
{
    "OrderNo": "H10641049",
	"PhoneLat": 25.0212444,
	"PhoneLon": 121.4780778
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
| Data         | 資料物件           | object |               |

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

## GetPayDetail 取得租金明細

### [/api/GetPayDetail/]

* 20210527補資料

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱      | 參數說明       | 必要 |  型態  | 範例     |
| ------------- | -------------- | :--: | :----: | -------- |
| OrderNo       | 訂單編號       |  Y   | string | H0002630 |
| Discount      | 折抵汽車時數   |  Y   |  int   | 0        |
| MotorDiscount | 折抵機車分鐘數 |  Y   |  int   | 0        |


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

| 參數名稱         | 參數說明                       |  型態  | 範例 |
| ---------------- | ------------------------------ | :----: | ---- |
| CanUseDiscount   | 是否可使用點數折扣 0:否 1:是   |  int   | 1    |
| CanUseMonthRent  | 是否可使用月租時間 0:否 1:是   |  int   | 1    |
| IsMonthRent      | 是否為月租 0:否 1:是           |  int   | 0    |
| IsMotor          | 是否為機車 0:否 1:是           |  int   | 0    |
| UseOrderPrice    | 使用訂金                       |  int   | 0    |
| ReturnOrderPrice | 返還訂金                       |  int   | 0    |
| FineOrderPrice   | 沒收訂金                       |  int   | 0    |
| Rent             | 訂單基本資訊                   | Object |      |
| CarRent          | 汽車相關資料                   | Object |      |
| MotorRent        | 機車相關資訊                   | Object |      |
| MonthRent        | 月租相關資訊                   | Object |      |
| MonBase          | 月租下拉                       |  List  |      |
| ProType          | 專案類型 0:同站 3:路邊 4:機車  |  int   | 0    |
| PayMode          | 計費模式 0:以時計費 1:以分計費 |  int   | 0    |
| DiscountAlertMsg | 不可使用折抵時的訊息提示       | string |      |
| NowSubsCards     | 目前可使用訂閱制月租           |  List  |      |

* Rent資料物件說明

| 參數名稱                     | 參數說明                   |  型態  | 範例                |
| ---------------------------- | -------------------------- | :----: | ------------------- |
| CarNo                        | 車號                       | string | RBJ-9397            |
| BookingStartDate             | 實際取車時間               | string | 2021-09-06 13:21:00 |
| BookingEndDate               | 預計還車時間               | string | 2021-09-06 14:30:00 |
| RentalDate                   | 實際還車時間               | string | 2021-09-06 13:27:00 |
| RentalTimeInterval           | 實際租用時數               | string | 60                  |
| RedeemingTimeInterval        | 可折抵時數                 | string | 565                 |
| RedeemingTimeCarInterval     | 可折抵時數(汽車)           | string | 565                 |
| RedeemingTimeMotorInterval   | 可折抵時數(機車)           | string | 331                 |
| ActualRedeemableTimeInterval | 代表該「實際」可折抵的時數 | string | 60                  |
| RemainRentalTimeInterval     | 代表折抵後的租用時數       | string | 0                   |
| UseMonthlyTimeInterval       | 月租專案時數折抵顯示       | string | 0                   |
| UseNorTimeInterval           | 一般時段時數折抵           | string | 0                   |
| RentBasicPrice               | 每小時基本租金             |  int   | 0                   |
| CarRental                    | 車輛租金                   |  int   | 125                 |
| MileageRent                  | 里程費用                   |  int   | 0                   |
| ETAGRental                   | ETAG費用                   |  int   | 0                   |
| OvertimeRental               | 逾時費用                   |  int   | 0                   |
| TotalRental                  | 總計                       |  int   | 125                 |
| ParkingFee                   | 停車費用                   |  int   | 0                   |
| TransferPrice                | 轉乘費用                   |  int   | 0                   |
| InsurancePurePrice           | 安心服務                   |  int   | 0                   |
| InsuranceExtPrice            | 安心服務延長費用           |  int   | 0                   |
| PreAmount                    | 預授權金額                 |  int   | 0                   |
| DiffAmount                   | 差額                       |  int   | 0                   |

* CarRent資料物件說明

| 參數名稱           | 參數說明       | 型態  | 範例 |
| ------------------ | -------------- | :---: | ---- |
| HourOfOneDay       | 多少小時算一天 |  int  | 10   |
| HoildayPrice       | 假日金額       |  int  | 1980 |
| WorkdayPrice       | 平日金額       |  int  | 1250 |
| HoildayOfHourPrice | 假日每小時金額 |  int  | 198  |
| WorkdayOfHourPrice | 平日每小時金額 |  int  | 125  |
| MilUnit            | 每公里金額     | float | 3.1  |

* MotorRent資料物件說明

| 參數名稱        | 參數說明   | 型態  | 範例 |
| --------------- | ---------- | :---: | ---- |
| BaseMinutes     | 基本時數   |  int  | 6    |
| BaseMinutePrice | 基本費     |  int  | 12   |
| MinuteOfPrice   | 每分鐘價格 | float | 2.0  |


* MonthRent資料物件說明

| 參數名稱    | 參數說明 | 型態  | 範例 |
| ----------- | -------- | :---: | ---- |
| WorkdayRate | 平日價格 | float | 0.0  |
| HoildayRate | 假日價格 | float | 0.0  |

* MonBase集合物件說明

| 參數名稱      | 參數說明     |  型態  | 範例  |
| ------------- | ------------ | :----: | ----- |
| MonthlyRentId | 月租訂單編號 |  int   | 12345 |
| ProjNM        | 月租方案名稱 | string |       |


* NowSubsCards集合物件說明

| 參數名稱           | 參數說明         |   型態   | 範例     |
| ------------------ | ---------------- | :------: | -------- |
| MonthlyRentId      | 月租編號         |   int    | 12345    |
| ProjID             | 月租方案代碼     |  string  | MR66     |
| MonProPeroid       | 總期數           |   int    | 3        |
| ShortDays          | 短期總天數       |   int    | 0        |
| MonProjNM          | 月租方案名稱     |  string  | MR66測試 |
| WorkDayHours       | 平日時數         |  double  | 0        |
| HolidayHours       | 假日時數         |  double  | 0        |
| MotoTotalMins      | 機車分鐘數       |  double  | 0        |
| WorkDayRateForCar  | 汽車平日優惠費率 |  double  | 99.0     |
| HoildayRateForCar  | 汽車假日優惠費率 |  double  | 168.0    |
| WorkDayRateForMoto | 機車平日優惠費率 |  double  | 1.5      |
| HoildayRateForMoto | 機車假日優惠費率 |  double  | 2.0      |
| StartDate          | 月租起日         | datetime |          |
| EndDate            | 月租迄日         | datetime |          |


* Output範例(汽車)

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
            "CarNo": "RCN-0278",
            "BookingStartDate": "2021-12-29 10:02:00",
            "BookingEndDate": "2021-12-29 11:30:00",
            "RentalDate": "2021-12-29 10:05:00",
            "RentalTimeInterval": "60",
            "RedeemingTimeInterval": "0",
            "RedeemingTimeCarInterval": "0",
            "RedeemingTimeMotorInterval": "0",
            "ActualRedeemableTimeInterval": "60",
            "RemainRentalTimeInterval": "60",
            "UseMonthlyTimeInterval": "0",
            "UseNorTimeInterval": "0",
            "RentBasicPrice": 0,
            "CarRental": 110,
            "MileageRent": 0,
            "ETAGRental": 0,
            "OvertimeRental": 0,
            "TotalRental": 110,
            "ParkingFee": 0,
            "TransferPrice": 0,
            "InsurancePurePrice": 0,
            "InsuranceExtPrice": 0,
            "PreAmount": 170,
            "DiffAmount": -60
        },
        "CarRent": {
            "HourOfOneDay": 10,
            "HoildayPrice": 1680,
            "WorkdayPrice": 1100,
            "HoildayOfHourPrice": 168,
            "WorkdayOfHourPrice": 110,
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
        "MonBase": [],
        "ProType": 0,
        "PayMode": 0,
        "DiscountAlertMsg": "",
        "NowSubsCards": null
    }
}
```

- Output範例(機車)

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
        "IsMotor": 1,
        "UseOrderPrice": 0,
        "ReturnOrderPrice": 0,
        "FineOrderPrice": 0,
        "Rent": {
            "CarNo": "EWA-0122",
            "BookingStartDate": "2021-09-06 10:57:00",
            "BookingEndDate": "2021-09-06 11:03:00",
            "RentalDate": "2021-09-06 11:03:00",
            "RentalTimeInterval": "6",
            "RedeemingTimeInterval": "600",
            "RedeemingTimeCarInterval": "600",
            "RedeemingTimeMotorInterval": "0",
            "ActualRedeemableTimeInterval": "6",
            "RemainRentalTimeInterval": "6",
            "UseMonthlyTimeInterval": "0",
            "UseNorTimeInterval": "0",
            "RentBasicPrice": 12,
            "CarRental": 12,
            "MileageRent": 0,
            "ETAGRental": 0,
            "OvertimeRental": 0,
            "TotalRental": 0,
            "ParkingFee": 0,
            "TransferPrice": 12,
            "InsurancePurePrice": 0,
            "InsuranceExtPrice": 0
        },
        "CarRent": {
            "HourOfOneDay": 0,
            "HoildayPrice": 0,
            "WorkdayPrice": 0,
            "HoildayOfHourPrice": 0,
            "WorkdayOfHourPrice": 0,
            "MilUnit": 0.0
        },
        "MotorRent": {
            "BaseMinutes": 6,
            "BaseMinutePrice": 12,
            "MinuteOfPrice": 2.0
        },
        "MonthRent": {
            "WorkdayRate": 0.0,
            "HoildayRate": 0.0
        },
        "MonBase": [],
        "ProType": 4,
        "PayMode": 1,
        "DiscountAlertMsg": "",
        "NowSubsCards": null
    }
}
```

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                           | 說明                         |
| -------- | ---------------------------------- | ---------------------------- |
| ERR203   | 找不到符合的訂單編號               | 租金計算時找不到此訂單編號   |
| ERR204   | 訂單狀態不符                       | 租金計算時訂單狀態不符       |
| ERR206   | 折抵時數須以30分鐘為單位           | 汽車使用的折抵時數非30的倍數 |
| ERR207   | 折抵時數超過可使用的時數           | 折抵時數超過目前所擁有的時數 |
| ERR208   | 還車已超過三十分鐘，請重新點擊還車 | 點下還車鍵已超過三十分鐘     |
| ERR303   | 折抵時數總和超過使用時數           | 折抵時數總和超過使用時數     |
| ERR914   | 資料邏輯錯誤                       | 資料邏輯錯誤                 |

## CreditAuth 付款與還款

### [/api/CreditAuth/]

* 20210909 補資料

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱     | 參數說明                       | 必要 |  型態  | 範例     |
| ------------ | ------------------------------ | :--: | :----: | -------- |
| PayType      | 付款模式(0:租金、1:罰金/補繳)  |  Y   |  int   | 0        |
| OrderNo      | 訂單編號                       |  Y   | string | H1254786 |
| CNTRNO       | 罰金或補繳代碼                 |  Y   |  int   | 0        |
| CheckoutMode | 付款方式(0:信用卡、1:和雲錢包) |  Y   |  int   | 0        |
| OnceStore    | 單次儲值餘額 (1:是、0:否)      |  Y   |  int   | 0        |

* input範例(租金)

```
{
    "PayType": "0",
    "OrderNo": "H1254786",
    "CNTRNO": 0,
    "CheckoutMode":0,
    "OnceStore": 0
}
```

* input範例(罰金)

```
{
    "PayType": "1",
    "OrderNo": "",
    "CNTRNO": 1554880,
    "CheckoutMode":0,
    "OnceStore": 0
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
| Data         | 資料物件           | object |               |

* Data參數說明

| 參數名稱    | 參數說明 | 型態 | 範例 |
| ----------- | -------- | :--: | ---- |
| RewardPoint | 換電獎勵 | int  | 25   |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "RewardPoint": 25,
    }
}
```

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                                         | 說明                                             |
| -------- | ------------------------------------------------ | ------------------------------------------------ |
| ERR185   | 找不到符合的訂單編號                             | 還車檢查時，找不到符合的訂單編號                 |
| ERR186   | 請先熄火                                         | 還車檢查時，未熄火                               |
| ERR187   | 請先關閉電源                                     | 還車檢查時，末關閉電源                           |
| ERR188   | 請將車輛移至還車範圍內                           | 還車檢查時，不在還車範圍內                       |
| ERR195   | 找不到此卡號                                     | 信用卡解綁時找不到此信用卡號                     |
| ERR197   | 刷卡授權失敗，請洽發卡銀行                       | 刷卡授權發生錯誤                                 |
| ERR203   | 找不到符合的訂單編號                             | 租金計算時找不到此訂單編號                       |
| ERR209   | 已完成還車付款，請勿重覆付款                     | 已完成還車付款，請勿重覆付款                     |
| ERR210   | 尚未完成還車步驟，無法還車付款                   | 未符合還車付款狀態，又重覆點下付款               |
| ERR223   | 找不到符合的訂單編號                             | 取/還車回饋時，找不到符合的訂單編號              |
| ERR231   | 目前讀不到iButton狀態，請檢查iButton扣環是否脫落 | 目前讀不到iButton狀態，請檢查iButton扣環是否脫落 |
| ERR244   | 系統偵測到異常，請重新進入                       | 系統偵測到異常，請重新進入                       |
| ERR245   | 還車已超過十四分鐘，請重新計算費率               | 還車時間過久沒重新計價                           |
| ERR400   | 車機回報資訊異常，請重新再試                     | 還車檢查時，回傳的車機編號與設定的不符合         |
| ERR429   | 車門目前未關閉，無法再執行上鎖/解鎖指令。        | 車門目前未關閉，無法再執行上鎖/解鎖指令。        |
| ERR434   | 無法取得車門狀態                                 | 無法取得車門狀態                                 |
| ERR439   | 車子室內燈或大燈未關                             | 車子室內燈或大燈未關                             |
| ERR730   | 查詢綁定卡號失敗                                 | 查詢綁定卡號失敗                                 |
| ERR932   | 錢包未開通                                       | 錢包未開通                                       |
| ERR933   | 錢包扣款失敗                                     | 錢包扣款失敗                                     |
| ERR934   | 錢包餘額不足                                     | 錢包餘額不足                                     |

# 月租訂閱制相關


## GetMonthList 取得訂閱制月租列表/我的所有方案

###  [/api/GetMonthList/]

* 20210510發佈

* ASP.NET Web API (REST API)

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

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data資料物件說明,汽車牌卡(ReMode=1, IsMoto=0)

| 參數名稱    | 參數說明                    | 型態 | 範例 |
| ----------- | --------------------------- | :--: | ---- |
| IsMotor     | 是否為機車                  | int  | 0    |
| NorMonCards | 汽車牌卡                    | List |      |
| MixMonCards | 城市車手牌卡                | List |      |
| ReMode      | 模式(1:月租，2我的所有方案) | int  | 1    |


* Data資料物件說明,機車牌卡(ReMode=1, IsMoto=1)

| 參數名稱    | 參數說明                    | 型態 | 範例 |
| ----------- | --------------------------- | :--: | ---- |
| IsMotor     | 是否為機車                  | int  | 1    |
| NorMonCards | 機車牌卡                    | List |      |
| ReMode      | 模式(1:月租，2我的所有方案) | int  | 1    |

* Data資料物件說明,我的所有方案(ReMode=2)

| 參數名稱 | 參數說明                    |  型態  | 範例 |
| -------- | --------------------------- | :----: | ---- |
| MyCar    | 汽車牌卡                    | object |      |
| MyMoto   | 機車牌卡                    | object |      |
| ReMode   | 模式(1:月租，2我的所有方案) |  int   | 2    |


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

-  Output範例,汽車牌卡(ReMode=1, IsMoto=0)


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

## GetMonthGroup 月租專案群組

### [/api/GetMonthGroup/]

* 20210510發佈

* ASP.NET Web API (REST API)

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

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data資料物件說明

| 參數名稱    | 參數說明                    | 型態    | 範例                  |
| --------    | --------                    | :--:    | ----------------------|
| MonProDisc  | 注意事項                    | string  |  汽包機66-1注意事項   |
| MonCards    | 汽機車牌卡    | list |      |

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

## BuyNow/DoAddMonth 購買月租

### [/api/BuyNow/DoAddMonth]

* 20210510發佈

* ASP.NET Web API (REST API)

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

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data資料物件說明

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
* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                         | 說明                             |
| -------- | -------------------------------- | -------------------------------- |
| ERR252   | SP執行失敗                       | SP執行失敗                       |
| ERR254   | LogID必填                        | LogID必填                        |
| ERR257   | 參數遺漏                         | 參數遺漏                         |
| ERR261   | 專案不存在                       | 專案不存在                       |
| ERR262   | 同時段只能訂閱一個汽車訂閱制月租 | 同時段只能訂閱一個汽車訂閱制月租 |
| ERR263   | 同時段只能訂閱一個機車訂閱制月租 | 同時段只能訂閱一個機車訂閱制月租 |
| ERR264   | 期數為0                          | 期數為0                          |
| ERR270   | 信用卡交易失敗                   | 信用卡交易失敗                   |
| ERR277   | 刷卡已存在                       | 刷卡已存在                       |
| ERR283   | 積分過低無法購買訂閱制           | 積分過低無法購買訂閱制           |

## BuyNow/DoUpMonth 月租升轉

### [/api/BuyNow/DoUpMonth]

* 20210510發佈

* ASP.NET Web API (REST API)

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

-  Output回傳參數說明


| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data資料物件說明

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
* 錯誤代碼

| 錯誤代碼 | 錯誤訊息             | 說明                 |
| -------- | -------------------- | -------------------- |
| ERR252   | SP執行失敗           | SP執行失敗           |
| ERR254   | LogID必填            | LogID必填            |
| ERR257   | 參數遺漏             | 參數遺漏             |
| ERR258   | 月租不存在           | 月租不存在           |
| ERR259   | 無對應目前期數月租檔 | 無對應目前期數月租檔 |
| ERR260   | 無對應待升月租檔     | 無對應待升月租檔     |
| ERR270   | 信用卡交易失敗       | 信用卡交易失敗       |
| ERR275   | 期數需相同           | 期數需相同           |
| ERR277   | 刷卡已存在           | 刷卡已存在           |
| ERR909   | 專案不存在           | 專案不存在           |
| ERR993   | 不能升轉比現在專案低的資費       | 不能升轉比現在專案低的資費       |
| ERR994   | 只能升轉同種類專案               | 只能升轉同種類專案               |

## BuyNow/DoPayArrs 月租欠費

### [/api/BuyNow/DoPayArrs]

* 20210510發佈

* ASP.NET Web API (REST API)

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

-  Output回傳參數說明


| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data資料物件說明

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

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息       | 說明           |
| -------- | -------------- | -------------- |
| ERR254   | LogID必填      | LogID必填      |
| ERR257   | 參數遺漏       | 參數遺漏       |
| ERR267   | ApiJson錯誤    | ApiJson錯誤    |
| ERR270   | 信用卡交易失敗 | 信用卡交易失敗 |
| ERR277   | 刷卡已存在     | 刷卡已存在     |
| ERR909   | 專案不存在     | 專案不存在     |
| ERR914   | 資料邏輯錯誤   | 資料邏輯錯誤   |
| ERR916   | 參數格式錯誤   | 參數格式錯誤   |


## BuyNowTool/DoAddMonth 月租購買工具

### [/api/BuyNowTool/DoAddMonth]

* 20220207發佈

* 使用情境 客人已刷卡未建訂閱制月租資料、補履保發票

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

* input 購買月租 (api/BuyNowTool/DoAddMonth) 參數說明

| 參數名稱     | 參數說明              | 必要 |  型態  | 範例 |
| ------------ | --------------------- | :--: | :----: | ---- |
| MonProjID    | 專案編號(key          |  Y   | string | MR01 |
| MonProPeriod | 期數(key)             |  Y   |  int   | 2    |
| ShortDays    | 短天期(key)           |  Y   |  int   | 0    |
| SetSubsNxt   | 設定自動續約(0否,1是) |  N   |  int   | 0    |
| MerchantTradeNo | 商店訂單編號       |  N   | string | A225668592M_20220127131822863|
| TransactionNo   | 台新訂單編號       |  N   | string | IR202201276YKVO00023 |
| MonthlyRentId   | 訂閱制編號         |  N   |  int   | 2363 |
| CreditCardFlg   | 是否付費(0:否 1是) |  N   |  int   | 0 |
| SubsDataFlg     | 是否訂閱(0:否 1是) |  N   |  int   | 0 |
| EscrowMonthFlg  | 是否履保(0:否 1是) |  N   |  int   | 0 |
| InvoiceFlg      | 是否開發票(0:否 1是) |  N   |  int   | 0 |
| IDNO            | 會員編號           |  Y   |  string  | A225668592 |
| ProdPrice       | 合約金額           |  Y   |  int     | 299 |


* input範例 (購買月租)

```
{
    "MonProjID": "MR01",
    "MonProPeriod": 2,
    "ShortDays": 0,
    "SetSubsNxt": 0,
    "MerchantTradeNo":"A225668592M_20220127131822863",
    "TransactionNo":"IR202201276YKVO00023",
    "MonthlyRentId": 2363,
    "CreditCardFlg":0,
    "SubsDataFlg":0,
    "EscrowMonthFlg" : 1,
    "InvoiceFlg" : 1,
    "IDNO":"A225668592",
    "ProdPrice":299
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
| Data         | 資料物件           | object |               |

* Data資料物件說明

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
* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                         | 說明                             |
| -------- | -------------------------------- | -------------------------------- |
| ERR247   | 參數格式不符                     | 輸入參數格式不符                 |
| ERR261   | 專案不存在                       | 專案不存在                       |
| ERR262   | 同時段只能訂閱一個汽車訂閱制月租 | 同時段只能訂閱一個汽車訂閱制月租 |
| ERR263   | 同時段只能訂閱一個機車訂閱制月租 | 同時段只能訂閱一個機車訂閱制月租 |
| ERR283   | 積分過低無法購買訂閱制           | 積分過低無法購買訂閱制           |


## BuyNowTool/DoUpMonth 月租升轉工具

### [/api/BuyNowTool/DoUpMonth]

* 20220207發佈

* 使用情境 1.客人已刷卡未建訂閱制升轉資料 2.只跑履保發票用DoAddMonth

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

* input 月租升轉 (api/BuyNowTool/DoUpMonth) 參數說明

| 參數名稱     | 參數說明              | 必要 |  型態  | 範例 |
| ------------ | --------------------- | :--: | :----: | ---- |
| IDNO            | 會員編號           |  Y   |  string  | A225668592 |
| MonProjID       | 目前專案編號(key          |  Y   | string | MR04 |
| MonProPeriod    | 目前專案期數(key)         |  Y   |  int   | 6    |
| ShortDays       | 目前專案短天期(key)       |  Y   |  int   | 0    |
| UP_MonProjID    | 升轉專案編號(key          |  Y   | string | MR05 |
| UP_MonProPeriod | 升轉期數(key)             |  Y   |  int   | 6   |
| UP_ShortDays    | 升轉短天期(key)           |  Y   |  int   | 0    |
| SetSubsNxt      | 設定自動續約(0否,1是)     |  N   |  int   | 0    |
| MerchantTradeNo | 商店訂單編號       |  N   | string | A225668592M_20220127131822863|
| TransactionNo   | 台新訂單編號       |  N   | string | IR202201276YKVO00023 |
| CreditCardFlg   | 是否付費(0:否 1是) |  N   |  int   | 0 |

* input範例 (月租升轉)

```
{
    "IDNO":"A225668592",
    "MonProjID": "MR04",
    "MonProPeriod": 6,
    "ShortDays": 0,
    "UP_MonProjID":"MR05",
    "UP_MonProPeriod":6,
    "UP_ShortDays":0,
    "SetSubsNxt": 0,
    "MerchantTradeNo":"A225668592M_20220127131822863",
    "TransactionNo":"IR202201276YKVO00023",
    "CreditCardFlg":0
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
| Data         | 資料物件           | object |               |

* Data資料物件說明

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
* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                         | 說明                             |
| -------- | -------------------------------- | -------------------------------- |
| ERR247   | 參數格式不符                     | 輸入參數格式不符                 |
| ERR261   | 專案不存在                       | 專案不存在                       |
| ERR262   | 同時段只能訂閱一個汽車訂閱制月租 | 同時段只能訂閱一個汽車訂閱制月租 |
| ERR263   | 同時段只能訂閱一個機車訂閱制月租 | 同時段只能訂閱一個機車訂閱制月租 |
| ERR283   | 積分過低無法購買訂閱制           | 積分過低無法購買訂閱制           |
| ERR909   | 專案不存在                       | 專案不存在                       |
| ERR993   | 不能升轉比現在專案低的資費       | 不能升轉比現在專案低的資費       |
| ERR994   | 只能升轉同種類專案               | 只能升轉同種類專案               |


## GetMySubs 我的方案牌卡明細

### [/api/GetMySubs/]

* 20210511發佈

* ASP.NET Web API (REST API)

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

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data資料物件說明

| 參數名稱    | 參數說明                    | 型態    | 範例                  |
| --------    | --------                    | :--:    | ----------------------|
| Month       | 資料物件 | list | |

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
| Data         | 資料物件                | obect |        　       |


* Data資料物件說明

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

## GetChgSubsList 變更下期續約列表

### [/api/GetChgSubsList]

* ASP.NET Web API (REST API)

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

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data資料物件說明

| 參數名稱      | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| MyCard       | 目前訂閱        | object |                 |
| OtrCards     | 其他方案            | list |  　  |


* MyCard (OtrCards) 資料物件說明

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

## GetUpSubsList 取得訂閱制升轉列表

### [/api/GetUpSubsList]

* ASP.NET Web API (REST API)

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

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |


* Data資料物件說明

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

## GetSubsHist 訂閱制歷史紀錄

### [/api/GetSubsHist/DoGetSubsHist]

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

  不需傳入參數

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data資料物件說明

| 參數名稱      | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| Hists       | 資料物件                  | list |  　     |

* Hists資料物件說明

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

## GetSubsHist-del 訂閱制歷史紀錄-刪除

### [/api/GetSubsHist/DoDelSubsHist]

* ASP.NET Web API (REST API)

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

| 參數名稱     | 參數說明                       |  型態  | 範例    |
| ------------ | ------------------------------ | :----: | ------- |
| Result       | 是否成功 (0:失敗 1:成功)       |  int   | 1       |
| ErrorCode    | 錯誤碼                         | string | 000000  |
| NeedRelogin  | 是否需重新登入 (0:否 1:是)     |  int   | 0       |
| NeedUpgrade  | 是否需要至商店更新 (0:否 1:是) |  int   | 0       |
| ErrorMessage | 錯誤訊息                       | string | Success |
| Data         | 資料物件                       | object |         |

* Data回傳參數說明

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

## GetArrsSubsList 訂閱制欠費查詢

### [/api/GetArrsSubsList/DoGetArrsSubsList]

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

  不須傳入參數

* Output回傳參數說明

| 參數名稱     | 參數說明                       |  型態  | 範例    |
| ------------ | ------------------------------ | :----: | ------- |
| Result       | 是否成功 (0:失敗 1:成功)       |  int   | 1       |
| ErrorCode    | 錯誤碼                         | string | 000000  |
| NeedRelogin  | 是否需重新登入 (0:否 1:是)     |  int   | 0       |
| NeedUpgrade  | 是否需要至商店更新 (0:否 1:是) |  int   | 0       |
| ErrorMessage | 錯誤訊息                       | string | Success |
| Data         | 資料物件                       | object |         |


* Data回傳參數說明

| 參數名稱      | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| TotalArresPrice | 欠費總計 | int |                 |
| Cards     | 欠費列表 | list |                 |

* Cards資料物件說明

| 參數名稱   | 參數說明         |  型態  | 範例   |
| ---------- | ---------------- | :----: | ------ |
| StartDate  | 月租起日訂單編號 | string |        |
| EndDate    | 月租迄日         | string |        |
| ProjNm     | 月租專案名稱     | string |        |
| CarTypePic | 車型對照檔名     | string | priusC |
| Arrs       | 欠費明細         |  list  |        |

* Arrs資料物件說明

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

## SetSubsNxt 設定自動續約

### [/api/SetSubsNxt]

* ASP.NET Web API (REST API)

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
| Data         | 資料物件                | object |        　       |


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

| 錯誤代碼 | 錯誤訊息                         | 說明                             |
| -------- | -------------------------------- | -------------------------------- |
| ERR247   | 參數格式不符                     | 參數格式不符                     |
| ERR254   | LogID必填                        | LogID必填                        |
| ERR257   | 參數遺漏                         | 參數遺漏                         |
| ERR258   | 月租不存在                       | 月租不存在                       |
| ERR276   | 迄日當天不可以做續訂變更         | 迄日當天不可以做續訂變更         |
| ERR909   | 專案不存在                       | 專案不存在                       |
| ERR910   | 只可為0或1                       | AutoSubs只可為0或1               |
| ERR992   | 合約迄日兩天前不可以設定自動訂閱 | 合約迄日兩天前不可以設定自動訂閱 |

# 車輛調度停車場相關

## GetMotorParkingData 取得機車調度停車場

### [/api/GetMotorParkingData/]

* 20210528新增

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

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
            }
        ]
    }
}
```

## GetParkingData 取得汽車調度停車場

### [/api/GetParkingData/]

* 20210602新增文件

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

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
            }
        ]
    }
}
```
# 推播相關

## News 活動通知

### [/api/News/]

* 20210908新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

  不須傳入參數

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data 回傳參數說明

| 參數名稱 | 參數說明 | 型態 | 範例 |
| -------- | -------- | :--: | ---- |
| NewsObj  | 活動通知 | List |      |

* 活動通知參數說明

| 參數名稱   | 參數說明       |   型態   | 範例                |
| ---------- | -------------- | :------: | ------------------- |
| NewsID     | 活動通知流水號 |   int    | 1000                |
| ActionType | 類型           |   int    | 4                   |
| PushTime   | 推播送出時間   | DateTime | 2021/09/12 12:43:52 |
| Title      | 主旨           |  string  | 9/7凌晨進行定期維護 |
| Message    | 內容           |  string  |                     |
| URL        | 外連URL        |  string  |                     |
| readFlg    | 1:已讀 0:未讀  |   int    | 1                   |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "NewsObj": [
			{
                "NewsID": 4,
                "ActionType": 0,
                "PushTime": "2021/09/12 12:43:52",
                "Title": "iRent 春節預約開始",
                "Message": "一年最期待的出遊就是過年了～\r\n準備帶一家人到中南部到哪走春？\r\n不論去哪 iRent 共享汽車全台各地都載你去~~\r\n同站租還全台各縣市皆有站點\r\n去南台灣曬太陽真的很不錯",
                "URL": "",
                "readFlg": 0
            },
            {
                "NewsID": 7,
                "ActionType": 3,
                "PushTime": "2021/09/12 12:43:52",
                "Title": "adam test",
                "Message": "推播測試123",
                "URL": "",
                "readFlg": 1
            }
		]
    }
}

```

## NewsRead 活動通知讀取

### [/api/NewsRead/]

* 20210908新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明       | 必要 | 型態 | 範例 |
| -------- | -------------- | :--: | :--: | ---- |
| NewsID   | 活動通知流水號 |  Y   | int  | 7    |

* input範例

```
{
    "NewsID": "7"
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
| Data         | 資料物件           | object |               |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": { }
}
```

## PersonNotice 個人訊息

### [/api/PersonNotice/]

* 20210908新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

  不須傳入參數

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data 回傳參數說明

| 參數名稱        | 參數說明 | 型態 | 範例 |
| --------------- | -------- | :--: | ---- |
| PersonNoticeObj | 個人訊息 | list |      |

* 個人訊息參數說明

| 參數名稱       | 參數說明       |   型態   | 範例                |
| -------------- | -------------- | :------: | ------------------- |
| NotificationID | 個人訊息流水號 |   int    | 1000                |
| PushTime       | 推播時間       | DateTime | 2021/09/01 01:33:34 |
| Title          | 主旨           |  string  | 9/7凌晨進行定期維護 |
| Message        | 內容           |  string  | 測試推播123         |
| URL            | 連結URL        |  string  |                     |
| readFlg        | 1:已讀 0:未讀  |   int    | 1                   |


* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "PersonNoticeObj": [
			{
                "NotificationID": 1320,
                "PushTime": "2021/09/01 01:33:34",
                "Title": "[PA]TEST20210831",
                "Message": "測試推播123",
                "URL": "",
                "readFlg": 1
            },
			{
                "NotificationID": 1336,
                "PushTime": "2021/09/12 23:18:18",
                "Title": "【共同承租】XXX邀請您共同承租唷！",
                "Message": "",
                "URL": "http://apyhfc07:8002/irent/Sharing/Togetherpassenger/invitingStatus?KtwBd%2fMrrFcEG2SMxUTqQz%2faYuMtvxDRc92igY7iR3kbhKkT20CdeHtTQY3oPJMm",
                "readFlg": 0
            }
		]
    }
}
```

## PersonNoticeRead 個人訊息讀取

### [/api/PersonNoticeRead/]

* 20210908新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱       | 參數說明       | 必要 | 型態 | 範例 |
| -------------- | -------------- | :--: | :--: | ---- |
| NotificationID | 個人訊息流水號 |  Y   | int  | 7    |

* input範例

```
{
    "NotificationID": "1320"
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
| Data         | 資料物件           | object |               |


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

## TestPushService 測試推播工具

### [/api/TestPushService/]

* 20210915新增文件

* ASP.NET Web API (REST API)

* api位置

  這個不會放在正式環境
  測試環境：https://irentcar-app-test.azurefd.net

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例                    |
| --------- | ---------- | :--: | :----: | ----------------------- |
| PushRegID | 推播註冊碼 |  Y   |  int   | 2772                    |
| Title     | 訊息抬頭   |      | string | 推播測試                |
| Message   | 訊息內容   |      | string | 推播測試                |
| UserName  | 客戶名稱   |      | string | 測試人員                |
| Url       | 連結的URL  |      | string | https://www.google.com/ |


* input範例

```
{
    "PushRegID":2747,
    "Title":"推播測試",
    "Message":"推播測試",
    "UserName":"A等鴿",
    "Url":""
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
| Data         | 資料物件           | object |               |


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

## NoticeRead 推播通知讀取

### [/api/NoticeRead/]

* 20210922新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* 說明 此API主要提供寫入推播通知的紀錄，每寫入一次效力一個小時

* input傳入參數說明

  不須傳入參數

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |


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

# 共同承租人機制

## JointRentInviteeListQuery 共同承租人邀請清單查詢 
### [/api/JointRentInviteeListQuery/]

* 20210819新增

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |

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
| Data         | 資料物件           | object |               |

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

| 錯誤代碼 | 錯誤訊息                                   | 說明                                       |
| -------- | ------------------------------------------ | ------------------------------------------ |
| ERR168   | 找不到符合的訂單                           | 取消訂單時找不到可取消的訂單               |
| ERR919   | 對方不能租車，請對方確認會員狀態哦！       | 對方不能租車，請對方確認會員狀態哦！       |
| ERR921   | 已至邀請人數上限，請手動移除非邀請對象哦！ | 已至邀請人數上限，請手動移除非邀請對象哦！ |
| ERR936   | 格式不符，請重新輸入哦！                   | 輸入格式不符                               |

## JointRentInvitation 案件共同承租人邀請

### [/api/JointRentInvitation/]

* 20210819新增

* ASP.NET Web API (REST API)

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
| Data         | 資料物件           | object |               |


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
| Data         | 資料物件           | object |               |


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

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明   | 必要 |  型態  | 範例        |
| --------- | ---------- | :--: | :----: | ----------- |
| AESEncryptString | AES加密參數 | Y | string | 如下 |


* input範例

```
{
    "AESEncryptString": "KtwBd/MrrFcEG2SMxUTqQ0t1FcXedyF/dtqaNMyHAztEtOIrdP6MIbG/zR6l6ymy"
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
| Data         | 資料物件           | object |               |


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

# 電子錢包相關

## CreditAndWalletQuery 查詢綁卡跟錢包

### [/api/CreditAndWalletQuery/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | :--: | :--: | ---- |
| 無參數   |          |      |      |      |

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |

* Data 回傳參數說明

| 參數名稱     | 參數說明                                                     |  型態  | 範例     |
| ------------ | ------------------------------------------------------------ | :----: | -------- |
| PayMode      | 付費方式 (0:信用卡 1:和雲錢包 4:Hotaipay)                    |  int   | 0        |
| HasBind      | 是否有綁定(0:無,1有)                                         |  int   | 1        |
| HasWallet    | 是否有錢包(0:無,1有)                                         |  int   | 0        |
| TotalAmount  | 錢包剩餘金額                                                 |  int   | 0        |
| BindListObj  | 信用卡列表                                                   |  list  |          |
| MEMSENDCD    | 發票寄送方式<br>1:捐贈<br>2:email<br>3:二聯<br>4:三聯<br>5:手機條碼<br>6:自然人憑證 |  int   | 5        |
| UNIMNO       | 統編                                                         | string |          |
| CARRIERID    | 手機條碼                                                     | string | /N37H2JD |
| NPOBAN       | 愛心碼                                                       | string |          |
| AutoStored   | 是否同意自動儲值 (0:不同意 1:同意)                           |  int   | 0        |
| HasHotaiPay  | 是否有和泰PAY(0:無,1有)                                      |  int   | 0        |
| HotaiListObj | 和泰PAY卡清單                                                |  list  |          |
| MotorPreAmt  | 機車預扣款金額                                               |  int   | 50       |

* BindListObj 回傳參數說明

| 參數名稱        | 參數說明                         |  型態  | 範例                                                  |
| --------------- | -------------------------------- | :----: | ----------------------------------------------------- |
| BankNo          | 銀行帳號                         | string |                                                       |
| CardNumber      | 信用卡卡號                       | string | 432102******1234                                      |
| CardName        | 信用卡自訂名稱                   | string | 商業銀行                                              |
| AvailableAmount | 剩餘額度                         | string |                                                       |
| CardToken       | 替代性信用卡卡號或替代表銀行卡號 | string | db59abcd-1234-1qaz-2wsx-3edc4rfv5tgb_3214567890123456 |

* HotaiListObj 回傳參數說明

| 參數名稱    | 參數說明                  |  型態  | 範例                                 |
| ----------- | ------------------------- | :----: | ------------------------------------ |
| MemberOneID | 和泰會員OneID             | string | 0064fb4f-8250-4690-954b-2ba94862606b |
| CardToken   | 信用卡Token               | string | 1385                                 |
| CardName    | 信用卡自訂名稱            | string |                                      |
| CardType    | 發卡機構(VISA/MASTER/JCB) | string | Visa                                 |
| BankDesc    | 發卡銀行                  | string | 國外卡                               |
| CardNumber  | 卡號(隱碼)                | string | ****-****-****-5278                  |
| IsDefault   | 是否為預設卡(0:否/1:是)   |  int   | 1                                    |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "PayMode": 0,
        "HasBind": 1,
        "HasWallet": 0,
        "TotalAmount": 0,
        "BindListObj": [
            {
                "BankNo": "",
                "CardNumber": "432102******1234",
                "CardName": "商業銀行",
                "AvailableAmount": "",
                "CardToken": "db59abcd-1234-1qaz-2wsx-3edc4rfv5tgb_3214567890123456"
            }
        ],
        "MEMSENDCD": 5,
        "UNIMNO": "",
        "CARRIERID": "/N37H2JD",
        "NPOBAN": "",
        "AutoStored": 0,
        "HasHotaiPay": 1,
        "HotaiListObj": [
            {
                "MemberOneID": "0064fb4f-8250-4690-954b-2ba94862606b",
                "CardToken": "1385",
                "CardName": "",
                "CardType": "Visa",
                "BankDesc": "國外卡",
                "CardNumber": "****-****-****-5278",
                "IsDefault": 1
            }
        ],
        "MotorPreAmt": 50
    }
}
```

## WalletStoreTradeTransHistory 錢包歷史紀錄查詢

### [/api/WalletStoreTradeTransHistory/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |   型態   | 範例             |
| -------- | -------- | :--: | :------: | ---------------- |
| SD       | 查詢起日 |  Y   | DateTime | 2021-08-01 00:00 |
| ED       | 查詢迄日 |  Y   | DateTime | 2021-08-30 00:00 |

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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱 | 參數說明     | 型態 | 範例 |
| -------- | ------------ | :--: | ---- |
| TradeHis | 錢包歷史紀錄 | List |      |

* TradeHis 回傳參數說明

| 參數名稱    | 參數說明      |  型態  | 範例          |
| ----------- | ------------- | :----: | ------------- |
| SEQNO       | 帳款流水號    |  int   | 1             |
| TradeYear   | 交易年分      |  int   | 2021          |
| TradeDate   | 交易日期      | string | 08/17         |
| TradeTime   | 交易時間      | string | 20:05         |
| TradeTypeNm | 交易類別      | string | 錢包提領      |
| TradeNote   | 交易類別註記  | string | 電子錢包提領  |
| TradeAMT    | 交易金額      |  int   | 1000          |
| ShowFLG     | APP上是否顯示 |  int   | 0:隱藏,1:顯示 |

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
                "SEQNO": 9,
                "TaishinNO": "IR2021051736452153XX",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "20:05",
                "TradeTypeNm": "錢包提領",
                "TradeNote": "電子錢包提領",
                "TradeAMT": -6000,
                "ShowFLG": 1
            },
            {
                "SEQNO": 8,
                "TaishinNO": "IR2021051736452153XX",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "錢包付款",
                "TradeNote": "H11216402",
                "TradeAMT": -800,
                "ShowFLG": 1
            },
            {
                "SEQNO": 7,
                "TaishinNO": "IR2021051736452153XX",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "錢包付款",
                "TradeNote": "訂閱制",
                "TradeAMT": -2999,
                "ShowFLG": 1
            },
            {
                "SEQNO": 6,
                "TaishinNO": "IR2021051736452153XX",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "轉贈",
                "TradeNote": "轉贈人 陳O安",
                "TradeAMT": 2000,
                "ShowFLG": 1
            },
            {
                "SEQNO": 5,
                "TaishinNO": "IR2021051736452153XX",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "合約退款",
                "TradeNote": "合約退款",
                "TradeAMT": 500,
                "ShowFLG": 1
            },
            {
                "SEQNO": 4,
                "TaishinNO": "IR2021051736452153XX",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "錢包加值(虛擬帳號轉帳)",
                "TradeNote": "虛擬帳號轉帳",
                "TradeAMT": 3000,
                "ShowFLG": 1
            },
            {
                "SEQNO": 3,
                "TaishinNO": "IR2021051736452153XX",
                "TradeYear": 2021,
                "TradeDate": "08/17",
                "TradeTime": "00:05",
                "TradeTypeNm": "錢包加值(超商繳款)",
                "TradeNote": "超商繳款",
                "TradeAMT": 8000,
                "ShowFLG": 1
            },
            {
                "SEQNO": 2,
                "TaishinNO": "IR2021051736452153XX",
                "TradeYear": 2021,
                "TradeDate": "08/13",
                "TradeTime": "00:00",
                "TradeTypeNm": "錢包付款",
                "TradeNote": "H11845561",
                "TradeAMT": -1000,
                "ShowFLG": 1
            },
            {
                "SEQNO": 1,
                "TaishinNO": "IR2021051736452153XX",
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

## WalletStoreTradeHistoryHidden 錢包歷程-儲值交易紀錄隱藏

### [/api/WalletStoreTradeHistoryHidden/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明           | 必要 | 型態 | 範例 |
| -------- | ------------------ | ---- | :--: | ---- |
| SEQNO    | 帳款流水號(by會員) |      | int  | 1    |


* input範例

```
{
  "SEQNO" : 1
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
| Data         | 資料物件           | object |               |

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

## GetWalletStoredMoneySet 錢包儲值-設定資訊

### [/api/GetWalletStoredMoneySet/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱  | 參數說明                                           | 必要 | 型態 | 範例 |
| --------- | -------------------------------------------------- | ---- | :--: | ---- |
| StoreType | 儲值方式(0:信用卡,2:虛擬帳號,3:超商繳費,4:和泰PAY) |      | int  | 3    |

* input範例

```
{
  "StoreType" : 3
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
| Data         | 資料物件           | object |               |

* Data回傳參數說明

| 參數名稱       | 參數說明     | 型態 | 範例 |
| -------------- | ------------ | :--: | ---- |
| StoredMoneySet | 儲值設定資訊 | List |      |

* StoredMoneySet 參數說明

| 參數名稱        | 參數說明                                           |   型態    | 範例   |
| --------------- | -------------------------------------------------- | :-------: | ------ |
| StoreType       | 儲值方式(0:信用卡,2:虛擬帳號,3:超商繳費,4:和泰PAY) |    int    | 3      |
| StoreTypeDetail | 明細方式(全家:family,7-11:seven)                   |  string   | family |
| WalletBalance   | 錢包餘額                                           |    int    | 100    |
| Rechargeable    | 尚可儲值                                           |    int    | 49900  |
| StoreLimit      | 單次儲值最低                                       |    int    | 100    |
| StoreMax        | 單次儲值最高                                       |    int    | 50000  |
| QuickBtns       | 快速按鈕                                           | List<int> |        |
| defSet          | 預設選取(1是0否)                                   |    int    |        |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	"StoredMoneySet":[
	   {
	     "StoreType":3,
		 "StoreTypeDetail": "family",
	     "WalletBalance": 100,
		 "Rechargeable": 20000,
		 "StoreLimit":300,
		 "StoreMax": 20000,
		 "QuickBtns": [300,1000,5000],
		 "defSet": 1
	   },
	   {
	     "StoreType":3,
		 "StoreTypeDetail": "seven",
	     "WalletBalance": 100,
		 "Rechargeable": 20000,
		 "StoreLimit":300,
		 "StoreMax": 20000,
		 "QuickBtns": [300,1000,5000],
		 "defSet": 0
	   }]	   
	}
}
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
	"StoredMoneySet":[
	   {
	     "StoreType":2,
		 "StoreTypeDetail": "",
	     "WalletBalance": 100,
		 "Rechargeable": 49900,
		 "StoreLimit": 100,
		 "StoreMax": 50000,
		 "QuickBtns": [100,1000,5000],
		 "defSet": 1
	   },  
	}
}
```

## WalletStoredByCredit 錢包儲值-信用卡

### [/api/WalletStoredByCredit/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                     | 必要 | 型態 | 範例 |
| ---------- | ---------------------------- | ---- | :--: | ---- |
| StoreMoney | 儲值金額                     | Y    | int  | 100  |
| StoreType  | 儲值方式(0:信用卡,4:和泰PAY) | Y    | int  | 4    |

* input範例

```
{
  "StoreMoney" : 100,
  "StoreType":4
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
| Data         | 資料物件           | object |               |

* Data 回傳參數說明

| 參數名稱   | 參數說明 | 型態 | 範例 |
| ---------- | -------- | :--: | ---- |
| StoreMoney | 儲值金額 | int  | 100  |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "StoreMoney": 100
    }
}
```

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                   | 說明                                     |
| -------- | -------------------------- | ---------------------------------------- |
| ERR730   | 查詢綁定卡號失敗           | 查詢綁定卡號失敗                         |
| ERR197   | 刷卡授權失敗，請洽發卡銀行 | 刷卡授權發生錯誤                         |
| ERR284   | 儲值金額不得低於下限       | 儲值金額不得低於100元                    |
| ERR285   | 儲值金額超過上限           | 儲值金額不得大於5萬元                    |
| ERR282   | 錢包金額超過上限           | 錢包現存餘額上限為5萬元(包括受贈)        |
| ERR280   | 金流超過上限               | 錢包單月交易及使用(包括轉贈)上限為30萬元 |
| ERR918   | Api呼叫失敗                | 呼叫台新錢包儲值發生失敗                 |
| ERR286   | 寫入錢包紀錄發生失敗       | 寫入錢包紀錄發生失敗                     |

## WalletStoreVisualAccount 錢包儲值-虛擬帳號

### [/api/WalletStoreVisualAccount/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明 | 必要 | 型態 | 範例 |
| ---------- | -------- | ---- | :--: | ---- |
| StoreMoney | 儲值金額 | Y    | int  | 456  |

* input範例

```
{
  "StoreMoney" : 456
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
| Data         | 資料物件           | object |               |

* Data 回傳參數說明

| 參數名稱       | 參數說明              |  型態  | 範例                |
| -------------- | --------------------- | :----: | ------------------- |
| StoreMoney     | 儲值金額              |  int   | 456                 |
| PayDeadline    | 繳費期限(距今+3日)    | string | 2021/10/18 23:59    |
| BankCode       | 銀行代碼              | string | 812                 |
| VirtualAccount | 轉入虛擬帳號          | string | 8552 0028 6660 2912 |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "PayDeadline": "2021/10/18 23:59",
        "BankCode": "812",
        "VirtualAccount": "8552 0028 6660 2912",
        "StoreMoney": 456
    }
}

{
    "Result": "0",
    "ErrorCode": "ERR284",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "儲值金額不得低於下限",
    "Data": {}
}
```

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                         | 說明                                     |
| -------- | -------------------------------- | ---------------------------------------- |
| ERR284   | 儲值金額不得低於下限             | 儲值金額不得低於100元                    |
| ERR282   | 錢包金額超過上限                 | 錢包現存餘額上限為5萬元(包括受贈)        |
| ERR280   | 金流超過上限                     | 錢包單月交易及使用(包括轉贈)上限為30萬元 |
| ERR937   | 虛擬帳號產生失敗，請洽系統管理員 | 虛擬帳號產生失敗                         |
| ERR918   | Api呼叫失敗                      | 發生Exception，請洽系統管理員            |

## WalletStoreShop 錢包儲值-商店條碼

### [/api/WalletStoreShop/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                | 必要 | 型態 | 範例 |
| ---------- | ----------------------- | ---- | :--: | ---- |
| StoreMoney | 儲值金額                | Y    | int  | 500  |
| CvsType    | 超商類型(0:7-11 1:全家) | Y    | int  | 1    |

* input範例

```
{
  "StoreMoney" : 500,
  "CvsType":1
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
| Data         | 資料物件           | object |               |

* Data 回傳參數說明

| 參數名稱     | 參數說明                                                     |  型態  | 範例             |
| ------------ | ------------------------------------------------------------ | :----: | ---------------- |
| StoreMoney   | 儲值金額                                                     |  int   | 500              |
| PayDeadline  | 繳費期限(距今+3H)                                            | string | 2021/10/14 19:36 |
| ShopBarCode1 | 超商條碼1                                                    | string | 101014K9A        |
| ShopBarCode2 | 超商條碼2                                                    | string | IRF0000000000015 |
| ShopBarCode3 | 超商條碼3                                                    | string | 235916000000500  |
| Barcode64    | BASE64 ENCODE 字串 (尺寸：320 X 480，由前端轉為PNG 格式等比顯示) | string |                  |

[^註]: 7-11只會回傳Barcode64 ENCODE 字串

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "PayDeadline": "2021/10/14 19:36",
        "ShopBarCode1": "101014K9A",
        "ShopBarCode2": "IRF0000000000015",
        "ShopBarCode3": "235916000000500",
        "Barcode64": "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",
        "StoreMoney": 500
    }
}

{
    "Result": "0",
    "ErrorCode": "ERR940",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "超商條碼產生失敗，請洽系統管理員",
    "Data": {}
}
```

* 錯誤代碼

| 錯誤代碼 | 錯誤訊息                         | 說明                                     |
| -------- | -------------------------------- | ---------------------------------------- |
| ERR284   | 儲值金額不得低於下限             | 儲值金額不得低於300元                    |
| ERR282   | 錢包金額超過上限                 | 錢包現存餘額上限為5萬元(包括受贈)        |
| ERR280   | 金流超過上限                     | 錢包單月交易及使用(包括轉贈)上限為30萬元 |
| ERR940   | 超商條碼產生失敗，請洽系統管理員 | 超商條碼產生失敗                         |
| ERR918   | Api呼叫失敗                      | 發生Exception，請洽系統管理員            |

## GetPayInfo 還車付款-取得付款方式

### [/api/GetPayInfo/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 | 型態 | 範例 |
| -------- | -------- | ---- | :--: | ---- |
| 無參數   |          |      |      |      |

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |


* Data 回傳參數說明

| 參數名稱         | 參數說明                      | 型態 | 範例 |
| ---------------- | ----------------------------- | :--: | ---- |
| DefPayMode       | 預設付款方式(0:信用卡 1:錢包 4:Hotaipay) | int  | 0    |
| PayModeBindCount | 已經綁定的付費方式數量        | int  | 1    |
| PayModeList      | 付款方式清單                  | List |      |

* PayModeList 回傳參數說明

| 參數名稱      | 參數說明                      |  型態  | 範例                                     |
| ------------- | ----------------------------- | :----: | ---------------------------------------- |
| PayMode       | 付款方式(0:信用卡 1:錢包 4:Hotaipay)     |  int   | 0                                        |
| PayModeName   | 付款方式名稱                  | string | 信用卡                                   |
| HasBind       | 是否有綁定過開通(0:否1:是)    |  int   | 1                                        |
| PayInfo       | 付款顯示資訊                  | string | *1234                                    |
| Balance       | 餘額                          |  int   | 0                                        |
| AutoStoreFlag | 是否自動儲值 (0:否1:是)       |  int   | 0                                        |
| NotBindMsg    | 未綁定時顯示的文字 (0:否1:是) | string | 若該支付方式有綁定或開通則該欄位為空字串 |

* Output範例

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
	"Data": {
        "DefPayMode": 0,
        "PayModeBindCount":1,
        "PayModeList":
        [
            {
                "PayMode":0,
                "PayModeName":"信用卡"
                "HasBind":"1"
                "PayInfo":"414763******1234"
                "Balance":0
                "AutoStoreFlag":0
                "NotBindMsg":""
            },
            {
                "PayMode":1,
                "PayModeName":"和雲錢包"
                "HasBind":0
                "PayInfo":""
                "Balance":0
                "AutoStoreFlag":0
                "NotBindMsg":"未開通"
			}
        ]   
	}
}
```

## WalletTransferStoredValue 錢包轉贈

### [/api/WalletTransferStoredValue/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                  | 必要 |  型態  | 範例       |
| ---------- | ------------------------- | ---- | :----: | ---------- |
| IDNO       | 身分證(從轉贈對象確認取得)| Y    | string | A123456789 |
| Amount     | 轉贈金額                  | Y    |  int   | 1000       |

* input範例

```
{
  "IDNO" : "A123456789", 
  "Amount" : 1000
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
| Data         | 資料物件           | object |               |

* Data 回傳參數說明

| 參數名稱   | 參數說明              |  型態  | 範例             |
| ---------- | --------------------- | :----: | ---------------- |


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
{
    "Result": "0",
    "ErrorCode": "ERR281",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "轉贈金額超過錢包金額",
    "Data": {
    }
}
```

## WalletTransferCheck 轉贈對象確認

### [/api/WalletTransferCheck/]

* 20210831新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明         | 必要 |  型態  | 範例       |
| ---------- | ---------------- | ---- | :----: | ---------- |
| IDNO_Phone | 身分證或手機號碼 | Y    | string | A123456789 |

* input範例

```
{
  "IDNO_Phone" : "A123456789"
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
| Data         | 資料物件           | object |               |

* Data 回傳參數說明

| 參數名稱  | 參數說明           |  型態  | 範例          |
| --------- | ------------------ | :----: | ------------- |
| TranCheck | 可否轉贈(1:可轉贈) |  int   | 1             |
| IDNO      | 會員身分證字號     | string | A123456789    |
| ShowName  | 遮罩後會員姓名     | string | 李Ｏ瑄        |
| ShowValue | 遮罩後的查詢Key值  | string | 0987\*\*\*321 |

* Output範例

```
{
    "Result": "0",
    "ErrorCode": "ERR278",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "非一般會員",
    "Data": {
        "CkResult": 0,
        "ShowName": "",
        "ShowValue": "",
        "IDNO": ""
    }
}
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "CkResult": 1,
        "ShowName": "劉O希",
        "ShowValue": "F130XXX853",
        "IDNO": "F130140853"
    }
}
```

## SetDefPayMode 設定預設支付方式

### [/api/SetDefPayMode/]

* 20210928新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明                           | 必要 | 型態 | 範例 |
| -------- | ---------------------------------- | ---- | :--: | ---- |
| PayMode  | 支付方式<br>0:信用卡<br>1:和雲錢包<br>4:Hotaipay | Y    | int  | 0    |

* input範例

```
{
    "PayMode": 0
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
| Data         | 資料物件           | object |               |


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

## AutoStoreSetting 自動儲值設定

### [/api/AutoStoreSetting/]

* 20210929新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                            | 必要 | 型態 | 範例 |
| ---------- | ----------------------------------- | ---- | :--: | ---- |
| AutoStored | 是否同意自動儲值<br>0:不同意 1:同意 | Y    | int  | 0    |

* input範例

```
{
    "AutoStored": 0
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
| Data         | 資料物件           | object |               |

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


## WalletWithdrowInvoice 寫入手續費發票

### [/api/WalletWithdrowInvoice/]

* 20210819新增文件

* ASP.NET Web API (REST API)

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(非必填)**

* 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明       | 必要 |  型態  | 範例                           |
| -------- | -------------- | ---- | :----: | ------------------------------ |
| NORDNO   | 對應主檔流水號 | Y    | String | P00800HiMS20211001140619666113 |
| INV_NO   | 發票號碼       | Y    | String | DE12345678                     |
| INV_DATE | 發票開立日期   | Y    | String | 20211010                       |
| RNDCODE  | 發票隨機碼     | Y    | String | 0594                           |

* Output回傳參數說明

| 參數名稱     | 參數說明           |  型態  | 範例          |
| ------------ | ------------------ | :----: | ------------- |
| Result       | 是否成功           |  int   | 0:失敗 1:成功 |
| ErrorCode    | 錯誤碼             | string | 000000        |
| NeedRelogin  | 是否需重新登入     |  int   | 0:否 1:是     |
| NeedUpgrade  | 是否需要至商店更新 |  int   | 0:否 1:是     |
| ErrorMessage | 錯誤訊息           | string | Success       |
| Data         | 資料物件           | object |               |


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

