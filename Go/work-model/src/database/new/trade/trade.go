package trade

import (
	"appserver/src/database/new/common"
	"time"
)

////////////// Central Bank //////////////

// 幣種
type CoinType struct {
	CoinType string `gorm:"primaryKey;not null;char(16);"`
	// address coin id code
}

// ////////////////// trade ///////////////////////
type Order struct {
	common.UUID
	UserID uint `gorm:"not null;default:0;"`

	// todo 是否放使用者名稱和銀行卡相關資訊
	UserName   string `gorm:"not null;"`
	BankID     uint   `gorm:"not null;default:0;"`
	BankName   string `gorm:"not null;default:0;"`
	BranchName string `gorm:"not null;"`

	// 買單 call 1 / 賣單 put 2
	Type uint `gorm:"not null;default:0;"`
	// 幣種
	CoinType
	// 排序值 優先值 越大越優先
	Priority uint `gorm:"not null;default:0;"`
	// 下單數量
	Amount uint `gorm:"not null;default:0;"`
	// 最小下單數量，比如下單1000，目前系統設定最多切五單，1000 / 5 = 200
	MinimumAmount uint `gorm:"not null;default:0;"`
	// 手續費
	Fee float32 `gorm:"not null;default:0;"`
	// 進行中 / 完成 / 取消
	Status uint `gorm:"not null;default:0;"`

	//// 付款 買單才有 /////
	// 付款照片url
	PaymentUrl string `gorm:"char(30);"`
	// 付款持間
	PaidAt time.Time
	common.CreatedAt
}

// lock table，媒合成功 等待後續變化
type LockedOrder struct {
	common.ID
	CallID    uint  `gorm:"index:idx_call_put;"`
	CallOrder Order `gorm:"foreignKey:CallID;"`
	PutID     uint  `gorm:"index:idx_call_put;"`
	PutOrder  Order `gorm:"foreignKey:PutID;"`
	common.CreatedAt
}

// 搓合成功訂單 交易紀錄，只放交易成功的紀錄
type Transaction struct {
	common.UUID
	CallID    uint  `gorm:"index:idx_call_put;"`
	CallOrder Order `gorm:"foreignKey:CallID;"`
	PutID     uint  `gorm:"index:idx_call_put;"`
	PutOrder  Order `gorm:"foreignKey:PutID;"`
	common.CreatedAtAndUpdatedAt
}

// 取消交易的table
type CanceledTransaction struct {
	common.ID
	CallID    uint  `gorm:"index:idx_call_put;"`
	CallOrder Order `gorm:"foreignKey:CallID;"`
	PutID     uint  `gorm:"index:idx_call_put;"`
	PutOrder  Order `gorm:"foreignKey:PutID;"`
	common.CreatedAt
}

// 所有訂單狀態
// // 代付款(進行中) / 已取消 / 已完成 / 爭議 / 自動取消 / 暫停交易 / 標記
// Status int `gorm:"not null;"`

// /// market maker /////

// /// 錢包相關 //////
// 錢包主體
type Wallet struct {
	common.UUID
	UserID uint `gorm:"not null;default:0;"`
	// todo 是否把商戶的錢包 直接綁在商戶控端的admin帳號上
	MerchantID uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 錢包細節
type WalletDetail struct {
	common.ID
	WalletID string `gorm:"not null;"`
	Wallet   Wallet
	CoinType
	common.CreatedAtAndUpdatedAt
}

// 要及時算 先記錄欄位
// 商戶錢包的特殊的統計
// type MerchantWalletDetail struct {
// 	// 要及時算
// 	// common.ID
// 	// WalletDetailID uint
// 	// // todo 未全 商控 商戶錢包管理所有欄位
// 	// // 總入款
// 	// Desposit int
// 	// // 總入款手續費
// 	// DespositFee int
// 	// // 總出款
// 	// Withdraw int
// 	// // 總出款手續費
// 	// WithdrawFee int
// 	// // 凍結
// 	// Freeze int
// 	// common.CreatedAtAndUpdatedAt
// }

// // 造市商錢包的特殊統計
// type MarketMakerWalletDetail struct {
// 	// in memory 及時
// 	// common.ID
// 	// WalletDetail uint
// 	// // 團隊累計獎金
// 	// CroupBonus float32
// 	// // 個人累計獎金
// 	// TotalBonus float32
// 	// common.CreatedAtAndUpdatedAt
// }
