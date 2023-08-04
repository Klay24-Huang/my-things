package trade

import (
	"appserver/src/database/new/common"
	"time"

	"github.com/shopspring/decimal"
)

////////////// Central Bank //////////////

// 幣種
type CoinType struct {
	CoinType string `gorm:"primaryKey;not null;char(16);"`
	// todo 區塊鍊相關欄位 address coin id code
}

// ////////////////// trade ///////////////////////
type Order struct {
	common.UUID
	UserID     uint   `gorm:"not null;default:0;"`
	UserName   string `gorm:"not null;"`
	BankID     uint   `gorm:"not null;default:0;"`
	BankName   string `gorm:"not null;;"`
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
	// null時表示可拆單
	MinimumAmount uint
	// 未成交的餘額
	RemainingAmount uint
	// 手續費
	Fee decimal.Decimal `gorm:"not null;default:0;"`
	// 同時記錄拆單是否被處理完
	// 進行中 / 完成 / 取消
	Status uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// lock table，媒合成功 等待後續變化
type LockedOrder struct {
	common.ID
	BuyerOrderID uint  `gorm:"index:idx_buy_sell;"`
	BuyerOrder   Order `gorm:"foreignKey:BuyerOrderID;"`
	//// 付款 買單才有 /////
	// 付款照片url 路徑會加密
	PaymentUrl string
	// 付款持間
	PaidAt        time.Time
	SellerOrderID uint  `gorm:"index:idx_buy_sell;"`
	SellerOrder   Order `gorm:"foreignKey:SellerOrderID;"`
	// 未付款 / 已付款(賣方要確認才能放款)
	Staute uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 非帳本 只是交易紀錄
// 搓合成功的訂單，只放交易成功的紀錄
type Transaction struct {
	common.UUID
	BuyerOrderID uint  `gorm:"index:idx_buy_sell;"`
	BuyerOrder   Order `gorm:"foreignKey:BuyerOrderID;"`
	//// 付款 買單才有 /////
	// 付款照片url 路徑會加密
	PaymentUrl string
	// 付款持間
	PaidAt        time.Time
	SellerOrderID uint  `gorm:"index:idx_buy_sell;"`
	SellerOrder   Order `gorm:"foreignKey:SellOrderID;"`
	// 帳本id
	LedgerID uint
	common.CreatedAtAndUpdatedAt
}

// 取消交易的table
type CanceledTransaction struct {
	common.ID
	BuyerOrderID  uint  `gorm:"index:idx_buy_sell;"`
	BuyerOrder    Order `gorm:"foreignKey:BuyerOrderID;"`
	SellerOrderID uint  `gorm:"index:idx_buy_sell;"`
	SellerOrder   Order `gorm:"foreignKey:SellerOrderID;"`
	//  已取消 /自動取消
	Type uint
	common.CreatedAt
}

// 所有訂單狀態
// // 代付款(進行中) / 已取消 / 已完成 / 爭議 / 自動取消 / 暫停交易 / 標記

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

// 總帳本
type Ledger struct {
	common.ID
	PayerWalletID    string `type:"uuid;"`
	ReceiverWalletID string `type:"uuid;"`
	Amount           decimal.Decimal
	// todo 發行者的錢包跟回收錢包要寫死在程式裡
	// 交易種類 ex:發行 回收 冲正 交易
	Type uint
	// 冲正的id
	ReversalID uint
	// 紀錄冲正的原因
	Note string
	common.CreatedAt
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
