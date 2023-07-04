package log

import "appserver/src/database/new/common"

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
	// todo 資料來源的db 跟table id
	// 事項
	Item string `gorm:"not null;type:varchar(30);"`
	// 事由
	Reason string `gorm:"not null;type:varchar(30);"`
	// 1 用戶申請 / 2 管理者同意 / 3 管理者拒絕 / 4 申請人取消
	Status uint
	// todo 移除
	// 回調
	// CallBackCount uint
	// // todo 尚未知作用
	// CallbackStatuse bool
	// CallbackLog     string `gorm:"varchar(256);default:null;"`
	// 附檔 url
	common.Attachment
	// 申請者
	common.Applicant
	// 審核人
	common.Approver
	common.CreateAtAndUpdateAt
}
