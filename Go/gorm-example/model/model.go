package model

import (
	"time"

	"gorm.io/gorm"
)

type ModelTime struct {
	ID        uint `gorm:"primaryKey"`
	CreatedAt time.Time
	UpdatedAt time.Time
}

type ModelUintId struct {
	ID uint `gorm:"primaryKey"`
}

type ModelUUID struct {
	ID string `gorm:"type:uuid;default:UUID();not null"`
}

type User struct {
	gorm.Model
	Account  string `gorm:"unique;not null;type:varchar(30)"`
	Password string `gorm:"not null;type:varchar(30)"`
	Note     string `gorm:"default:null;type:varchar(30)"`
	GroupId  uint
	Group    Group
	//  密鑰?

	OtpEnable   bool   `gorm:"default:false;"`
	OtpVerified bool   `gorm:"default:false"`
	OtpSecret   string `gorm:"default:null"`
	OtpAuth_url string `gorm:"default:null"`

	LastLogin time.Time `gorm:"default:null"`
}

// 群組
type Group struct {
	gorm.Model
	Name  string `gorm:"unique;not null;type:varchar(30)"`
	Level uint
	Note  string `gorm:"type:varchar(30);default:null"`
}

// 集團
type Corporation struct {
	gorm.Model
	CorpId   string `gorm:"type:uuid;default:UUID();not null"`
	Name     string `gorm:"unique;not null;type:varchar(20)"`
	LoginUre string `gorm:"unique;not null;type:varchar(30)"`
	Verified bool   `gorm:"defaut:false;not null"`
	Enable   bool   `gorm:"default:false;not null"`
	// 管端登入ip限制
	LimitLoginIP bool   `gorm:"default:true;not null"`
	Note         string `gorm:"type:varchar(30);default:null"`
	// todo domain name
}

// 集團管端登入ip白名單
type CorporationWhitelistring struct {
	gorm.Model
	CorporationId string `gorm:"type:uuid;not null;uniqueIndex:corp_id_ip"`
	Corporation   Corporation
	IP            string `gorm:"type:varchar(15);not null;uniqueIndex:corp_id_ip"`
	//狀態 1新增 / 0刪除
	Statue bool `gorm:"default:true"`
	// 操作者ID
	OperatorId uint
	User       User `gorm:"foreignKey:OperatorId"`
	// TODO: 出處有必要? 編輯or新增
	// TODO: 需要記錄成新增者和刪除者?

}

// 商戶
type Merchant struct {
	ModelUintId
	CorperationId uint
	Corporation
	Name string `gorm:"type:varchar(20);not null;"`
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
}

type Domain struct {
	ModelUintId
	Name string `gorm:"not null;type:varchar(50)"`
}

type LoginLog struct {
	ModelUintId
	UserId int
	User
	// 登入平台
	Application string `gorm:"not null;type:varchar(10)"`
	IP          string `gorm:"not null;type:varchar(15)"`
	// login statuse 成功 / 失敗
	Statue bool
}
