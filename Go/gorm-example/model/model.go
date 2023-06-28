package model

import (
	"time"
)

type CreateAt struct {
	CreateAt time.Time
}

type CreateAtAndUpdateAt struct {
	CreateAt
	UpdatedAt time.Time
}

type ID struct {
	ID uint `gorm:"primaryKey"`
}

type UUID struct {
	ID string `gorm:"type:uuid;default:UUID();not null"`
}

type Operator struct {
	OperatorId uint
	User       User `gorm:"foreignKey:OperatorId"`
}

type IP struct {
	IP string `gorm:"not null;type:varchar(15)"`
}

type OperationBasic struct {
	Nmae string `gorm:"primaryKey;type:varchar(30); not null;"`
	CreateAtAndUpdateAt
}

type User struct {
	ID
	Account  string `gorm:"unique;not null;type:varchar(30)"`
	Password string `gorm:"not null;type:varchar(30)"`
	Note     string `gorm:"default:null;type:varchar(30)"`
	GroupId  uint
	Group    Group
	//  密鑰?
	OtpEnable   bool   `gorm:"default:false;not null"`
	OtpVerified bool   `gorm:"default:false;not null"`
	OtpSecret   string `gorm:"default:null"`
	OtpAuth_url string `gorm:"default:null"`
	CreateAtAndUpdateAt
}

// 群組
type Group struct {
	ID
	Name  string `gorm:"unique;not null;type:varchar(30)"`
	Level uint
	Note  string `gorm:"type:varchar(30);default:null"`
	CreateAtAndUpdateAt
}

// 集團
type Corporation struct {
	UUID
	Name     string `gorm:"unique;not null;type:varchar(20)"`
	LoginUre string `gorm:"unique;not null;type:varchar(30)"`
	Verified bool   `gorm:"defaut:false;not null"`
	Enable   bool   `gorm:"default:false;not null"`
	// 管端登入ip限制
	LimitLoginIP bool   `gorm:"default:true;not null"`
	Note         string `gorm:"type:varchar(30);default:null"`
	// todo domain name
	CreateAtAndUpdateAt
}

// 集團管端登入ip白名單
type CorporationWhitelistring struct {
	CorporationId string `gorm:"type:uuid;not null;uniqueIndex:corp_id_ip"`
	Corporation   Corporation
	IP            string `gorm:"type:varchar(15);not null;uniqueIndex:corp_id_ip"`
	//狀態 1新增 / 0刪除
	Statue bool `gorm:"default:true"`
	// 操作者ID

	// TODO: 出處有必要? 編輯or新增
	// TODO: 需要記錄成新增者和刪除者?
	CreateAtAndUpdateAt
}

// 商戶
type Merchant struct {
	ID
	CorperationId uint `gorm:"not null;"`
	Corporation
	WalletId uint `gorm:"not null;"`
	Wallet
	Name  string `gorm:"type:varchar(20);not null;"`
	Phone string `gorm:"type:varchar(15);default:null;"`
	// 掛單出售Y幣
	Sell     bool `gorm:"default:true;not null"`
	Transfer bool `gorm:"default:true;not null"`
	// 控端 url
	ConsoleUrl string `gorm:"not null;type:varchar(50)"`
	Url        string `gorm:"not null;type:varchar(50)"`
	// test url
	TestUrl  string `gorm:"not null;type:varchar(50)"`
	Verified bool   `gorm:"defaut:false;not null"`
	Enable   bool   `gorm:"default:false;not null"`
	CreateAtAndUpdateAt
}

type Domain struct {
	ID
	Name string `gorm:"not null;type:varchar(50)"`
	CreateAtAndUpdateAt
}

type LoginLog struct {
	ID
	UserId int `gorm:"not null;"`
	User
	// 登入平台
	Application string `gorm:"not null;type:varchar(10)"`
	IP
	// login statuse 成功 / 失敗
	Statue bool `gorm:"not null;"`
	CreateAtAndUpdateAt
}

type MerchantPasswordResetLog struct {
	ID
	MerchantId uint `gorm:"not null;"`
	Merchant
	Operator
	CreateAt
}

type UnbindMerchantPhoneLog struct {
	ID
	MerchantId uint `gorm:"not null;"`
	Merchant
	Phone string `gorm:"type:varchar(15);default:null;"`
	Operator
	CreateAt
}

type AccountLockLog struct {
	LockTime time.Time
	UserId   uint `gorm:"not null;"`
	User
	Locked bool   `gorm:"default:false;not null;"`
	Reason string `gorm:"type:varchar(30);not null;"`
	// todo operator?
	CreateAtAndUpdateAt
}

type MerchantLockLog struct {
	LockTime   time.Time
	MerchantId uint `gorm:"not null;"`
	Merchant
	// todo operator?
	CreateAtAndUpdateAt
}

// google 驗證操作紀錄
type MerchantOTPLog struct {
	MerchantId uint `gorm:"not null;"`
	Merchant
	// 操作內容 啟用/停用 2fa
	Operation string `gorm:"not null;type:varchar(30);"`
	Operator
}

type Boardcast struct {
	MerchantId uint `gorm:"not null;"`
	Merchant
	// todo 補幣/ 補Y幣 差別?
	//商戶 補幣審核通過 / Y幣下發審核通過 / Y幣補幣審核通過
	Flags uint
	// operator?
	CreateAtAndUpdateAt
}

type OperatorItem struct {
	OperationBasic
}

type OperationDetail struct {
	OperationBasic
}

type OperationLog struct {
	Operator
	OperatorItem
	OperationDetail
	Content string `gorm:"type:varchar(30);not null;"`
	IP
	CreateAt
}

type Wallet struct {
	UUID
	Balance int
	CreateAtAndUpdateAt
}

type MerchantWalletSetting struct {
	// 商戶錢包管理
	//todo 帶商戶管端開完 再回來會比較清楚
}

// yapay 錢包會員
type WalletUser struct {
	ID
	Name string `gorm:"not null;type:varchar(30);"`
	CreateAtAndUpdateAt
}

type MerchantBindWalletUser struct {
	MerchantId uint `gorm:"not null;"`
	Merchant
	WalletUserId uint
	WalletUser
	Bingding bool `gorm:"not null;default:true;"`
	// todo 出入款紀錄
	// todo 解除再綁定的話 出入款金額的計算方法
	CreateAtAndUpdateAt
}
