package marketmaker

import "appserver/src/database/new/common"

// ////////////////// 造市商控端 ///////////////////////

// 目前沒有 先保留
// // 造市商 任務
// type Task struct {
// 	common.ID
// 	// 代收 call 1 / 代付 put 2
// 	Type uint `gorm:"not null;default:0;"`
// 	// uuid yapay訂單id
// 	TradeID string
// 	// 總獎金
// 	Amount float32
// 	common.CreatedAt
// }

// // 任務獎勵
// type Bounus struct {
// 	common.ID
// 	UserID uint
// 	TaskID uint
// 	Task
// 	Ratio  float32
// 	Amount float32
// 	common.CreatedAt
// }

////// 造市商 設定相關 ///////

// 額度上限設定
type QuotaSetting struct {
	common.ID
	// 代收
	CollectionDayLimit  uint `gorm:"not null;default:0;"`
	CollectionOnceLimit uint `gorm:"not null;default:0;"`
	// 代付
	PayingDayLimit  uint `gorm:"not null;default:0;"`
	PayingOnceLimit uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 獎金比例設定
type BonusSetting struct {
	common.ID
	// 代收獎金比例
	CollectionRatio float32 `gorm:"not null;default:0;"`
	// 代付獎金比例
	PayingRatio float32 `gorm:"not null;default:0;"`

	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 銀行卡交易次數上限
type TradeSetting struct {
	common.ID
	Count uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 帳號代收代付上限
type AccountQuotaSetting struct {
	common.ID
	// 每日代收上限
	DayCollectionLimit uint `gorm:"not null;default:0;"`
	// 每日代付上限
	DayPayingLimit uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 媒合設定
type MatchSetting struct {
	common.ID
	// 每日同銀行卡片匹配次數
	DayLimitOfSameBank uint `gorm:"not null;default:0;"`
	// 同步錢包訂單數量
	SyncWalletOrder uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 使用者服務設定
type UserAgreementSetting struct {
	common.ID
	Content string
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 溫馨提醒設定
type RemindSetting struct {
	common.ID
	Content string
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 注意事項設定
type PleaseNoteSetting struct {
	common.ID
	Content string
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// IOS 簽證 設定
type IOSSignatureSetting struct {
	common.ID
	// 企業簽 超級簽
	Type uint   `gorm:"not null;default:0;"`
	Name string `gorm:"not null;type:char(30);"`
	// 載點
	Url    string `gorm:"not null;type:char(50);"`
	Note   string `gorm:"typechar(50);"`
	Enable bool   `gorm:"not null;default:true"`
	// todo 優先權 目前看起來沒作用
	common.Operator
	common.CreatedAtAndUpdatedAt
}
