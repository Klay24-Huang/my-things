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
	Status uint `gorm:"not null;"`
	// Verified bool   `gorm:"defaut:false;not null"`
	Enable bool   `gorm:"default:false;not null"`
	Note   string `gorm:"type:char(30);default:null"`
	// 管端登入ip限制
	LimitLoginIP bool `gorm:"default:true;not null"`
	// 登入白名單
	Whitelistings string `gorm:"type:json"`
	common.CreateAtAndUpdateAt
}

// 集團管端登入ip白名單紀錄
type CorporationWhitelistingLog struct {
	common.ID
	CorporationID string `gorm:"type:uuid;not null;uniqueIndex:corp_id_ip"`
	Corporation   Corporation
	IP            string `gorm:"type:char(15);not null;uniqueIndex:corp_id_ip"`
	//狀態 1新增 / 0刪除
	Statue bool `gorm:"default:true"`
	// 操作者ID
	common.Operator
	common.CreateAtAndUpdateAt
}

// 商戶
type Merchant struct {
	common.ID
	CorperationID uint `gorm:"not null;"`
	Corporation
	// todo wallet id 是否可用user id取代/可能會有多個錢包 最多五個錢包
	WalletID string `gorm:"not null;"`
	Name     string `gorm:"type:char(20);not null;"`
	Phone    string `gorm:"type:char(15);"`
	// 掛單出售Y幣
	Sell     bool `gorm:"default:true;not null"`
	Transfer bool `gorm:"default:true;not null"`
	// 控端 url
	ConsoleUrl string `gorm:"not null;type:char(100);"`
	Url        string `gorm:"not null;type:text"`
	// test url
	TestUrl string `gorm:"not null;type:text"`
	// 商戶圖示
	Pic string `gorm:"not null;type:text:"`
	///// 手續費
	// 存款手續費
	DepositFee uint `gorm:"not null;"`
	// 提款手續費
	WithdrawalFee uint `gorm:"not null;"`
	// 掛單手續費
	OrderFee uint `gorm:"not null;"`
	// 回收手續費
	RecyclingFee uint `gorm:"not null;"`
	// 回收代幣數量下限
	RecyclingLimit uint `gorm:"not null;"`
	// 可出售Y幣
	Salable bool `gorm:"not null;default:true;"`
	// 會員可出款
	Dispensable bool `gorm:"not null;default:true;"`
	// 安全存量
	SaftyAmount uint `gorm:"not null;"`
	// 待審核 / 已通過 / 已拒絕
	Status uint `gorm:"not null;"`
	// Verified bool   `gorm:"defaut:false;not null"`
	Enable bool   `gorm:"default:false;not null"`
	APIKey string `gorm:"not null;type:char(50)"`
	// 管端登入ip限制
	LimitLoginIP      bool   `gorm:"default:true;not null"`
	Whitelistings     string `gorm:"type:json"`
	TestWhitelistings string `gorm:"type:json"`

	System
	MerchantSetting

	common.CreateAtAndUpdateAt
}

// 預設域名列表 可以在創建商戶時選擇
type Domain struct {
	common.ID
	Name string `gorm:"not null;type:char(50)"`
	common.CreateAtAndUpdateAt
}

// 帳號類別 推播管理
type Boardcast struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	Merchant
	// todo 補幣/ 補Y幣 差別?
	//商戶 補幣審核通過 / Y幣下發審核通過 / Y幣補幣審核通過
	Flags uint
	common.CreateAtAndUpdateAt
}

// 商控 遊戲會員錢包綁定 商戶
type MerchantBindWalletUser struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	Merchant
	WalletID string
	Binding  bool `gorm:"not null;default:true;"`
	// todo 出入款紀錄
	// todo 解除再綁定的話 出入款金額的計算方法
	common.CreateAtAndUpdateAt
}

// 商戶 交收體系
// 一個商戶只能在一個體系
type System struct {
	common.ID
	// 主商戶ID
	MainMerchantID uint
	Name           string `gorm:"char(30); not null;"`
	Merchants      []Merchant
	common.CreateAtAndUpdateAt
}

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
	Content  string `gorm:"not null;type:char(50);"`
	StartAt  time.Time
	EndAt    time.Time
	Verified bool `gorm:"defaut:false;not null"`
	common.Operator
	common.CreateAtAndUpdateAt
}

// 公告
type Bulletin struct {
	common.ID
	// 單一集團ID 有值時發給單一集團，null時則發給全部集團
	CorporationID uint
	Corporation
	Content  string `gorm:"not null;type:char(100);"`
	Verified bool   `gorm:"defaut:false;not null"`
	common.CreateAtAndUpdateAt
}

// 商戶控端 系統設定
type MerchantConsoleSystemSetting struct {
	// 例行性維護
	Routine bool `gorm:"not null;default:false"`
	// 金流系統維護
	PaymentFlow bool `gorm:"not null;default:false"`
	StartedAt   time.Time
	EndedAt     time.Time
	common.Operator
	common.CreateAtAndUpdateAt
}

// ////////////////// 商戶管端 ///////////////////////
type MerchantSetting struct {
	// 商戶配置葉面
	MerchantID  uint
	Page        bool `gorm:"not null;default:false;"`
	ApiKey      bool `gorm:"not null;default:false;"`
	IPWhitelist bool `gorm:"not null;default:false;"`
	common.Operator
	common.CreateAtAndUpdateAt
}

type BankCard struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	Merchant
	UserName string `gorm:"not null;type:char(20);"`
	// TODO 關連商戶Bank list
	BankName string `gorm:"not null;type:char(30);"`
	// 分行名稱
	Branch string `gorm:"not null;type:char(30);"`
	// 卡號
	Code string `gorm:"not null;type:char(30);"`
	common.CreateAtAndUpdateAt
}

// 營運管理
type MerchantSystemSetting struct {
	common.ID
	Initial    int  `gorm:"not null;default:0;"`
	AlarmRatio uint `gorm:"not null;default:0;"`
	common.CreateAtAndUpdateAt
}

// todo 商戶 會員 站長
// todo 關於商戶 補幣 上交 計算手續費 凍結 等等跟貨幣有關table 未全
// todo 綁定遊戲api相關
