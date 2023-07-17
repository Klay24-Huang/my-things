package user

import (
	"gorm-example/model/common"
)

type User struct {
	common.ID
	Account  string `gorm:"unique;not null;type:varchar(30)"`
	Password string `gorm:"not null;type:varchar(30)"`
	Name     string `gorm:"not null;type:varchar(30);"`
	Note     string `gorm:"default:null;type:varchar(30)"`
	// GroupID  uint
	// Group    merchant.MerchantGroup
	// 商戶管端 / 商戶控端 / 錢包管端 / 錢包user / 造市商管端 / 造市商user
	Type uint `gorm:"not null;"`
	//  密鑰?
	OtpEnable   bool   `gorm:"default:false;not null"`
	OtpVerified bool   `gorm:"default:false;not null"`
	OtpSecret   string `gorm:"default:null"`
	OtpAuth_url string `gorm:"default:null"`
	common.Enable
	common.CreateAtAndUpdateAt
	common.Deleted

	PublicKeys []PublicKey
}

type PublicKey struct {
	Key    string `gorm:"primaryKey,not null;type:varchar(256);"`
	UserID uint
	common.CreatedAt
}

// 登入錯誤次數過多被鎖定
type LockedUser struct {
	common.ID
	UserID uint
	User
	common.CreatedAt
	common.Deleted
}

// 登入紀錄
// 商戶控端 管端都有
type LoginLog struct {
	common.ID
	UserID uint `gorm:"not null;"`
	User
	// 登入平台
	Application string `gorm:"not null;type:varchar(10)"`
	common.IP
	// login statuse 成功 / 失敗
	Statue bool `gorm:"not null;"`
	common.CreatedAt
}

// app user
type MarketMakerUser struct {
	common.ID
	UserID uint `gorm:"not null;"`
	User
	SpecialAgent bool `gorm:"not null;default:false;"`
	Level        uint `gorm:"not null;"`
	// 通訊軟體平台
	Messager        string `gorm:"not null;type:varchar(30);"`
	MessagerAccount string `gorm:"not null;type:varchar(30);"`
	// 推薦人
	RecommenderID       *uint
	Recommender         *MarketMakerUser
	RecommenderVerified bool `gorm:"not null;default:false;"`
	// 帳號開通
	Active    bool `gorm:"not null;default:false;"`
	Suspended bool `gorm:"not null;default:fasle;"`
	// 允許編輯銀行卡
	EditBankCard bool   `gorm:"not null;default:true;"`
	Phone        string `gorm:"not null;type:varchar(15);"`
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

	common.CreateAtAndUpdateAt
}

type MarketMakerBankCardSetting struct {
	common.ID
	// 代收 1 / 代付 2
	Type   uint   `gorm:"not null;"`
	Enable bool   `gorm:"not null;default:true;"`
	Bank   string `gorm:"not null;default:not null;type:varchar(20);"`
	Branch string `gorm:"not null;default:not null;type:varchar(20);"`
	Name   string `gorm:"not null;default:not null;type:varchar(20);"`
	// 單筆上限
	Limit uint `gorm:"not null;"`
	// 每日上限
	DayLimit uint `gorm:"not null;"`

	common.CreateAtAndUpdateAt
	common.Deleted
}
