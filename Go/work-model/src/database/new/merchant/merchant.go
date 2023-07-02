package merchant

import (
	"appserver/src/database/new/common"
	"appserver/src/database/new/user"
	"time"
)

//////// 商戶控端 ////////////

// 商控 群組
type MerchantGroup struct {
	common.ID
	Name  string `gorm:"unique;not null;type:varchar(30)"`
	Level uint
	Note  string `gorm:"type:varchar(30);default:null"`
	common.CreateAtAndUpdateAt
}

// 集團
type Corporation struct {
	common.UUID
	Name string `gorm:"unique;not null;type:varchar(20)"`
	// todo 確認用途
	LoginUrl string `gorm:"unique;not null;type:varchar(30)"`
	Verified bool   `gorm:"defaut:false;not null"`
	Enable   bool   `gorm:"default:false;not null"`
	// 管端登入ip限制
	LimitLoginIP bool   `gorm:"default:true;not null"`
	Note         string `gorm:"type:varchar(30);default:null"`
	// todo domain name
	common.CreateAtAndUpdateAt
}

// 集團管端登入ip白名單
type CorporationWhitelistring struct {
	CorporationID string `gorm:"type:uuid;not null;uniqueIndex:corp_id_ip"`
	Corporation   Corporation
	IP            string `gorm:"type:varchar(15);not null;uniqueIndex:corp_id_ip"`
	//狀態 1新增 / 0刪除
	Statue bool `gorm:"default:true"`
	// 操作者ID

	// TODO: 出處有必要? 編輯or新增
	// TODO: 需要記錄成新增者和刪除者?
	common.CreateAtAndUpdateAt
}

// 商戶
type Merchant struct {
	common.ID
	CorperationID uint `gorm:"not null;"`
	Corporation
	// todo wallet id 是否可用user id取代/可能會有多個錢包 最多五個錢包
	WalletID uint `gorm:"not null;"`
	Wallet
	Name  string `gorm:"type:varchar(20);not null;"`
	Phone string `gorm:"type:varchar(15);default:null;"`
	// 掛單出售Y幣
	Sell     bool `gorm:"default:true;not null"`
	Transfer bool `gorm:"default:true;not null"`
	// 控端 url
	ConsoleUrl string `gorm:"not null;type:char(100);"`
	Url        string `gorm:"not null;type:text"`
	// test url
	TestUrl  string `gorm:"not null;type:text"`
	Verified bool   `gorm:"defaut:false;not null"`
	Enable   bool   `gorm:"default:false;not null"`

	MerchantSetting

	common.CreateAtAndUpdateAt
}

type Domain struct {
	common.ID
	Name string `gorm:"not null;type:varchar(50)"`
	common.CreateAtAndUpdateAt
}

type MerchantPasswordResetLog struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	Merchant
	common.Operator
	common.CreatedAt
}

type UnbindMerchantPhoneLog struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	Merchant
	Phone string `gorm:"type:varchar(15);default:null;"`
	common.Operator
	common.CreatedAt
}

type AccountLockLog struct {
	LockTime time.Time
	UserID   uint `gorm:"not null;"`
	User     user.User
	Locked   bool   `gorm:"default:false;not null;"`
	Reason   string `gorm:"type:varchar(30);not null;"`
	// todo operator?
	common.CreateAtAndUpdateAt
}

type MerchantLockLog struct {
	LockTime   time.Time
	MerchantID uint `gorm:"not null;"`
	Merchant
	// todo operator?
	common.CreateAtAndUpdateAt
}

// google 驗證操作紀錄
type MerchantOTPLog struct {
	MerchantID uint `gorm:"not null;"`
	Merchant
	// 操作內容 啟用/停用 2fa
	Operation string `gorm:"not null;type:varchar(30);"`
	common.Operator
}

