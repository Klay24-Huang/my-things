package trade

import (
	"appserver/src/database/new/common"
	"time"
)

// ////////////////// trade ///////////////////////
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
	common.Key
	common.CreatedAt
}

type Order struct {
	// todo 銀行卡
	common.ID
	common.Key
	// 買單 call 1 / 賣單 put 2
	Type int `gorm:"not null;"`
	// 拆單
	// todo 要記成percent?
	// todo 拆單細節邏輯考慮
	Splittable bool `gorm:"not null;default:false"`
	// 所有拆單前最初的訂單
	// 要記最上層的訂單
	HighestOrderID *uint
	HighestOrder   *Order
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
	HighestTradeID *uint
	HighestTrade   *Trade
	CallID         uint  `gorm:"index:idx_call_put;"`
	CallOrder      Order `gorm:"foreignKey:CallID;"`
	PutID          uint  `gorm:"index:idx_call_put;"`
	PutOrder       Order `gorm:"foreignKey:PutID;"`
	// 代付款(進行中) / 已取消 / 已完成 / 爭議 / 自動取消 / 暫停交易 / 標記
	Status int `gorm:"not null;"`
	// 付款照片url
	PaymentUrl string `gorm:"char(30);"`
	// 付款持間
	PaidAt time.Time
	// 沖正
	Reversal
	common.CreateAtAndUpdateAt
	// todo 銀行卡匹配次數上限
}

// todo 確認 是否有 商戶 造市商的收發幣的沖正?
// 訂單沖正
type Reversal struct {
	// todo 會有部分金額?
	common.ID
	TradeID string `gorm:"type:uuid;"`
	common.CreatedAt
}

// 造市商收補幣
type MarketMakerSupplementOrRetract struct {
	common.ID
	// 補幣 supplement 1 / 回收 retract 2
	Type         int `gorm:"not null;"`
	ApplicantKey common.Key
	ApproverKey  common.Key
	Title        string `gorm:"char(30);"`
	// 事項
	Content string `gorm:"char(30);"`
	Reason  string `gorm:"char(30);"`
	common.CreateAtAndUpdateAt
}

// 錢包主體
type Wallet struct {
	common.UUID
	UserID uint
	common.CreateAtAndUpdateAt
}

// 錢包細節
type WalletDetail struct {
	common.ID
	WalletID string
	Wallet
	CoinType
	// 總餘額
	Balance int

	// todo 未全 商控 商戶錢包管理所有欄位
	// // 總入款
	// Desposit int
	// // 總入款手續費
	// DespositFee int
	// // 總出款
	// Withdraw int
	// // 總出款手續費
	// WithdrawFee int
	// // 凍結
	// Freeze int

	common.CreateAtAndUpdateAt
}

// ///// 錢包控端 ///////
// 補幣 / 收幣
type WalletConsoleRefillAndRecycleLog struct {
	common.ID
	WalletID string
	Wallet
	CoinType
	Amount uint `gorm:"not null;"`
	common.CreatedAt
}

// 回收代幣
type WalletConsoleRecycleLog struct {
	common.ID
	WalletID string
	Wallet
	CoinType
	// 申請回收金額
	Amount int
	// 實際回收金額
	RecycleAmount int
	// 手續費
	Fee int
	common.CreatedAt
}

////// merchant ////////

// 入款(會員存款) 紀錄
type DespositAndWithdrawLog struct {
	common.ID
	// 入款 / 出款
	Type         uint `gorm:"not null;"`
	UserID       uint
	UserWalletID uint
	UserWallet   Wallet `gorm:"references:UerWalletID;"`
	// 手續費率
	FeeRatio uint `gorm:"not null;"`
	// 實收手續費
	Fee              uint `gorm:"not null;"`
	MerchantWalletID uint
	MerchantWallet   Wallet `gorm:"references:MerchantWalletID;"`
	Amount           uint   `gorm:"not null;"`
	// 成功 / 用戶未付款 / 已付款 開分失敗
	Status uint `gorm:"not null;"`
	common.CreatedAt
	CompletedAt time.Time
}

// 商戶補幣
type MerchantRefillLog struct {
	common.ID
	WalletID string
	Wallet
	CoinType
	// 補幣金額
	Amount int
	// 手續費
	Fee int
	common.CreatedAt
}

// 申請補發x幣
type MerchantSupplyLog struct {
	common.ID
	WalletID string
	Wallet
	CoinType
	// 補幣金額
	Amount int
	common.CreatedAt
}

// 商戶交收
type MerchantSettlementLog struct {
	common.ID
	// 付款方錢包ID
	PayerWalletID string
	PayerWallet   Wallet `gorm:"referenceKey:PayerWalletID;"`
	Amount        int
	Fee           int
	// 收款方錢包ID
	ReceiverWalletID string
	RecieverWallet   Wallet `gorm:"referenceKey:RecieverWalletID;"`
	common.CreatedAt
}

// 商戶戶轉
type MerchantTransferLog struct {
	MerchantSettlementLog
}

// 商戶系統回收
type MerchantSystemtRecycleLog struct {
	common.ID
	WalletID string
	Wallet
	CoinType
	// 申請回收金額
	Amount int
	common.CreatedAt
}
