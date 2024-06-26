package model

import (
	"time"
)

type CreatedAt struct {
	CreatedAt time.Time
}

type CreateAtAndUpdateAt struct {
	CreatedAt
	UpdatedAt time.Time
}

type DeletedAt struct {
	DeletedAt time.Time
}

type Deleted struct {
	Deleted bool `gorm:"not null;default:false;"`
	DeletedAt
}

type Enable struct {
	Enable bool `gorm:"not null;default:false;"`
}

type ID struct {
	ID uint `gorm:"primaryKey"`
}

type UUID struct {
	ID string `gorm:"type:uuid;default:UUID();not null"`
}

type Operator struct {
	OperatorID uint
	User       User `gorm:"foreignKey:OperatorID"`
}

type Approver struct {
	ApproverID uint
	User       User `gorm:"foreignKey:ApproverID"`
}

type Applicant struct {
	// todo 是否改成只存public Key
	ApplicantID uint
	User        User `gorm:"foreignKey:ApplicantID"`
}

type IP struct {
	IP string `gorm:"not null;type:char(15)"`
}

type NameBasic struct {
	Nmae string `gorm:"primaryKey;type:char(30); not null;"`
	CreateAtAndUpdateAt
}

type Key struct {
	Key string `gorm:"not null;char(256)"`
}

type Group struct {
	Name  string `gorm:"unique;not null;type:char(30)"`
	Level uint
	Note  string `gorm:"type:char(30);default:null"`
	// 這個群組可以用的造市商控端功能列表，json與bit flag格式
	FunctionSetting string `gorm:"not null;type:json;"`
}

/////// User /////////

type User struct {
	ID
	Account  string `gorm:"unique;not null;type:char(30)"`
	Password string `gorm:"not null;type:char(30)"`
	Name     string `gorm:"not null;type:char(30);"`
	Note     string `gorm:"default:null;type:char(30)"`
	GroupID  uint
	Group    MerchantGroup
	// 商戶管端 / 商戶控端 / 錢包管端 / 錢包user / 造市商管端 / 造市商user
	Type uint `gorm:"not null;"`
	//  密鑰?
	OtpEnable   bool   `gorm:"default:false;not null"`
	OtpVerified bool   `gorm:"default:false;not null"`
	OtpSecret   string `gorm:"default:null"`
	OtpAuth_url string `gorm:"default:null"`
	Enable
	CreateAtAndUpdateAt
	Deleted

	PublicKeys []PublicKey
}

type PublicKey struct {
	Key    string `gorm:"primaryKey,not null;type:char(256);"`
	UserID uint
	CreatedAt
}

// 登入錯誤次數過多被鎖定
type LockedUser struct {
	ID
	UserID uint
	User
	CreatedAt
	Deleted
}

// 登入紀錄
// 商戶控端 管端都有
type LoginLog struct {
	ID
	UserID uint `gorm:"not null;"`
	User
	// 登入平台
	Application string `gorm:"not null;type:char(10)"`
	IP
	// login statuse 成功 / 失敗
	Statue bool `gorm:"not null;"`
	CreatedAt
}

// app user
type MarketMakerUser struct {
	ID
	UserID uint `gorm:"not null;"`
	User
	SpecialAgent bool `gorm:"not null;default:false;"`
	Level        uint `gorm:"not null;"`
	// 通訊軟體平台
	Messager        string `gorm:"not null;type:char(30);"`
	MessagerAccount string `gorm:"not null;type:char(30);"`
	// 推薦人
	RecommenderID       *uint
	Recommender         *MarketMakerUser
	RecommenderVerified bool `gorm:"not null;default:false;"`
	// 帳號開通
	Active    bool `gorm:"not null;default:false;"`
	Suspended bool `gorm:"not null;default:fasle;"`
	// 允許編輯銀行卡
	EditBankCard bool   `gorm:"not null;default:true;"`
	Phone        string `gorm:"not null;type:char(15);"`
	RegisterIP   IP

	/////// 代收代付
	// 可代收
	Collectable bool `gorm:"not null;default:false;"`
	// 代收獎金比例
	CollectionRatio float32 `gorm:"not null;"`
	// 代收上限
	CollectionDayLimit uint `gorm:"not null;"`
	// 可代付
	Payable bool `gorm:"not null;default:false;"`
	// 代付獎金比例
	PayingRatio float32 `gorm:"not null;"`
	// 代付上限
	PayingDayLimit uint `gorm:"not null;"`

	CreateAtAndUpdateAt
}

