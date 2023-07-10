package user

import (
	"appserver/src/database/new/common"
	"time"
)

type User struct {
	common.ID
	Account  string `gorm:"not null;type:char(30);uniqueIndex:uqidx_account_type;"`
	Password string `gorm:"not null;type:char(30);"`
	Name     string `gorm:"not null;type:char(30);"`
	Note     string `gorm:"default:null;type:char(30)"`
	// 商戶管端 / 商戶控端 / 錢包管端 / 錢包user / 造市商管端 / 造市商user
	Type          uint   `gorm:"not null;default:0;uniqueIndex:uqidx_account_type;"`
	OtpEnable     bool   `gorm:"default:false;not null"`
	OtpVerified   bool   `gorm:"default:false;not null"`
	OtpSecret     string `gorm:"default:null"`
	OtpAuth_url   string `gorm:"default:null"`
	LastLoginedIP string `gorm:"not null;type:char(15);"`
	LastLoginedAt time.Time
	// 啟用
	// 商控 後台帳號管理 新增帳號直接啟用且不用審核
	common.Enable
	// 連續登入失敗被封鎖
	LockedAt   time.Time
	PublicKeys []PublicKey

	GroupID         uint `gorm:"not null;default:0;"`
	Group           `gorm:"foreignKey:GroupID;"`
	WalletUser      WalletUser
	MerchantUser    MerchantUser
	MarketMakerUser MarketMakerUser
	common.CreatedAtAndUpdatedAt
	common.Deleted
}

// user 每次登入的時候要傳public key上來
type PublicKey struct {
	Key    string `gorm:"primaryKey,not null;type:char(255);"`
	UserID uint
	common.CreatedAt
}

// 群組 (共用)
type Group struct {
	common.ID
	// 商控 / 商管 / 錢包管 / 造市商管
	Type  uint   `gorm:"not null;default:0;"`
	Name  string `gorm:"not null;type:char(20);"`
	Level uint
	Note  string `gorm:"type:char(30);default:null"`
	// 這個群組可以用的造市商控端功能列表，json與bit flag格式
	FunctionSetting string `gorm:"not null;type:json;"`
	common.CreatedAtAndUpdatedAt
}

// //// 造市商 app ///////////
// app user
type MarketMakerUser struct {
	common.ID
	UserID uint `gorm:"not null;"`
	// 是否為特殊代理
	IsSpecialAgent bool `gorm:"not null;default:false;"`
	// 此user的一級代理是誰
	TopUserID *uint `gorm:"not null;default:0;"`
	TopUser   *MarketMakerUser
	Level     uint `gorm:"not null;default:0"`
	// 通訊軟體平台
	Messager        string `gorm:"not null;type:char(30);"`
	MessagerAccount string `gorm:"not null;type:char(30);"`
	// 推薦人
	RecommenderID       *uint `gomr:"not null;defualt:0;"`
	Recommender         *MarketMakerUser
	RecommenderVerified bool `gorm:"not null;default:false;"`
	// 帳號開通
	Active bool `gorm:"not null;default:false;"`
	// // 暫時停用
	// Suspended bool `gorm:"not null;default:fasle;"`
	// 允許編輯銀行卡
	CanEditBankCard bool   `gorm:"not null;default:true;"`
	Phone           string `gorm:"not null;type:char(15);"`
	RegisteredIP    string `gorm:"not null;type:char(15);"`

	/////// 代收代付設定 ////////
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

	MarketMakerBankCardSettings []MarketMakerUserBankCardSetting
	common.CreatedAtAndUpdatedAt
}

// 造市商會員銀行卡交易限制
type MarketMakerUserBankCardSetting struct {
	common.ID
	MarketMakerUserID uint `gorm:"not null;default:0;"`
	// 代收 1 / 代付 2 / 代收付 3
	Type   uint   `gorm:"not null;default:0;"`
	Enable bool   `gorm:"not null;default:true;"`
	BankID uint   `gorm:"not null;default:0;"`
	Branch string `gorm:"not null;default:not null;type:char(20);"`
	Name   string `gorm:"not null;default:not null;type:char(20);"`
	// 單筆上限
	Limit uint `gorm:"not null;default:0;"`
	// 每日上限
	DayLimit uint `gorm:"not null;default:0;"`

	common.CreatedAtAndUpdatedAt
	common.Deleted
}

