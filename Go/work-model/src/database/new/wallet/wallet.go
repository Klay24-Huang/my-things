package wallet

import (
	"appserver/src/database/new/common"
	"time"
)

/////////// 放錢包控端 跟 app相關東西 並非錢包本身///////////

// 控端銀行
type Bank struct {
	common.ID
	// todo web有填入銀行ID的地方 有需要保留?
	BankID   string `gorm:"unique;not null;type:char(30);"`
	BankName string `gorm:"unique;not null;type:char(30);"`
	// 待審核 / 已通過 / 已拒絕
	Status uint `gorm:"not null;"`
	Enable bool `gorm:"not null;default:true;"`
	common.Applicant
	common.CreatedAtAndUpdatedAt
}

// 阻擋註冊IP
type RegistrationIPBlocking struct {
	IP string `gorm:"primaryKey;type:char(15);"`
	common.CreatedAtAndUpdatedAt
}

// todo: 鎖單限制? 感覺不合理
type SystemSetting struct {
	common.ID
	// 掛單數量限制
	OrderLimit uint `gorm:"not null;default:0;"`
	// 鎖單數量限制
	LockedOrderLimit uint `gorm:"not null;default:0;"`
	// 取消數量限制
	CanceledOrderLimit uint `gorm:"not null;default:0;"`
	// 手續費
	Fee         uint `gorm:"not null;default:0;"`
	AutoMatched bool `gorm:"not null;default:true;"`
	common.CreatedAtAndUpdatedAt
}

// 同時吃單上限
// todo 達到吃單上限時 會不交易嗎?
type TradeSetting struct {
	common.ID
	// 總代理 / 代理 / 造市商 / 自然人 / 商戶
	Type int `gorm:"not null;default:0;"`
	Call int `gorm:"not null;default:0;"`
	Put  int `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 拆單數量
type SeparatedBill struct {
	Number uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 錢包 獎勵活動
type Activity struct {
	common.ID
	Name      string    `gorm:"not null;type:char(50);"`
	StartedAt time.Time `gorm:"not null;"`
	EndedAt   time.Time `gorm:"not null;"`
	// 活動消息標題
	Title   string `gorm:"not null;type:char(50);"`
	Content string `gorm:"not null;type:text;"`
	// todo 額度設定 領取內容
	common.CreatedAtAndUpdatedAt
}

// 公告
type Bulletin struct {
	common.ID
	Title    string `gorm:"notn null;type:char(50);"`
	Content  string `gorm:"not null;type:text;"`
	Verified bool   `gorm:"defaut:false;not null"`
	common.Attachment
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// todo 公告內容 個人消息 是跟個人推撥一樣嗎?