type MarketMakerBankCardSetting struct {
	ID
	// 代收 1 / 代付 2
	Type   uint   `gorm:"not null;"`
	Enable bool   `gorm:"not null;default:true;"`
	Bank   string `gorm:"not null;default:not null;type:char(20);"`
	Branch string `gorm:"not null;default:not null;type:char(20);"`
	Name   string `gorm:"not null;default:not null;type:char(20);"`
	// 單筆上限
	Limit uint `gorm:"not null;"`
	// 每日上限
	DayLimit uint `gorm:"not null;"`

	CreateAtAndUpdateAt
	Deleted
}

//////// 商戶控端 ////////////

// 商控 群組
type MerchantGroup struct {
	ID
	Name  string `gorm:"unique;not null;type:char(30)"`
	Level uint
	Note  string `gorm:"type:char(30);default:null"`
	CreateAtAndUpdateAt
}

// 集團
type Corporation struct {
	UUID
	Name     string `gorm:"unique;not null;type:char(20)"`
	LoginUre string `gorm:"unique;not null;type:char(30)"`
	Verified bool   `gorm:"defaut:false;not null"`
	Enable   bool   `gorm:"default:false;not null"`
	// 管端登入ip限制
	LimitLoginIP bool   `gorm:"default:true;not null"`
	Note         string `gorm:"type:char(30);default:null"`
	// todo domain name
	CreateAtAndUpdateAt
}

// 集團管端登入ip白名單
type CorporationWhitelistring struct {
	CorporationID string `gorm:"type:uuid;not null;uniqueIndex:corp_id_ip"`
	Corporation   Corporation
	IP            string `gorm:"type:char(15);not null;uniqueIndex:corp_id_ip"`
	//狀態 1新增 / 0刪除
	Statue bool `gorm:"default:true"`
	// 操作者ID

	// TODO: 出處有必要? 編輯or新增
	// TODO: 需要記錄成新增者和刪除者?
	CreateAtAndUpdateAt
}

// 商戶
type Merchant struct {
	ID
	CorperationID uint `gorm:"not null;"`
	Corporation
	WalletID uint `gorm:"not null;"`
	Wallet
	Name  string `gorm:"type:char(20);not null;"`
	Phone string `gorm:"type:char(15);default:null;"`
	// 掛單出售Y幣
	Sell     bool `gorm:"default:true;not null"`
	Transfer bool `gorm:"default:true;not null"`
	// 控端 url
	ConsoleUrl string `gorm:"not null;type:char(50)"`
	Url        string `gorm:"not null;type:char(50)"`
	// test url
	TestUrl  string `gorm:"not null;type:char(50)"`
	Verified bool   `gorm:"defaut:false;not null"`
	Enable   bool   `gorm:"default:false;not null"`

	MerchantSetting

	CreateAtAndUpdateAt
}

type Domain struct {
	ID
	Name string `gorm:"not null;type:char(50)"`
	CreateAtAndUpdateAt
}

type MerchantPasswordResetLog struct {
	ID
	MerchantID uint `gorm:"not null;"`
	Merchant
	Operator
	CreatedAt
}

type UnbindMerchantPhoneLog struct {
	ID
	MerchantID uint `gorm:"not null;"`
	Merchant
	Phone string `gorm:"type:char(15);default:null;"`
	Operator
	CreatedAt
}

type AccountLockLog struct {
	LockTime time.Time
	UserID   uint `gorm:"not null;"`
	User
	Locked bool   `gorm:"default:false;not null;"`
	Reason string `gorm:"type:char(30);not null;"`
	// todo operator?
	CreateAtAndUpdateAt
}

type MerchantLockLog struct {
	LockTime   time.Time
	MerchantID uint `gorm:"not null;"`
	Merchant
	// todo operator?
	CreateAtAndUpdateAt
}

// google 驗證操作紀錄
type MerchantOTPLog struct {
	MerchantID uint `gorm:"not null;"`
	Merchant
	// 操作內容 啟用/停用 2fa
	Operation string `gorm:"not null;type:char(30);"`
	Operator
}

type Boardcast struct {
	MerchantID uint `gorm:"not null;"`
	Merchant
	// todo 補幣/ 補Y幣 差別?
	//商戶 補幣審核通過 / Y幣下發審核通過 / Y幣補幣審核通過
	Flags uint
	// operator?
	CreateAtAndUpdateAt
}

type OperatorItem struct {
	NameBasic
}

type OperationDetail struct {
	NameBasic
}

type OperationLog struct {
	Operator
	OperatorItem
	OperationDetail
	Content string `gorm:"type:char(30);not null;"`
	IP
	CreatedAt
}

type Wallet struct {
	UUID
	Balance int
	CreateAtAndUpdateAt
}

type MerchantWalletSetting struct {
	// 商戶錢包管理
	//todo 帶商戶管端開完 再回來會比較清楚
}

