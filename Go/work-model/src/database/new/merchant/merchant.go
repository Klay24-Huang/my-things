package merchant

import (
	"appserver/src/database/new/common"
	"time"
)

//////// 商戶控端 ////////////

// 集團
type Corporation struct {
	common.UUID
	Name string `gorm:"unique;not null;type:char(20)"`
	// todo 確認用途
	LoginUrl string `gorm:"unique;not null;type:char(30)"`
	// 待審核 / 已通過 / 已拒絕
	Status uint `gorm:"not null;default:0;"`
	// Verified bool   `gorm:"defaut:false;not null"`
	Enable bool   `gorm:"default:false;not null;"`
	Note   string `gorm:"type:char(30);default:null;"`
	// 管端登入ip限制
	LimitLoginIP bool `gorm:"default:true;not null;"`
	// 登入白名單
	Whitelistings string `gorm:"type:json"`
	common.CreatedAtAndUpdatedAt
}

// 單一商戶主要資料
// 非商戶相關會員
type Merchant struct {
	common.ID
	CorperationID uint `gorm:"not null;default:0;"`
	Corporation
	WalletID string `gorm:"not null;"`
	Name     string `gorm:"type:char(20);not null;"`
	Phone    string `gorm:"type:char(15);"`
	// 掛單出售Y幣
	Sell bool `gorm:"default:true;not null"`
	// 會員出款
	Withdraw bool `gorm:"default:false;not null"`
	// 控端 url
	ConsoleUrl string `gorm:"not null;type:char(100);"`
	Url        string `gorm:"not null;type:text"`
	// test url
	TestUrl string `gorm:"not null;type:text"`
	// 商戶圖示
	Picture string `gorm:"not null;type:text;"`
	///// 手續費
	// 存款手續費
	DepositFee uint `gorm:"not null;default:0;"`
	// 提款手續費
	WithdrawalFee uint `gorm:"not null;default:0;"`
	// 掛單手續費
	OrderFee uint `gorm:"not null;default:0;"`
	// 回收手續費
	RecyclingFee uint `gorm:"not null;default:0;"`
	// 回收代幣數量下限
	RecyclingLimit uint `gorm:"not null;default:0;"`
	// 可出售Y幣
	Salable bool `gorm:"not null;default:true;"`
	// 會員可出款
	Dispensable bool `gorm:"not null;default:true;"`
	// 安全存量
	SaftyAmount uint `gorm:"not null;default:0;"`
	// 待審核 / 已通過 / 已拒絕
	Status uint `gorm:"not null;defualt:0;"`
	// Verified bool   `gorm:"defaut:false;not null"`
	Enable bool   `gorm:"default:false;not null"`
	APIKey string `gorm:"not null;type:char(50)"`
	// 管端登入ip限制
	LimitLoginIP      bool   `gorm:"default:true;not null"`
	Whitelistings     string `gorm:"type:json"`
	TestWhitelistings string `gorm:"type:json"`

	///// 體系 /////
	// 如果是某個體系的主商戶，則此欄位有值
	SystemID uint
	// 如果屬於某個體系的成員，則此欄位有值
	AssociatedSystemID uint
	Setting            Setting

	common.CreatedAtAndUpdatedAt
}

// 商戶銀行卡 此商戶底下所有的帳號都可以看到銀行卡資訊
type BankCard struct {
	common.ID
	MerchantID uint `gorm:"not null;default:0;"`
	Merchant
	BankID     uint   `gorm:"not null;defualt:0;"`
	BranchName string `gorm:"not null;type:char(20);"`
	Code       string `gorm:"not null;type:char(20);"`
	// 卡片使用者名稱
	Name string `gorm:"not null;type:char(20);"`
	common.CreatedAtAndUpdatedAt
	common.DeletedAt
}

// 預設域名列表 可以在創建商戶時選擇
type Domain struct {
	common.ID
	Name string `gorm:"not null;type:char(50)"`
	common.CreatedAtAndUpdatedAt
}

// 帳號類別 推播管理
type Boardcast struct {
	common.ID
	MerchantID uint `gorm:"not null;default:0;"`
	Merchant
	// todo 補幣/ 補Y幣 差別?
	//商戶 補幣審核通過 / Y幣下發審核通過 / Y幣補幣審核通過
	Type uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 商戶 交收體系
// 一個商戶只能在一個體系
type System struct {
	common.ID
	// 主商戶
	MainMerchant Merchant
	Name         string `gorm:"char(30); not null;"`
	// 此體系底下的商戶
	Merchants []Merchant `gorm:"foreignKey:AssociatedSystemID;"`
	common.CreatedAtAndUpdatedAt
}

// 跑馬燈
type Marquee struct {
	common.ID
	// 發給所有集團
	AllCorporations bool `gorm:"not null;default:false"`
	// 單一集團
	CorporationID string `gorm:"type:UUID();"`
	Corporation   Corporation
	// 所有商戶
	AllMerchants bool `gorm:"not null;default:false"`
	// 單一商戶
	MerchantID uint
	Merchant   Merchant

	Content   string `gorm:"not null;type:char(50);"`
	StartedAt time.Time
	EndedAt   time.Time
	Verified  bool `gorm:"defaut:false;not null"`
	// 附檔
	common.Attachment
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 公告
type Bulletin struct {
	common.ID
	// 單一集團ID 有值時發給單一集團，null時則發給全部集團
	CorporationID uint
	Corporation
	Content  string `gorm:"not null;type:char(100);"`
	Verified bool   `gorm:"defaut:false;not null"`
	common.CreatedAtAndUpdatedAt
}

// 商戶控端 系統設定
type ConsoleSystemSetting struct {
	// 例行性維護
	Routine bool `gorm:"not null;default:false"`
	// 金流系統維護
	PaymentFlow bool `gorm:"not null;default:false"`
	StartedAt   time.Time
	EndedAt     time.Time
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// ////////////////// 商戶管端 ///////////////////////
type Setting struct {
	// 商戶配置頁面
	MerchantID  uint
	Page        bool `gorm:"not null;default:false;"`
	ApiKey      bool `gorm:"not null;default:false;"`
	IPWhitelist bool `gorm:"not null;default:false;"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 營運管理
type SystemSetting struct {
	common.ID
	Initial    uint `gorm:"not null;default:0;"`
	AlarmRatio uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// todo 關於商戶 補幣 上交 計算手續費 凍結 等等跟貨幣有關table 未全
