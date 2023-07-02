package trade

import (
	"appserver/src/database/new/common"
	"time"
)

// ////////////////// trade ///////////////////////
// 產生/銷毀 代幣
type CentralBank struct {
	common.ID
	// 造幣 create / 回收 delete
	Type string `gorm:"not null;varchar(5)"`
	common.Key
	common.CreatedAt
}

type Order struct {
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
	ParentID    *uint
	ParentOrder *Order
	// 幣種
	CoinType string `gorm:"not null;varchar(3);"`
	// 鎖定中 部分切單交易中
	Locked bool `gorm:"not null;default:false;"`
	// 排序值 優先值 越大越優先
	Priority uint `gorm:"not null;defalt:0;"`
	// 下單數量
	Amount uint
	// 鎖單時間
	LockedAt time.Time
	common.CreatedAt
	// todo 拆單 parent id
}

// 搓合成功訂單 交易紀錄
type Trade struct {
	common.UUID
	// // 開單前最初的交易ID
	ParentID    *uint
	ParentTrade *Trade
	CallID      uint  `gorm:"index:idx_call_put;"`
	CallOrder   Order `gorm:"foreignKey:CallID;"`
	PutID       uint  `gorm:"index:idx_call_put;"`
	PutOrder    Order `gorm:"foreignKey:PutID;"`
	// 代付款(進行中) / 已取消 / 已完成 / 爭議 / 自動取消 / 暫停交易 / 標記
	Status string `gorm:"not null;varchar(10);"`
	// 付款照片url
	PaymentUrl string `gorm:"varchar(30);"`
	// 付款持間
	PaidAt time.Time
	// 沖正
	Reversal
	common.CreateAtAndUpdateAt
	// todo 銀行卡匹配次數上限
}

// todo 商戶 造市商的收發幣的沖正
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
	Title        string `gorm:"varchar(30);"`
	// 事項
	Content string `gorm:"varchar(30);"`
	Reason  string `gorm:"varchar(30);"`
	common.CreateAtAndUpdateAt
}

type Wallet struct {
	common.UUID
	Balance int
	common.CreateAtAndUpdateAt
}