// type Boardcast struct {
// 	MerchantID uint `gorm:"not null;"`
// 	Merchant
// 	// todo 補幣/ 補Y幣 差別?
// 	//商戶 補幣審核通過 / Y幣下發審核通過 / Y幣補幣審核通過
// 	Flags uint
// 	// operator?
// 	common.CreateAtAndUpdateAt
// }

type OperatorItem struct {
	common.NameBasic
}

type OperationDetail struct {
	common.NameBasic
}

type OperationLog struct {
	common.Operator
	OperatorItem
	OperationDetail
	Content string `gorm:"type:varchar(30);not null;"`
	common.IP
	common.CreatedAt
}

type Wallet struct {
	common.UUID
	Balance int
	common.CreateAtAndUpdateAt
}

type MerchantWalletSetting struct {
	// 商戶錢包管理
	//todo 帶商戶管端開完 再回來會比較清楚
}

// yapay 錢包會員
type WalletUser struct {
	common.ID
	Name string `gorm:"not null;type:varchar(30);"`
	common.CreateAtAndUpdateAt
}

type MerchantBindWalletUser struct {
	MerchantID uint `gorm:"not null;"`
	Merchant
	WalletUserID uint
	WalletUser
	Bingding bool `gorm:"not null;default:true;"`
	// todo 出入款紀錄
	// todo 解除再綁定的話 出入款金額的計算方法
	common.CreateAtAndUpdateAt
}

// todo 商戶控端 先到會員綁定

// todo 商戶類別先跳過

// 跑馬燈
type Marquee struct {
	common.ID
	// 發給所有集團
	AllCorporations bool `gorm:"not null;default:false"`
	// 單一集團
	CorporationID uint
	Corporation
	// 所有商戶
	AllMerchants bool `gorm:"not null;default:false"`
	// 單一商戶
	MerchantID uint
	Merchant
	Content string `gorm:"not null;type:varchar(50);"`
	StartAt time.Time
	EndAt   time.Time
	// VerifyID uint
	// Verify
	common.Operator
	common.CreateAtAndUpdateAt
}

// 公告
type Bulletin struct {
	common.ID
	// 發給所有集團
	AllCorporations bool `gorm:"not null;default:false"`
	// 單一集團
	CorporationID uint
	Corporation
	Content string `gorm:"not null;type:varchar(100);"`
	common.CreateAtAndUpdateAt
}

type MerchantSystemSetting struct {
	// 例行性維護
	Routine bool `gorm:"not null;default:false"`
	// 金流系統維護
	PaymentFlow bool `gorm:"not null;default:false"`
	StartAt     time.Time
	EndAt       time.Time
	common.Operator
	common.CreateAtAndUpdateAt
}

// ////////////////// 商戶控端 ///////////////////////
type MerchantSetting struct {
	// 商戶配置葉面
	MerchantID  uint
	Page        bool `gorm:"not null;default:false;"`
	ApiKey      bool `gorm:"not null;default:false;"`
	IPWhitelist bool `gorm:"not null;default:false;"`
	common.Operator
	common.CreateAtAndUpdateAt
}

// 商控腳色
type Role struct {
	// 集團管理員 商戶管理員 站長 開發人員 行銷人員
	common.NameBasic
	common.CreateAtAndUpdateAt
}

type MerchantUserRole struct {
	UserID uint `gorm:"idx_user_role"`
	RoleID uint `gorm:"idx_user_role"`
	Role
	common.Operator
	common.CreateAtAndUpdateAt
}

type BankCard struct {
	common.ID
	MerchantID uint   `gorm:"not null;"`
	UserName   string `gorm:"not null;type:varchar(20);"`
	BankName   string `gorm:"not null;type:varchar(30);"`
	// 分行名稱
	Branch string `gorm:"not null;type:varchar(30);"`
	// 卡號
	Code string `gorm:"not null;type:varchar(30);"`
	common.CreateAtAndUpdateAt
}

// todo 關於商戶 補幣 上交 計算手續費 凍結 等等跟貨幣有關table 未全
// todo 綁定遊戲api相關
