package common

import "time"

type CreatedAt struct {
	CreatedAt time.Time
}

type CreateAtAndUpdateAt struct {
	CreatedAt
	UpdatedAt time.Time
}

type DeletedAt struct {
	DeletedAt time.Time
}

type Deleted struct {
	Deleted bool `gorm:"not null;default:false;"`
	DeletedAt
}

type Enable struct {
	Enable bool `gorm:"not null;default:false;"`
}

type ID struct {
	ID uint `gorm:"primaryKey"`
}

type UUID struct {
	ID string `gorm:"type:uuid;default:UUID();not null"`
}

type Operator struct {
	OperatorID uint
	// User       User `gorm:"foreignKey:OperatorID"`
}

type Approver struct {
	ApproverID uint
	// User       User `gorm:"foreignKey:ApproverID"`
}

type Applicant struct {
	// todo 是否改成只存public Key
	ApplicantID uint
	// User        User `gorm:"foreignKey:ApplicantID"`
}

type IP struct {
	IP string `gorm:"not null;type:varchar(15)"`
}

type NameBasic struct {
	Nmae string `gorm:"primaryKey;type:varchar(30); not null;"`
	CreateAtAndUpdateAt
}

type Key struct {
	Key string `gorm:"not null;varchar(256)"`
}

type Group struct {
	Name  string `gorm:"unique;not null;type:varchar(30)"`
	Level uint
	Note  string `gorm:"type:varchar(30);default:null"`
	// 這個群組可以用的造市商控端功能列表，json與bit flag格式
	FunctionSetting string `gorm:"not null;type:json;"`
}
