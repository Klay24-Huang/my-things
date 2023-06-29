package trade

import (
	"gorm-example/model/common"
	"gorm-example/model/user"
)

// ////////////////// trade ///////////////////////
// 產生/銷毀 代幣
type CentralBank struct {
	common.ID
	// 造幣 create / 回收 delete
	Type string `gorm:"not null;varchar(5)"`
	common.Key
	user.PublicKey `gorm:"foreignKey:Key"`
	common.CreatedAt
}

type Order struct {
	common.ID
	common.Key
	user.PublicKey `gorm:"foreignKey:Key"`
	// 買單 call 1 / 賣單 put 2
	Type int `gorm:"not null;"`
	// 拆單
	// todo 要記成percent?
	// todo 拆單細節邏輯考慮
	Splittable  bool `gorm:"not null;default:false"`
	ParentID    *uint
	ParentOrder *Order
	CoinType    string `gorm:"not null;varchar(3);"`
	// 鎖定中 部分切單交易中
	Locked bool `gorm:"not null;default:false;"`
	// 下單數量
	Amount uint
	common.CreatedAt

	// todo 拆單 parent id
}

// 搓合成功訂單
type Trade struct {
	common.UUID
	CallID    uint  `gorm:"index:idx_call_put;"`
	CallOrder Order `gorm:"foreignKey:CallID;"`
	PutID     uint  `gorm:"index:idx_call_put;"`
	PutOrder  Order `gorm:"foreignKey:PutID;"`
	// 代付款(進行中) / 已取消 / 已完成 / 爭議
	Status string `gorm:"not null;varchar(10);"`
	// 沖正
	Reversal
	common.CreatedAt
	// todo 銀行卡匹配次數上限
}

// 沖正
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
	Type int `gorm:"not null;"`
	common.Applicant
	Title string `gorm:"varchar(30);"`
	// 事項
	Content string `gorm:"varchar(30);"`
	Reason  string `gorm:"varchar(30);"`
	common.Approver
	common.CreateAtAndUpdateAt
}
