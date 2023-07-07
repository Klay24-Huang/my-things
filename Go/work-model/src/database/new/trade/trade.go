package trade

import (
	"appserver/src/database/new/common"
	"time"
)

// 幣種
type CoinType struct {
	CoinType string `gorm:"not null;char(5);"`
}

// 產生/銷毀 代幣
type CentralBank struct {
	common.ID
	// 造幣 create / 回收 delete
	Action string `gorm:"not null;char(5)"`
	// 幣種
	CoinType
	UserID uint
	common.CreatedAt
}

// ////////////////// trade ///////////////////////
type Order struct {
	// todo 銀行卡
	common.ID
	UserID uint
	// 買單 call 1 / 賣單 put 2
	Type int `gorm:"not null;"`
	// 拆單
	// todo 要記成percent? 或記成最小拆單金額 畢竟拆單數量可在控端更改
	// todo 拆單細節邏輯考慮
	Splittable bool `gorm:"not null;default:false"`
	// 所有拆單前最初的訂單
	// 要記最上層的訂單
	TopOrderID *uint
	TopOrder   *Order
	// 幣種
	CoinType
	// 鎖定中 部分切單交易中
	Locked bool `gorm:"not null;default:false;"`
	// 排序值 優先值 越大越優先
	Priority uint `gorm:"not null;defalt:0;"`
	// 下單數量
	Amount uint
	// 手續費
	Fee float32
	// 鎖單時間
	LockedAt time.Time
	common.CreatedAt
}

// 搓合成功訂單 交易紀錄
type Trade struct {
	common.UUID
	// // 開單前最初的交易ID
	TopTradeID *uint
	TopTrade   *Trade
	CallID     uint  `gorm:"index:idx_call_put;"`
	CallOrder  Order `gorm:"foreignKey:CallID;"`
	PutID      uint  `gorm:"index:idx_call_put;"`
	PutOrder   Order `gorm:"foreignKey:PutID;"`
	// 代付款(進行中) / 已取消 / 已完成 / 爭議 / 自動取消 / 暫停交易 / 標記
	Status int `gorm:"not null;"`
	// 付款照片url
	PaymentUrl string `gorm:"char(30);"`
	// 付款持間
	PaidAt time.Time
	// 沖正
	// todo 確認 是否有 商戶 造市商的收發幣的沖正?
	Reversal bool `gorm:"defalt:false;"`
	common.CreatedAtAndUpdatedAt
	// todo 銀行卡匹配次數上限
}

// /// market maker /////

// /// 錢包相關 //////
// 錢包主體
type Wallet struct {
	common.UUID
	UserID uint
	common.CreatedAtAndUpdatedAt
}

// 錢包細節
type WalletDetail struct {
	common.ID
	WalletID string
	Wallet
	CoinType
	// 總餘額
	Balance int
	// 留存
	// 下訂單的時候暫時要把金額放進來，避免下單金額超過錢包餘額
	Retain int

	MerchantWalletDetail
	MarketMakerWalletDetail
	common.CreatedAtAndUpdatedAt
}

// 商戶錢包的特殊的統計
type MerchantWalletDetail struct {
	common.ID
	WalletDetailID uint
	// todo 未全 商控 商戶錢包管理所有欄位
	// 總入款
	Desposit int
	// 總入款手續費
	DespositFee int
	// 總出款
	Withdraw int
	// 總出款手續費
	WithdrawFee int
	// 凍結
	Freeze int
	common.CreatedAtAndUpdatedAt
}

// 造市商錢包的特殊統計
type MarketMakerWalletDetail struct {
	common.ID
	WalletDetail uint
	// 團隊累計獎金
	CroupBonus float32
	// 個人累計獎金
	TotalBonus float32
	common.CreatedAtAndUpdatedAt
}