// yapay 錢包會員
type WalletUser struct {
	ID
	Name string `gorm:"not null;type:char(30);"`
	CreateAtAndUpdateAt
}

type MerchantBindWalletUser struct {
	MerchantID uint `gorm:"not null;"`
	Merchant
	WalletUserID uint
	WalletUser
	Bingding bool `gorm:"not null;default:true;"`
	// todo 出入款紀錄
	// todo 解除再綁定的話 出入款金額的計算方法
	CreateAtAndUpdateAt
}

// todo 商戶控端 先到會員綁定

// 審核類型
type VerifyType struct {
	NameBasic
}

// 審核標題
type VerifyTitle struct {
	NameBasic
	VerifyTypeID uint
	VerifyType
	Level int
}

// 審核列表
type Verify struct {
	ID
	// 標題
	VerifyTitleID uint
	VerifyTitle
	Title string `gorm:"not null;type:char(30);"`
	// 事項
	Item string `gorm:"not null;type:char(30);"`
	// 事由
	Reason string `gorm:"not null;type:char(30);"`
	// 1 待審核 / 2 已同意 / 3 已拒絕 / 4 已取消
	Status uint
	// 回調
	CallBackCount uint
	// todo 尚未知作用
	CallbackStatuse bool
	CallbackLog     string `gorm:"char(256);default:null;"`
	// 申請者
	Applicant
	// 審核人
	Approver
	CreateAtAndUpdateAt
}

// todo 商戶類別先跳過

// 跑馬燈
type Marquee struct {
	ID
	// 發給所有集團
	AllCorporations bool `gorm:"not null;default:false"`
	// 單一集團
	CorporationID uint
	Corporation
	// 所有商戶
	AllMerchants bool `gorm:"not null;default:false"`
	// 單一商戶
	MerchantID uint
	Merchant
	Content  string `gorm:"not null;type:char(50);"`
	StartAt  time.Time
	EndAt    time.Time
	VerifyID uint
	Verify
	Operator
	CreateAtAndUpdateAt
}

// 公告
type Bulletin struct {
	ID
	// 發給所有集團
	AllCorporations bool `gorm:"not null;default:false"`
	// 單一集團
	CorporationID uint
	Corporation
	Content string `gorm:"not null;type:char(100);"`
	CreateAtAndUpdateAt
}

type MerchantSystemSetting struct {
	// 例行性維護
	Routine bool `gorm:"not null;default:false"`
	// 金流系統維護
	PaymentFlow bool `gorm:"not null;default:false"`
	StartAt     time.Time
	EndAt       time.Time
	Operator
	CreateAtAndUpdateAt
}

// ////////////////// 商戶控端 ///////////////////////
type MerchantSetting struct {
	// 商戶配置葉面
	MerchantID  uint
	Page        bool `gorm:"not null;default:false;"`
	ApiKey      bool `gorm:"not null;default:false;"`
	IPWhitelist bool `gorm:"not null;default:false;"`
	Operator
	CreateAtAndUpdateAt
}

// 商控腳色
type Role struct {
	// 集團管理員 商戶管理員 站長 開發人員 行銷人員
	NameBasic
	CreateAtAndUpdateAt
}

type MerchantUserRole struct {
	UserID uint `gorm:"idx_user_role"`
	RoleID uint `gorm:"idx_user_role"`
	Role
	Operator
	CreateAtAndUpdateAt
}

type BankCard struct {
	ID
	MerchantID uint   `gorm:"not null;"`
	UserName   string `gorm:"not null;type:char(20);"`
	BankName   string `gorm:"not null;type:char(30);"`
	// 分行名稱
	Branch string `gorm:"not null;type:char(30);"`
	// 卡號
	Code string `gorm:"not null;type:char(30);"`
	CreateAtAndUpdateAt
}

// todo 關於商戶 補幣 上交 計算手續費 凍結 等等跟貨幣有關table 未全
// todo 綁定遊戲api相關

// ////////////////// 造市商控端 ///////////////////////
// 造市商 群組
type MarketMakerGroup struct {
	ID
	Group
	CreateAtAndUpdateAt
}

type MarketMakerGroupUser struct {
	ID
	UserID             int
	MarketMakerGroupID uint
	MarketMakerGroup
	CreateAtAndUpdateAt
}

// 造市商 任務
type Task struct {
	ID
	// 代收 call 1 / 代付 put 2
	Type int `gorm:"not null;"`
	// uuid yapay訂單id
	TradeID string
	// 總獎金
	Amount float32
	CreatedAt
}

type Bounus struct {
	ID
	UserID uint
	TaskID uint
	Task
	Ratio  float32
	Amount float32
	CreatedAt
}

// 造市商 設定相關

