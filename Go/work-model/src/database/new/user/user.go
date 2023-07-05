package user

import (
	"appserver/src/database/new/common"
	"time"
)

type User struct {
	common.ID
	Account  string `gorm:"unique;not null;type:char(30)"`
	Password string `gorm:"not null;type:char(30)"`
	Name     string `gorm:"not null;type:char(30);"`
	Note     string `gorm:"default:null;type:char(30)"`
	// 商戶管端 / 商戶控端 / 錢包管端 / 錢包user / 造市商管端 / 造市商user / 遊戲user
	Type          uint   `gorm:"not null;"`
	OtpEnable     bool   `gorm:"default:false;not null"`
	OtpVerified   bool   `gorm:"default:false;not null"`
	OtpSecret     string `gorm:"default:null"`
	OtpAuth_url   string `gorm:"default:null"`
	LastLoginedIP common.IP
	LastLoginedAt time.Time

	common.Enable
	common.CreateAtAndUpdateAt
	common.Deleted
	// 連續登入失敗被封鎖
	LockedAt   time.Time
	PublicKeys []PublicKey

	Group
	WalletConsoleUser
	MerchantUserRole
}

type PublicKey struct {
	Key    string `gorm:"primaryKey,not null;type:char(256);"`
	UserID uint
	common.CreatedAt
}

// 群組 (共用)
type Group struct {
	common.ID
	// 商控 / 商管 / 錢包管 / 造市商管
	Type  uint   `gorm:"not null;"`
	Name  string `gorm:"not null;type:char(20);"`
	Level uint
	Note  string `gorm:"type:char(30);default:null"`
	// 這個群組可以用的造市商控端功能列表，json與bit flag格式
	FunctionSetting string `gorm:"not null;type:json;"`
	common.CreateAtAndUpdateAt
}

// //// 造市商 app ///////////
// app user
type MarketMakerUser struct {
	common.ID
	UserID uint `gorm:"not null;"`
	User
	// 是否為特殊代理
	SpecialAgent bool `gorm:"not null;default:false;"`
	// 此user的一級代理是誰
	HighestUserID *uint
	HighestUser   *MarketMakerUser
	Level         uint `gorm:"not null;"`
	// 通訊軟體平台
	Messager        string `gorm:"not null;type:char(30);"`
	MessagerAccount string `gorm:"not null;type:char(30);"`
	// 推薦人
	RecommenderID       *uint
	Recommender         *MarketMakerUser
	RecommenderVerified bool `gorm:"not null;default:false;"`
	// 帳號開通
	Active    bool `gorm:"not null;default:false;"`
	Suspended bool `gorm:"not null;default:fasle;"`
	// 允許編輯銀行卡
	EditBankCard bool   `gorm:"not null;default:true;"`
	Phone        string `gorm:"not null;type:char(15);"`
	RegisterIP   common.IP

	/////// 代收代付
	// 可代收
	Collectable bool `gorm:"not null;default:false;"`
	// 代收獎金比例
	CollectionRatio float32 `gorm:"not null;"`
	// 代收上限
	CollectionDayLimit uint `gorm:"not null;"`
	// 可代付
	Payable bool `gorm:"not null;default:false;"`
	// 代付獎金比例
	PayingRatio float32 `gorm:"not null;"`
	// 代付上限
	PayingDayLimit uint `gorm:"not null;"`

	MarketMakerBankCardSettings []MarketMakerBankCardSetting
	common.CreateAtAndUpdateAt
}

type MarketMakerBankCardSetting struct {
	common.ID
	MarketUserID uint `gorm:"not null;"`
	MarketMakerUser
	// 代收 1 / 代付 2 / 代收付 3
	Type   uint   `gorm:"not null;"`
	Enable bool   `gorm:"not null;default:true;"`
	Bank   string `gorm:"not null;default:not null;type:char(20);"`
	Branch string `gorm:"not null;default:not null;type:char(20);"`
	Name   string `gorm:"not null;default:not null;type:char(20);"`
	// 單筆上限
	Limit uint `gorm:"not null;"`
	// 每日上限
	DayLimit uint `gorm:"not null;"`

	common.CreateAtAndUpdateAt
	common.Deleted
}

// ///// 錢包控端 ////////

// 錢包控端使用者
type WalletConsoleUser struct {
	common.ID
	UserID  uint
	GroupID uint
	Group
	common.CreateAtAndUpdateAt
}

// ///// 錢包app ///////
// 錢包app使用者
type WalletUser struct {
	common.ID
	RegisteredIP common.IP
	Verified     bool `gorm:"not null;default:false;"`
	// 停權
	Suspend bool `gorm:"not null;default:false;"`
	WalletUserVerify
	common.CreateAtAndUpdateAt
}

// 錢包app 實名 照片 ID card認證
type WalletUserVerify struct {
	common.ID
	WalletUserID uint
	RealName     string `gorm:"not null;type:char(20);"`
	IDCardNumber string `gorm:"not null;type:char(20);"`
	BirthDay     time.Time
	IDCardFront  common.Attachment
	IDCardBack   common.Attachment
	// 手持ID card照片
	UserIDCardPhoto common.Attachment
	// 待審核 已同意 已拒絕
	Status uint `gorm:"not null;"`
	common.CreateAtAndUpdateAt
}

//////// 商戶管端 ////////////

// 商管腳色
type Role struct {
	common.ID
	// 集團管理員 商戶管理員 站長 開發人員 行銷人員
	Name string `gorm:"not null;type:char(20)"`
	common.CreateAtAndUpdateAt
}

// 商管帳號
type MerchantUser struct {
	common.ID
	UserID        uint `gorm:"not null;"`
	CorporationID uint `gorm:"not null;"`
	// 如果為null 則為此集團的跨商戶帳號
	MerchantID uint
	MerchantUserRole
	common.CreateAtAndUpdateAt
}

// 商管帳號 腳色 binding
type MerchantUserRole struct {
	common.ID
	MerchantUserID uint `gorm:"idx_user_role"`
	RoleID         uint `gorm:"idx_user_role"`
	Role
	common.CreateAtAndUpdateAt
}

// //// 商戶 遊戲腳色 ///////
type MerchantGameUser struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	UserID     uint `gorm:"not null;"`
	User
	WalletID string `gorm:"not null;"`
	common.CreateAtAndUpdateAt
}
