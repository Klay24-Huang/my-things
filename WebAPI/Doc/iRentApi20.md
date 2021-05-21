版本: 1.0

# iRentApi2 WebAPI

iRentApi20 Web API版本

目錄

登入相關

- [Login 登入](#Login)
- [RefrashToken 更新Token](#RefrashToken)
- [CheckAppVersion 檢查APP版本](#CheckAppVersion)
- [GetMemberStatus 取得會員狀態](#GetMemberStatus)

會員相關

- [GetMemberScore 取得會員積分](#GetMemberScore)

首頁地圖相關

- [GetFavoriteStation取得常用站點](#GetFavoriteStation)
- [SetFavoriteStation設定常用站點](#SetFavoriteStation)
- [GetCarType同站以據點取出車型](#GetCarType)
- [GetProject取得專案與資費](#GetProject)
- [GetBanner 取得廣告資訊](#GetBanner)
- [GetNormalRent 取得同站租還站點](#GetNormalRent)
- [GetCarTypeGroupList取得車型清單](#GetCarTypeGroupList)

取還車跟車機操控相關
- [ChangeUUCard 變更悠遊卡](#ChangeUUCard)

月租訂閱制相關
- [GetMonthList   取得訂閱制月租列表](#GetMonthList)  
- [GetMonthGroup  訂閱制月租專案群組](#GetMonthGroup)  
- [BuyNow 立即購買](#BuyNow)
- [GetMySubs我的方案牌卡明細](#GetMySubs)
- [GetSubsCNT取得合約明細](#GetSubsCNT)
- [GetChgSubsList 變更下期續約列表](#GetChgSubsList)
- [GetUpSubsList 訂閱制升轉列表](#GetUpSubsList)
- [GetSubsHist 訂閱制歷史紀錄](#GetSubsHist)
- [GetSubsHist-del 訂閱制歷史紀錄-刪除](#GetSubsHist-del)
- [GetArrsSubsList 訂閱制欠費查詢](#GetArrsSubsList)

訂單相關
- [OrderDetail 歷史訂單明細](#OrderDetail)

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



# 登入相關

##  Login 登入 

### [/api/Login/]

* 20210322發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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
| BLOCK_EDATE     | 停權截止日                                                   | string |            |

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
            "BLOCK_EDATE": ""
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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

| 參數名稱   | 參數說明     |   型態   | 範例                    |
| ---------- | ------------ | :------: | ----------------------- |
| TotalCount | 總筆數       |   int    | 61                      |
| RowNo      | 編號         |   int    | 1                       |
| GetDate    | 取得日期     | DateTime | 2021-05-19T13:37:03.733 |
| SEQ        | 序號         |   int    | 103                     |
| SCORE      | 分數         |   int    | -50                     |
| UIDESC     | 用戶畫面敘述 |  string  | 天佑台灣                |

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
        "TotalPage": 7,
        "DetailList": [
            {
                "TotalCount": 61,
                "RowNo": 1,
                "GetDate": "2021-05-19T13:37:03.733",
                "SEQ": 103,
                "SCORE": -50,
                "UIDESC": "天佑台灣"
            },
            {
                "TotalCount": 61,
                "RowNo": 2,
                "GetDate": "2021-05-10T11:00:00",
                "SEQ": 101,
                "SCORE": 1,
                "UIDESC": "單次租用"
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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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



# 首頁地圖相關

## GetFavoriteStation取得常用站點
### [/api/GetFavoriteStation/]

* 20210315發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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


## GetProject取得專案與資費
### [/api/GetProject/]

* 20210315修改 - 增加是否為常用據點欄位
* 20210324修改 - 增加搜尋欄位 
* 20210407修改 - input移除掉Seats 

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

* 動作 [POST]
  
* input傳入參數說明

| 參數名稱  | 參數說明 | 必要 |  型態  | 範例        |
| --------- | -------- | :--: | :----: | ----------- |
| StationID | 據點代碼 | Y | string | X0II |
| ☆CarTypes | 車型代碼 | N | array string | [ "PRIUSC" ] |


* Seats 回傳參數說明

| 參數名稱 | 參數說明     |  型態  | 範例 |
| -------- | ------------ | :----: | ---- |
| SDate | 預計取車時間 | Y | string | 2021-03-12 14:00:00 |
| EDate | 預計還車時間 | Y | string | 2021-03-13 15:00:00 |
| Mode | 顯示方式 | Y | int | 0:依據點代碼 1:依經緯度 |
| Latitude | 緯度 |  | float | |
| Longitude | 經度 | | float | |
| Radius | 半徑 | | float | 0 |
| Insurance | 是否使用安心服務 | Y | int | 0:否 1:是 |

* input範例
```
{
    "CarTypes": [ "PRIUSC" ],
    "StationID": "X0II",
    "SDate": "2021-03-12 14:00:00",
    "Latitude": "",
    "Radius": "0",
    "EDate": "2021-03-13 15:00:00",
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
                "ProjectObj": [
                    {
                        "StationID": "X0II",
                        "ProjID": "P735",
                        "ProjName": "同站汽車99推廣專案",
                        "ProDesc": "1.本專案限iRent會員使用。  \r\n2.本專案活動期間為即日起~2021/3/31。  \r\n3.本專案優惠價：平日時租99元，日租990元；假日時租168元或198元，日租1,680元或1,980元(各車款價格詳見「關於iRent─租車費率」)。還車時依實際使用情形再收取里程費及eTag過路費。  \r\n4.平日定義: 週一至週五(不含國定假日)。  \r\n5.本專案為同站租還服務，還車需將車輛停放回原站點。  \r\n6.預約租車時請選擇適合之預計取車時間，預約訂單成立後每台車將保留15分鐘。若超過預計取車時間15分鐘仍未取車，系統將自動取消該筆預約。  \r\n7.本專案不得累計網路會員e-bonus，且不得折抵消費金額。  \r\n8.平日優惠價迄日為2021/3/31，非平日優惠價期間以推廣價計算租金",
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
                        "Price": 1774,
                        "WorkdayPerHour": 99,
                        "HolidayPerHour": 168,
                        "CarOfArea": "同站",
                        "Content": "",
                        "IsRent": "Y"
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



## GetBanner 取得廣告資訊

### [/api/GetBanner/]

* 20210316發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

# 取還車跟車機操控相關



## ChangeUUCard 變更悠遊卡

### [/api/ChangeUUCard/]

* 20210415發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

# 月租訂閱制相關


## GetMonthList 取得訂閱制月租列表/我的所有方案

###  [/api/GetMonthList/]

* 20210510發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

* MyCar, MyMoto 參數說明

  | 參數名稱  | 參數說明       | 型態   | 範例                                   |
  | --------- | -------------- | ------ | -------------------------------------- |
  | 其餘參數  | 同NorMonCards  |        | 參考NorMonCards, MixMonCards  參數說明 |
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
				"IsPay": 0
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
				"IsPay": 0
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
				"IsPay": 0
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
				"IsPay": 0
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
				"IsPay": 0
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
				"IsPay": 0
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
				"IsPay": 0
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
				"IsPay": 0
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
				"IsPay": 0
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
                "IsPay": 0
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
                "IsPay": 0
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
                "IsPay": 0
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
                "IsPay": 0
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
                "IsPay": 0
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
                "IsPay": 0
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
                "IsPay": 0
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
            "IsPay": 1
        },
        "MyMoto": {
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
            "IsPay": 1
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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                 | 必要 |  型態   | 範例                              |
| ---------- | ------------------------ | :--: | :----:  | ----------------------------------|
| MonProjID  | 專案代碼                 |  Y   |  string | MR66                              |

* input範例

```
{
    "MonProjID": "MR66",
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
                "IsPay": 0
            }
        ]
    }
}
```

------



## BuyNow 立即購買
### [/api/BuyNow/]

* 20210510發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                   | 必要 |  型態  | 範例                           |
| ---------- | -------------------------- | :--: | :----: | ------------------------------ |
| ApiID      | 呼叫端apiId                |  Y   |  int   | 179                            |
| ApiJson    | 呼叫端ApiJson-序列化後字串 |  N   | string | 請參考ApiJson 序列化後字串範例 |
| ProdNm     | 產品名稱                   |  N   | string | 測試_汽包機66-3                |
| ProdDisc   | 產品描述                   |  N   | string | 測試                           |
| ProdPrice  | 產品價格                   |  Y   |  int   | 7000                           |
| DoPay      | 執行付款(0顯示,1付款)      |  Y   |  int   | 0                              |
| PayTypeId  | 選定付款方式               |  N   |  int   | 5                              |
| InvoTypeId | 選定發票設定               |  N   |  int   | 6                              |

* ApiJson(ApiID=179 / 購買月租)參數說明

| 參數名稱     | 參數說明              | 必要 |  型態  | 範例 |
| ------------ | --------------------- | :--: | :----: | ---- |
| MonProjID    | 專案編號(key          |  Y   | string | MR66 |
| MonProPeriod | 期數(key)             |  Y   |  int   | 3    |
| ShortDays    | 短天期(key)           |  Y   |  int   | 0    |
| SetSubsNxt   | 設定自動續約(0否,1是) |  N   |  int   | 0    |

* ApiJson(ApiID=188 /月租升轉)參數說明

  | 參數名稱        | 參數說明          | 必要 | 型態   | 範例  |
  | --------------- | ----------------- | ---- | ------ | ----- |
  | MonProjID       | 專案編號(key)     | Y    | string | MR66  |
  | MonProPeriod    | 期數(key)         | Y    | int    | 3     |
  | ShortDays       | 短天期(key)       | Y    | int    | 0     |
  | UP_MonProjID    | 升轉專案編號(key) | Y    | string | MR102 |
  | UP_MonProPeriod | 升轉期數(key)     | Y    | int    | 3     |
  | UP_ShortDays    | 升轉短天期(key)   | Y    | int    | 0     |
  | PayTypeId       | 付款方式          | Y    | int    | 5     |
  | InvoTypeId      | 發票方式          | Y    | int    | 6     |

* ApiJson(ApiID=190 /月租欠費繳交)參數說明

  | 參數名稱       | 參數說明                  | 必要 | 型態   | 範例    |
  | -------------- | ------------------------- | ---- | ------ | ------- |
  | MonthlyRentIds | 月租Id(多筆以逗號","分隔) | Y    | string | 832,833 |

  

* ApiJson 序列化後字串範例

| ApiID | Api名稱      | ApiJson                                                      |
| ----- | ------------ | ------------------------------------------------------------ |
| 179   | 購買月租     | {\\"MonProjID\\":\\"MR66\\",\\"MonProPeriod\\":3,\\"ShortDays\\":0,\\"SetSubsNxt\\":0} |
| 188   | 月租升轉     | {\\"MonProjID\\":\\"MR66\\",\\"MonProPeriod\\":3,\\"ShortDays\\":0,\\"UP_MonProjID\\":\\"MR102\\",\\"UP_MonProPeriod\\":3,\\"UP_ShortDays\\":0} |
| 190   | 月租欠費繳交 | {\\"MonthlyRentIds\\":\\"832,833\\"}                         |

* input範例 (購買月租179)
```
{
    "ApiID":179,
    "ApiJson":"{\"MonProjID\":\"MR66\",\"MonProPeriod\":3,\"ShortDays\":0,\"SetSubsNxt\":1}",
    "ProdNm":"測試_汽包機66-3",
    "ProdPrice":7000,
    "DoPay":0,
    "PayTypeId":5,
    "InvoTypeId":6
}
```

* input範例 (月租升轉188)

  ```
  {
      "ApiID":188,
      "ApiJson":"{\"MonProjID\":\"MR66\",\"MonProPeriod\":3,\"ShortDays\":0,\"UP_MonProjID\":\"MR102\",\"UP_MonProPeriod\":3,\"UP_ShortDays\":0}",
      "ProdNm":"測試_汽包機102-3",
      "ProdPrice":7300,
      "DoPay":1,
      "PayTypeId":5,
      "InvoTypeId":6
  }
  ```

* input範例 (月租欠費繳交190)

  ```
  {
      "ApiID":190,
      "ApiJson":"{\"MonthlyRentIds\":\"832,833\"}",
      "ProdNm":"",
      "ProdPrice":7000,
      "DoPay":1,
      "PayTypeId":5,
      "InvoTypeId":12
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

* 資料物件說明(DoPay=0 顯示)

| 參數名稱    | 參數說明                    | 型態    | 範例                  |
| --------    | --------                    | :--:    | ----------------------|
| ProdNm      | 產品名稱                    | string  |  測試_汽包機66-3      |
| ProdDisc    | 產品描述                    | string  |  空字串               |
| ProdPrice   | 產品價格                    | int     |  7000                 |
| PayTypes    | 資料物件:付款方式(list)     |         |                       |
| InvoTypes   | 資料物件:發票設定(list)     |         |                       |
| PayResult   | 付費結果(0失敗 1成功)       | int     |    0                  |


* 資料物件說明(DoPay=1 付款)

| 參數名稱    | 參數說明                    | 型態    | 範例                  |
| --------    | --------                    | :--:    | ----------------------|
| ProdNm      | 產品名稱                    | string  |  空字串  |
| ProdDisc    | 產品描述                    | string  |  空字串  |
| ProdPrice   | 產品價格                    | int     |  皆為0   |
| PayTypes    | 資料物件:付款方式(list)     |         |  空陣列  |
| InvoTypes   | 資料物件:發票設定(list)     |         |  空陣列  |
| PayResult   | 付費結果(0失敗 1成功)       | int     |    1     |

* PayTypes, InvoTypes 參數說明

| 參數名稱      | 參數說明              |  型態  | 範例               |
| -----------   | ----------            | :----: | -------------------|
| CodeId        | 代碼                  | int    | 5                  |
| CodeNm        | 名稱                  | string | 信用卡,手機條碼    |
| IsBind        | 是否為預設值(0否 1是) | int    | 0                  |
| Disc          | 其它描述              | string | (預留)目前為空字串 |

* Output範例,汽車牌卡(DoPay=0 顯示)
```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "ProdNm": "測試_汽包機66-3",
        "ProdDisc": "",
        "ProdPrice": 7000,
        "PayTypes": [
            {
                "CodeId": 5,
                "CodeNm": "信用卡",
                "IsBind": 1,
                "Disc": ""
            }
        ],
        "InvoTypes": [
            {
                "CodeId": 9,
                "CodeNm": "手機條碼",
                "IsBind": 0,
                "Disc": ""
            },
            {
                "CodeId": 10,
                "CodeNm": "自然人憑證",
                "IsBind": 0,
                "Disc": ""
            },
            {
                "CodeId": 7,
                "CodeNm": "二聯",
                "IsBind": 0,
                "Disc": ""
            },
            {
                "CodeId": 8,
                "CodeNm": "三聯",
                "IsBind": 0,
                "Disc": ""
            },
            {
                "CodeId": 6,
                "CodeNm": "捐贈碼",
                "IsBind": 0,
                "Disc": ""
            },
            {
                "CodeId": 12,
                "CodeNm": "email",
                "IsBind": 1,
                "Disc": ""
            }
        ],
        "PayResult": 0
    }
}
```

* Output範例,汽車牌卡(DoPay=1 付款)

```
{
    "Result": "1",
    "ErrorCode": "000000",
    "NeedRelogin": 0,
    "NeedUpgrade": 0,
    "ErrorMessage": "Success",
    "Data": {
        "ProdNm": "",
        "ProdDisc": "",
        "ProdPrice": 0,
        "PayTypes": [],
        "InvoTypes": [],
        "PayResult": 1
    }
}
```



## GetMySubs 我的方案牌卡明細
### [/api/GetMySubs/]

* 20210511發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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
| PayTypes    | 資料物件 | list | |
| InvoTypes   | 資料物件 | list | |

* Month 參數說明

| 參數名稱      | 參數說明              |  型態  | 範例               |
| -----------   | ----------            | :----: | -------------------|
| MonProjID     | 月租專案代碼  | string | MR66 |
| MonProPeriod  | 總期數		| int | 3 |
| ShortDays		| 短天期天數    | int | 0 |
| MonProjNM		| 月租專案名稱  | string | 測試_汽包機66-3 |
| CarWDHours	| 汽車平日時數	| double | 3.0             |
| CarHDHours	| 汽車假日時數	| double | 3.0 |
| MotoTotalMins | 機車不分平假日分鐘數 | double | 300.0 |
| WDRateForCar | 汽車平日優惠價 | double | 99.0 |
| HDRateForCar | 汽車假日優惠價 | double | 168.0 |
| WDRateForMoto | 機車平日優惠價 | double | 1.0 |
| HDRateForMoto | 機車假日優惠價 | double | 1.2 |
| StartDate 	| 起日 			| string | 05/18 |
| EndDate 		| 迄日 			| string | 06/16 23:59 |
| MonthStartDate | 全月租專案起日 | string | 2021/05/18 |
| MonthEndDate | 全月租專案迄日 | string | 2021/08/16 |
| NxtMonProPeriod | 下期續訂總期數 | string | 3 |
| IsMix | 是否為城市車手 (0否1是) | int | 1 |
| IsUpd | 是否已升級 (0否1是) | int | 0 |
| SubsNxt		| 是否自動續訂 (0否1是) | int | 1 |
| IsChange		| 是否變更下期合約 (0否1是) | int | 0 |
| IsPay 		| 是否當期有繳費 (0否1是) | int | 1 |


* PayTypes, InvoTypes 參數說明

| 參數名稱      | 參數說明              |  型態  | 範例               |
| -----------   | ----------            | :----: | -------------------|
| CodeId        | 代碼                  | int    | 5                  |
| CodeNm        | 名稱                  | string | 信用卡,手機條碼    |
| IsDef         | 是否為預設值(0否 1是) | int    | 0                  |


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
            "MotoTotalMins": 300.0,
            "WDRateForCar": 99.0,
            "HDRateForCar": 168.0,
            "WDRateForMoto": 1.0,
            "HDRateForMoto": 1.2,
            "StartDate": "05/18",
            "EndDate": "06/16 23:59",
            "MonthStartDate": "2021/05/18",
            "MonthEndDate": "2021/08/16",
            "NxtMonProPeriod": 3,
            "IsMix": 1,
            "IsUpd": 0,
            "SubsNxt": 1,
            "IsChange": 0,
            "IsPay": 1
        },
        "PayTypes": [
            {
                "CodeId": 5,
                "CodeNm": "信用卡",
                "IsDef": 1
            }
        ],
        "InvoTypes": [
            {
                "CodeId": 9,
                "CodeNm": "手機條碼",
                "IsDef": 0
            },
            {
                "CodeId": 10,
                "CodeNm": "自然人憑證",
                "IsDef": 0
            },
            {
                "CodeId": 8,
                "CodeNm": "三聯",
                "IsDef": 0
            },
            {
                "CodeId": 6,
                "CodeNm": "捐贈碼",
                "IsDef": 0
            },
            {
                "CodeId": 12,
                "CodeNm": "會員載具",
                "IsDef": 1
            }
        ]
    }
}
```




## GetSubsCNT 取得合約明細

### [/api/GetSubsCNT/]

* 20210511發佈

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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
| MotoTotalMins | 機車不分平假日分鐘數 	| double | 300.0 |
| WDRateForCar | 汽車平日優惠價 | double | 99.0 |
| HDRateForCar | 機車平日優惠價 | double | 168.0 |
| WDRateForMoto | 機車平日優惠價 | double | 1.0 |
| HDRateForMoto | 機車假日優惠價 | double | 1.2 |
| StartDate | 起日 			| string | 05/18 |
| EndDate	| 迄日 			| string | 08/16 |
| MonProDisc  	| 注意事項      | string  | 汽包機66-3注意事項 |



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
            "StartDate": "05/18",
            "EndDate": "08/16",
            "MonProDisc": "汽包機66-3注意事項"
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
            "StartDate": "2021/08/16",
            "EndDate": "2021/11/14",
            "MonProDisc": "汽包機66-3注意事項"
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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                   | 必要 |  型態  | 範例                           |
| ---------- | -------------------------- | :--: | :----: | ------------------------------ |
| MonProID     | 方案代碼(key)              |  Y   | string | MR66                          |
| MonProPeriod | 總期數(key)               |  Y  | int    | 3                     |
| ShortDays    | 短期總天數(key)            |  Y  | int    | 0                              |


* input範例
```
{
    "MonProID"::"MR66",
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
| MotoTotalMins	| 機車不分平假日分鐘數   | double | 120      |
| WDRateForCar	| 汽車平日優惠費率 | double | 99.0 |
| HDRateForCar	| 假日平日優惠費率 | double | 168.0 |
| WDRateForMoto	| 機車平日優惠費率 | double | 1.0 |
| HDRateForMoto	| 機車假日優惠費率 | double | 1.2 |
| IsDiscount	| 是否為優惠方案0否1是  | int    | 0       |


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
            "IsDiscount": 0
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
                "IsDiscount": 0
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
                "IsDiscount": 0
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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* 動作 [POST]
  
* input傳入參數說明

| 參數名稱   | 參數說明                   | 必要 |  型態  | 範例                           |
| ---------- | -------------------------- | :--: | :----: | ------------------------------ |
| MonProID     | 方案代碼(key)              |  Y   | string | MR66                          |
| MonProPeriod | 總期數(key)               |  Y  | int    | 3                      |
| ShortDays    | 短期總天數(key)            |  Y  | int    | 0                              |


* input範例
```
{
    "MonProID"::"MR66",
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


* Output範例

```
{
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
                "IsDiscount": 0
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
            "IsDiscount": 0
        }, {
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
            "IsDiscount": 0
        }
    ]
}
```

---

## GetSubsHist 訂閱制歷史紀錄

### [/api/GetSubsHist/DoGetSubsHist]

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

  不需傳入參數


* input範例
```
{
   
}
```


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
                "invoice_price": 7000
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
                "InvType": "捐贈碼",
                "unified_business_no": "12345678",
                "invoiceCode": "A12345678",
                "invoice_date": "2021/05/19",
                "invoice_price": 7000
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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* 動作 [POST]

* input傳入參數說明

| 參數名稱   | 參數說明                   | 必要 |  型態  | 範例                           |
| ---------- | -------------------------- | :--: | :----: | ------------------------------ |
| MonProID     | 方案代碼(key)              |  Y   | string | MR66                          |
| MonProPeriod | 總期數(key)               |  Y  | int    | 3                     |
| ShortDays    | 短期總天數(key)            |  Y  | int    | 0                              |

* input範例
```
{
    "MonProID"::"MR66",
    "MonProPeriod:3,
    "ShortDays":"0"
}
```


* Output回傳參數說明

| 參數名稱      | 參數說明                |  型態  | 範例          |
| ------------ | ----------------------- | :----: | ------------- |
| TotalArresPrice | 欠費總計 | int |                 |
| Cards     | 欠費列表 | list |                 |
| Cards-StartDate | 月租起日         | string |  　 |
| Cards-EndDate | 月租迄日 | string | |
| Cards-ProjNm | 月租專案名稱 | string | |
| Cards-Arrs | 欠費明細 | list | |



* 資料物件說明(Arrs)

| 參數名稱    | 參數說明               | 型態   | 範例                  |
| --------   | --------             | :--:   |----------------------|
| Period     | 繳費期數     | int    |                      |
| ArresPrice | 繳費金額       | int    |  　            |



* Output範例


```
{
	"TotalArresPrice": 27000,
	"Cards": [
		{
			"StartDate": "2021/05/19",
			"EndDate": "2021/08/17",
			"ProjNm": "測試_機車2000",
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
```



# 訂單相關



## OrderDetail

### [/api/OrderDetail/]

* 20210517修改

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

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
		"TotalRewardPoint" : 0
    
	}
    
}
```
----