// ///// 錢包app ///////
// 錢包app使用者
type WalletUser struct {
	common.ID
	UserID       uint   `gorm:"not null;default:0;"`
	RegisteredIP string `gorm:"not null;type:char(15);"`
	// 娛樂城打綁定錢包api給我們的，對應他們會員的唯一值
	UUID     string `gorm:"type:uuid;unique;"`
	Verified bool   `gorm:"not null;default:false;"`
	// 交易密碼
	TransactionPassword string `gorm:"type:char(5)"`
	WalletUserVerify    WalletUserVerify
	WalletUserBankCards []WalletUserBankCard
	common.CreatedAtAndUpdatedAt
}

// 錢包app 實名 照片 ID card認證
type WalletUserVerify struct {
	common.ID
	WalletUserID uint
	RealName     string `gorm:"not null;type:char(20);"`
	IDCardNumber string `gorm:"not null;type:char(20);"`
	BirthDay     time.Time
	IDCardFront  common.Attachment `gorm:"embedded;"`
	IDCardBack   common.Attachment `gorm:"embedded;"`
	// 手持ID card照片
	UserIDCardPhoto common.Attachment `gorm:"embedded;"`
	// 待審核 已同意 已拒絕
	Status uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 錢包使用者 銀行卡
type WalletUserBankCard struct {
	common.ID
	WalletUserID uint       `gorm:"not null;defalt:0;"`
	WalletUser   WalletUser //`gorm:"reference:WalletUserID;"`
	BankID       uint       `gorm:"not null;default:0;"`
	Branch       string     `gorm:"not null;default:not null;type:char(20);"`
	Code         string     `gorm:"not null;type:char(20);"`
	// 新增銀行卡時不再填入名稱，直接只用實名驗證的名稱

	common.CreatedAtAndUpdatedAt
	common.Deleted
}

//////// 商戶管端 ////////////

// 商管腳色
type Role struct {
	common.ID
	// 集團管理員 商戶管理員 站長 開發人員 行銷人員
	Name string `gorm:"not null;type:char(20)"`
	common.CreatedAtAndUpdatedAt
}

// 商管帳號
type MerchantUser struct {
	common.ID
	UserID        uint `gorm:"not null;"`
	CorporationID uint `gorm:"not null;"`
	// 如商戶為null 則為此集團的跨商戶帳號
	MerchantID uint
	RoleID     uint `gorm:"not null;default:0;"`
	Role       Role
	// 	// 如果腳色是站長，且帳號為跨商戶的話，則要記錄所屬的所有站台
	MerchantUserStantionMasters []MerchantUserStantionMaster //`gorm:"foreignKey:MerchantUserRoleID;"`
	common.CreatedAtAndUpdatedAt
}

// todo 確認一個帳號是否有可能有多個腳色
// // 商管帳號 腳色 binding
// type MerchantUserRole struct {
// 	common.ID
// 	MerchantUserID uint `gorm:"idx_user_role"`
// 	RoleID         uint `gorm:"idx_user_role"`
// 	Role
// 	// 如果腳色是站長，且帳號為跨商戶的話，則要記錄所屬的所有站台
// 	MerchantUserStantionMasters []MerchantUserStantionMaster `gorm:"foreignKey:MerchantUserRoleID;"`
// 	common.CreatedAtAndUpdatedAt
// }

// 記錄跨商戶站長資料
type MerchantUserStantionMaster struct {
	common.ID
	MerchantUserID uint `gorm:"not null;default:0;"`
	MerchantID     uint `gorm:"not null;default:0;"`
	common.CreatedAt
}

// // //// 商戶 遊戲腳色 ///////
// type MerchantGameUser struct {
// 	common.ID
// 	MerchantID uint   `gorm:"not null;"`
// 	UserID     uint   `gorm:"not null;"`
// 	WalletID   string `gorm:"not null;"`
// 	common.CreatedAtAndUpdatedAt
// }
