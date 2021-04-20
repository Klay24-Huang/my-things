版本: 1.0

# iRentApi2 WebAPI

iRentApi20 Web API版本

目錄

登入相關

- [Login 登入](#Login)
- [RefrashToken 更新Token](#RefrashToken)
- [CheckAppVersion 檢查APP版本](#CheckAppVersion)
- [GetMemberStatus 取得會員狀態](#GetMemberStatus)

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

<h5 id="Login" name="Login">20210322發佈</h5>

# Login 登入

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

  ### [/api/Login/]

  ### 動作 [POST]

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



<h5 id="RefrashToken" name="RefrashToken">20210322發佈</h5>

# RefrashToken 更新Token

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

  ### [/api/RefrashToken/]

  ### 動作 [POST]

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

<h5 id="CheckAppVersion" name="CheckAppVersion">20210407發佈</h5>

# CheckAppVersion 檢查APP版本

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

  ### [/api/CheckAppVersion/]

  ### 動作 [POST]

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

<h5 id="GetMemberStatus" name="GetMemberStatus">20210322發佈</h5>

# GetMemberStatus 取得會員狀態

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

  ### [/api/GetMemberStatus/]

  ### 動作 [POST]

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
            "TotalRentCount": 0
        }
    }
}
```



# 首頁地圖相關

<h5 id="GetFavoriteStation" name="GetFavoriteStation">20210315發佈</h5>

# GetFavoriteStation取得常用站點
* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

  ### [/api/GetFavoriteStation/]
  ### 動作 [POST]
  
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
| FavoriteObj   | 常用站點列表 |  List  | |

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
| PicDescription | 據點說明 | string | |

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

<h5 id="SetFavoriteStation" name="SetFavoriteStation">20210315發佈</h5>

# SetFavoriteStation設定常用站點
* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

  ### [/api/SetFavoriteStation/]
  ### 動作 [POST]
  
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
| Data | 資料物件 | | |


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

<h5 id="GetCarType" name="GetCarType">20210315修改 - 增加是否為常用據點欄位</h5>
<h5>20210324修改 - 增加搜尋使用欄位CarTypes,Seats </h5>
<h5>20210407修改 - input移除掉Seats </h5>
<h5>20210408修改 - 增加IsRent欄位</h5>

# GetCarType同站以據點取出車型
* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(可不填)**

  ### [/api/GetCarType/]
  ### 動作 [POST]
  
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
| GetCarTypeObj | 車型牌卡清單 | List | |

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

<h5 id="GetProject" name="GetProject">20210315修改 - 增加是否為常用據點欄位</h5>
<h5>20210324修改 - 增加搜尋欄位 </h5>
<h5>20210407修改 - input移除掉Seats </h5>
# GetProject取得專案與資費
* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

  ### [/api/GetProject/]
  ### 動作 [POST]
  
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
| StationInfoObj | 站點照片 | List | |


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

<h5 id="GetBanner" name="GetBanner">20210316發佈</h5>

# GetBanner 取得廣告資訊

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

  ### [/api/GetBanner/]

  ### 動作 [GET]

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

<h5 id="GetNormalRent" name="GetNormalRent">20210324修改</h5>
<h5>20210407修改 - input移除掉Seats </h5>

# GetNormalRent 取得同站租還站點

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

  ### [/api/GetNormalRent/]

  ### 動作 [GET]

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

-------------

<h5 id="GetCarTypeGroupList" name="GetCarTypeGroupList">20210331發佈</h5>

# GetCarTypeGroupList 取得車型清單

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

  ### [/api/GetCarTypeGroupList/]

  ### 動作 [POST]

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

<h5 id="ChangeUUCard" name="ChangeUUCard">20210415發佈</h5>

# ChangeUUCard 變更悠遊卡

* ASP.NET Web API (REST API)

* api位置

  正式環境：https://irentcar-app.azurefd.net/

  測試環境：https://irentv2-app-api.irent-ase.p.azurewebsites.net/

* 傳送跟接收採JSON格式

* HEADER帶入AccessToken**(必填)**

  ### [/api/ChangeUUCard/]

  ### 動作 [POST]

* input傳入參數說明

| 參數名稱 | 參數說明 | 必要 |  型態  | 範例     |
| -------- | -------- | :--: | :----: | -------- |
| OrderNo  | 訂單編號 |  Y   | string | H0002630 |

- input範例

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

