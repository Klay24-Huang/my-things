package log

import (
	"appserver/src/database/new/common"
	"time"
)

// 審核類型
type Type struct {
	common.ID
	SystemType common.Type
	Name       string `gorm:"not null;type:char(20)"`
	common.CreateAtAndUpdateAt
}

// 審核標題
type Title struct {
	common.ID
	Name   string `gorm:"not null;type:char(20)"`
	TypeID uint   `gorm:"not null;"`
	Type
	Level uint `gorm:"not null;"`
	common.CreateAtAndUpdateAt
}

// 審核列表
type Verify struct {
	common.ID
	// 標題
	TitleID uint
	Title
	// 事項
	Item string `gorm:"not null;type:char(30);"`
	// 事由
	Reason string `gorm:"not null;type:char(30);"`
	// 修改前資料明細 json
	Before string `gorm:"not null;type:json"`
	// 修改後資料明細 json
	After string `gorm:"not null;type:json"`

	// 1 用戶申請 / 2 管理者同意 / 3 管理者拒絕 / 4 申請人取消
	Status uint
	// 附檔 url
	common.Attachment
	// 申請者
	common.Applicant
	// 審核人
	common.Approver
	// 當審核通過 直接只用下方資訊修改資料內容
	Database string `gorm:"not null;type:char(10);"`
	Table    string `gorm:"not null;type:char(10);"`
	DataID   uint   `gorm:"not null;"`
	common.CreateAtAndUpdateAt
}

// 商戶管理IP白名單紀錄
type MerchantWhitelistingLog struct {
	common.ID
	MerchantID uint

	common.CreatedAt
}

// TODO 研究丟到cloud watch or 其他log db
// TODO 下方所有log 是否要在收斂成一個類別 用type去切換
type MerchantPasswordResetLog struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	common.Operator
	common.CreatedAt
}

type UnbindMerchantPhoneLog struct {
	common.ID
	MerchantID uint   `gorm:"not null;"`
	Phone      string `gorm:"type:char(15);default:null;"`
	common.Operator
	common.CreatedAt
}

// 控端帳號鎖定
type AccountLockLog struct {
	common.ID
	LockTime time.Time
	UserID   uint   `gorm:"not null;"`
	Reason   string `gorm:"type:char(30);not null;"`
	common.CreateAtAndUpdateAt
}

// 管端帳號鎖定
type MerchantLockLog struct {
	common.ID
	LockTime   time.Time
	MerchantID uint `gorm:"not null;"`
	common.CreateAtAndUpdateAt
}

// // google 驗證操作紀錄
type MerchantOTPLog struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	// 操作內容 啟用/停用 2fa
	Operation string `gorm:"not null;type:char(30);"`
	common.Operator
}
