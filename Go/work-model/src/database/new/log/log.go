package log

import (
	"appserver/src/database/new/common"
	"appserver/src/database/new/trade"
	"time"
)

// //// 審核相關 ///////////
// 審核類型
type VerificationType struct {
	common.ID
	common.SystemType
	Name string `gorm:"not null;type:char(20)"`
	common.CreatedAtAndUpdatedAt
}

// 審核標題
type VerificationTitle struct {
	common.ID
	Name   string `gorm:"not null;type:char(20)"`
	TypeID uint   `gorm:"not null;default:0;"`
	// 審核類型
	VerificationType VerificationType `gorm:"foreignKey:TypeID;"`
	Level            uint             `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 審核列表
type Verification struct {
	common.ID
	// 標題
	TitleID           uint              `gorm:"not null;default:0;"`
	VerificationTitle VerificationTitle `gorm:"foreignKey:TitleID;"`
	// 事項
	Item string `gorm:"not null;type:char(30);"`
	// 事由
	Reason string `gorm:"not null;type:char(30);"`
	// 修改前資料明細 json
	Before string `gorm:"not null;type:json"`
	// 修改後資料明細 json
	After string `gorm:"not null;type:json"`

	// 1 用戶申請 / 2 管理者同意 / 3 管理者拒絕 / 4 申請人取消
	Status uint `gorm:"not null;default:0;"`
	// 附檔 url
	common.Attachment
	// 申請者
	common.Applicant
	// 審核人
	common.Approver
	// 當審核通過 直接只用下方資訊修改資料內容
	Database string `gorm:"not null;type:char(10);"`
	Table    string `gorm:"not null;type:char(10);"`
	DataID   uint   `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

////// 登入相關紀錄 ////////

// 登入錯誤次數過多被鎖定
type UserLockLog struct {
	common.ID
	UserID uint `gorm:"not null;default:0;"`
	// todo 錢包控端 app user看到有紀錄ip，確認其他系統或app是否也有
	common.IP
	LockedAt   time.Time
	UnLockedAt time.Time
	common.CreatedAtAndUpdatedAt
}

// 登入紀錄
// 商戶控端 管端都有
type LoginLog struct {
	common.ID
	UserID uint `gorm:"not null;default:0;"`
	// 登入平台
	// todo 是否改成用type和數字
	// PC / android / ios / 5min / yapay
	Application string `gorm:"not null;type:char(10)"`
	// 登入IP
	common.IP
	// login statuse 成功 / 失敗
	Statue bool `gorm:"not null;default:false;"`
	// 失敗原因
	Note string `gorm:"default:null;type:char(30)"`
	common.CreatedAt
}

// /// 集團相關紀錄 //////
// 商戶集團管理 -> 商戶集團ip白名單紀錄
type CorporationWhitelistingLog struct {
	common.ID
	CorporationID string `gorm:"type:uuid;not null;uniqueIndex:corp_id_ip"`
	IP            string `gorm:"type:char(15);not null;uniqueIndex:corp_id_ip"`
	//狀態 1新增 / 0刪除
	Statue bool `gorm:"default:true;default:true;"`
	// 操作者ID
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// /// 商戶相關紀錄 /////
// 商戶管理IP白名單紀錄
type MerchantWhitelistingLog struct {
	common.ID
	MerchantID uint `gorm:"not null;default:0;"`
	////// 變動前ip紀錄 //////
	BeforeIPWhitelisting     string `gorm:"type:json;"`
	BeforeTestIPWhitelisting string `gorm:"type:json;"`
	////// 變動後ip紀錄 //////
	AfterIPWhitelisting     string `gorm:"type:json;"`
	AfterTestIPWhitelisting string `gorm:"type:json;"`
	// 控端管端都可以編輯商戶ip
	// 控端 0 / 管端 1
	Source uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAt
}

// TODO 研究丟到cloud watch or 其他log db
// TODO 下方所有log 是否要在收斂成一個類別 用type去切換
type MerchantPasswordResetLog struct {
	common.ID
	MerchantID uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAt
}

type UnbindMerchantPhoneLog struct {
	common.ID
	MerchantID uint   `gorm:"not null;default:0;"`
	Phone      string `gorm:"type:char(15);default:null;"`
	common.Operator
	common.CreatedAt
}

// 控端帳號鎖定
// type AccountLockLog struct {
// 	common.ID
// 	LockTime time.Time
// 	UserID   uint   `gorm:"not null;default:0;"`
// 	Reason   string `gorm:"type:char(30);not null;"`
// 	common.CreatedAtAndUpdatedAt
// }

// // 管端帳號鎖定
// type MerchantLockLog struct {
// 	common.ID
// 	LockTime   time.Time
// 	MerchantID uint `gorm:"not null;"`
// 	common.CreatedAtAndUpdatedAt
// }

// // google 驗證操作紀錄
type MerchantOTPLog struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	// 操作內容 啟用/停用 2fa
	Operation string `gorm:"not null;type:char(30);"`
	common.Operator
}

///// 造市商 ////////

// 造市商收補幣
// todo 目前沒有這個功能 是否保留?
// type MarketMakerSupplementOrRetractLog struct {
// 	common.ID
// 	// 補幣 supplement 1 / 回收 retract 2
// 	Type         int `gorm:"not null;"`
// 	ApplicantKey common.Key
// 	ApproverKey  common.Key
// 	Title        string `gorm:"char(30);"`
// 	// 事項
// 	Content string `gorm:"char(30);"`
// 	Reason  string `gorm:"char(30);"`
// 	Amount  int    `gorm:"not null;"`
// 	common.CreatedAtAndUpdatedAt
// }

// 計算成交訂單任務獎金
type MarketMakerTaskLog struct {
	common.ID
	OrderID uint `gorm:"not null;default:0;"`
	// 獎金比例
	// todo 要每次紀錄嗎? 每個碼商user的獎金比例設定會不一樣，且隨時可以修改
	BounusRatio float32 `gorm:"not null;default:0;"`
	// 實際收到獎金
	Bounus float32 `gorm:"not null;default:0;"`
	common.CreatedAt
}

// 紀錄造市商銀行卡當日是否達到次數上限 或總金額上限
type MarketMakerUserBankCardLog struct {
	common.ID
	MarketMakerUserBankCardSettingID uint `gorm:"not null;default:0;"`
	// 到達次數上限
	Overlimited bool `gorm:"not null;default:false;"`
	Amount      uint `gorm:"not null;default:0;"`
	Count       uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// ///// 錢包控端 ///////
// 補幣 / 收幣
type WalletConsoleRefillAndRecycleLog struct {
	common.ID
	WalletID string `gorm:"not null;"`
	trade.CoinType
	Amount uint `gorm:"not null;default:0;"`
	common.CreatedAt
}

// 回收代幣
type WalletConsoleRecycleLog struct {
	common.ID
	WalletID string `gorm:"not null;"`
	trade.CoinType
	// 申請回收金額
	Amount uint `gorm:"not null;default:0;"`
	// 實際回收金額
	RecycleAmount uint `gorm:"not null;default:0;"`
	// 手續費
	Fee float32 `gorm:"not null;default:0;"`
	common.CreatedAt
}

////// merchant ////////

// todo 下方的log改成帳本形式

// 入款(會員存款) 紀錄
// type DespositAndWithdrawLog struct {
// 	common.ID
// 	// 入款 / 出款
// 	Type         uint `gorm:"not null;defautl:0;"`
// 	UserID       uint
// 	UserWalletID uint
// 	// 手續費率
// 	FeeRatio uint `gorm:"not null;"`
// 	// 實收手續費
// 	Fee              uint `gorm:"not null;"`
// 	MerchantWalletID uint
// 	Amount           uint `gorm:"not null;"`
// 	// 成功 / 用戶未付款 / 已付款 開分失敗
// 	Status uint `gorm:"not null;"`
// 	common.CreatedAt
// 	CompletedAt time.Time
// }

// 商戶補幣
// type MerchantRefillLog struct {
// 	common.ID
// 	WalletID string
// 	trade.CoinType
// 	// 補幣金額
// 	Amount int
// 	// 手續費
// 	Fee int
// 	common.CreatedAt
// }

// // 申請補發x幣
// type MerchantSupplyLog struct {
// 	common.ID
// 	WalletID string
// 	trade.CoinType
// 	// 補幣金額
// 	Amount int
// 	common.CreatedAt
// }

// // 商戶交收
// type MerchantSettlementLog struct {
// 	common.ID
// 	// 付款方錢包ID
// 	PayerWalletID string
// 	Amount        int
// 	Fee           int
// 	// 收款方錢包ID
// 	ReceiverWalletID string
// 	common.CreatedAt
// }

// // 商戶戶轉
// type MerchantTransferLog struct {
// 	MerchantSettlementLog
// }

// // 商戶系統回收
// type MerchantSystemtRecycleLog struct {
// 	common.ID
// 	WalletID string
// 	trade.CoinType
// 	// 申請回收金額
// 	Amount int
// 	common.CreatedAt
// }