// 額度上限設定
type QuotaSetting struct {
	ID
	// 代收
	CollectionDayLimit  uint `gorm:"not null;"`
	CollectionOnceLimit uint `gorm:"not null;"`
	// 代付
	PayingDayLimit  uint `gorm:"not null;"`
	PayingOnceLimit uint `gorm:"not null;"`
	Operator
	CreateAtAndUpdateAt
}

// 獎金比例設定
type BonusSetting struct {
	ID
	// 代收獎金比例
	CollectionRatio float32 `gorm:"not null;"`
	// 代付獎金比例
	PayingRatio float32 `gorm:"not null;"`

	Operator
	CreateAtAndUpdateAt
}

// 銀行卡交易次數上限
type TradeSetting struct {
	ID
	Count uint `gorm:"not null;"`
	Operator
	CreateAtAndUpdateAt
}

// 帳號代收代付上限
type AccountQuotaSetting struct {
	ID
	// 代收
	CollectionDayLimit uint `gorm:"not null;"`
	// 代付
	PayingDayLimit uint `gorm:"not null;"`
	Operator
	CreateAtAndUpdateAt
}

// 媒合設定
type MatchSetting struct {
	ID
	// 每日同銀行卡片匹配次數
	DayLimitOfSameBank uint `gorm:"not null;"`
	// 同步錢包訂單數量
	SyncWalletOrder uint `gorm:"not null;"`
	Operator
	CreateAtAndUpdateAt
}

// 使用者服務設定
type UserAgreementSetting struct {
	ID
	Content string
	Operator
	CreateAtAndUpdateAt
}

// 溫馨提醒設定
type RemindSetting struct {
	ID
	Content string
	Operator
	CreateAtAndUpdateAt
}

// 注意事項設定
type PleaseNoteSetting struct {
	ID
	Content string
	Operator
	CreateAtAndUpdateAt
}

// IOS 簽證 設定
type IOSSignatureSetting struct {
	ID
	// 企業簽 超級簽
	Type uint   `gorm:"not null;"`
	Name string `gorm:"not null;type:char(30);"`
	// 載點
	Url    string `gorm:"not null;type:char(50);"`
	Note   string `gorm:"typechar(50);"`
	Enable bool   `gorm:"default:true"`
	// todo 優先權
	Operator
	CreateAtAndUpdateAt
}

// /////////////////// yapay ///////////////////////
type WalletGroup struct {
	ID
	Group
	CreateAtAndUpdateAt
}

type WalletBank struct {
	ID
	BankID   string `gorm:"unique;not null;type:char(30);"`
	BankName string `gorm:"unique;not null;type:char(30);"`
	Applicant
	CreateAtAndUpdateAt
}

// ////////////////// trade ///////////////////////
// 產生/銷毀 代幣
type CentralBank struct {
	ID
	// 造幣 create / 回收 delete
	Type string `gorm:"not null;char(5)"`
	Key
	PublicKey `gorm:"foreignKey:Key"`
	CreatedAt
}

type Order struct {
	ID
	Key
	PublicKey `gorm:"foreignKey:Key"`
	// 買單 call 1 / 賣單 put 2
	Type int `gorm:"not null;"`
	// 拆單
	// todo 要記成percent?
	// todo 拆單細節邏輯考慮
	Splittable  bool `gorm:"not null;default:false"`
	ParentID    *uint
	ParentOrder *Order
	CoinType    string `gorm:"not null;char(3);"`
	// 鎖定中 部分切單交易中
	Locked bool `gorm:"not null;default:false;"`
	// 下單數量
	Amount uint
	CreatedAt

	// todo 拆單 parent id
}

// 搓合成功訂單
type Trade struct {
	UUID
	CallID    uint  `gorm:"index:idx_call_put;"`
	CallOrder Order `gorm:"foreignKey:CallID;"`
	PutID     uint  `gorm:"index:idx_call_put;"`
	PutOrder  Order `gorm:"foreignKey:PutID;"`
	// 代付款(進行中) / 已取消 / 已完成 / 爭議
	Status string `gorm:"not null;char(10);"`
	// 沖正
	Reversal
	CreatedAt
	// todo 銀行卡匹配次數上限
}

// 沖正
type Reversal struct {
	// todo 會有部分金額?
	ID
	TradeID string `gorm:"type:uuid;"`
	CreatedAt
}

// 造市商收補幣
type MarketMakerSupplementOrRetract struct {
	ID
	// 補幣 supplement 1 / 回收 retract 2
	Type int `gorm:"not null;"`
	Applicant
	Title string `gorm:"char(30);"`
	// 事項
	Content string `gorm:"char(30);"`
	Reason  string `gorm:"char(30);"`
	Approver
	CreateAtAndUpdateAt
}
