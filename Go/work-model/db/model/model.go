package model

import (
	"appserver/db/common"
	"appserver/db/trade"
	"time"

	"github.com/shopspring/decimal"
)

// //// login session /////////
// todo 會根據security proxy再調整
type Session struct {
	Key        string `gorm:"not null;type:char(255);primaryKey;"`
	UserId     uint
	LogoutedAt time.Time
	common.CreatedAt
}

////// market maker /////////

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
type MarketMakerQuotaSetting struct {
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
type MarketMakerBonusSetting struct {
	common.ID
	// 代收獎金比例
	CollectionRatio float32 `gorm:"not null;default:0;"`
	// 代付獎金比例
	PayingRatio float32 `gorm:"not null;default:0;"`

	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 銀行卡交易次數上限
type MarketMakerTradeSetting struct {
	common.ID
	Count uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 帳號代收代付上限
type MarketMakerAccountQuotaSetting struct {
	common.ID
	// 每日代收上限
	DayCollectionLimit uint `gorm:"not null;default:0;"`
	// 每日代付上限
	DayPayingLimit uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 媒合設定
type MarketMakerMatchSetting struct {
	common.ID
	// 每日同銀行卡片匹配次數
	DayLimitOfSameBank uint `gorm:"not null;default:0;"`
	// 同步錢包訂單數量
	SyncWalletOrder uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 使用者服務設定
type MarketMakerUserAgreementSetting struct {
	common.ID
	Content string
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 溫馨提醒設定
type MarketMakerRemindSetting struct {
	common.ID
	Content string
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 注意事項設定
type MarketMakerPleaseNoteSetting struct {
	common.ID
	Content string
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// IOS 簽證 設定
type MarketMakerIOSSignatureSetting struct {
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

/////// merchant /////////
//////// 商戶控端 ////////////

// 集團
type MerchantCorporation struct {
	common.UUID
	Name string `gorm:"unique;not null;type:char(20)"`
	// todo 確認用途
	LoginUrl string `gorm:"unique;not null;type:char(30)"`
	// 待審核 / 已通過 / 已拒絕
	Status uint `gorm:"not null;default:0;"`
	// Verified bool   `gorm:"defaut:false;not null"`
	Enable bool   `gorm:"default:false;not null;"`
	Note   string `gorm:"type:char(30);default:null;"`
	// 管端登入ip限制
	LimitLoginIP bool `gorm:"default:true;not null;"`
	// 登入白名單
	Whitelistings string `gorm:"type:json"`
	common.CreatedAtAndUpdatedAt
}

// 單一商戶主要資料
// 非商戶相關會員
type Merchant struct {
	common.ID
	CorperationID uint `gorm:"not null;default:0;"`
	MerchantCorporation
	WalletID string `gorm:"not null;"`
	Name     string `gorm:"type:char(20);not null;"`
	Phone    string `gorm:"type:char(15);"`
	// 掛單出售Y幣
	Sell bool `gorm:"default:true;not null"`
	// 會員出款
	Withdraw bool `gorm:"default:false;not null"`
	// 控端 url
	ConsoleUrl string `gorm:"not null;type:char(100);"`
	Url        string `gorm:"not null;type:text"`
	// test url
	TestUrl string `gorm:"not null;type:text"`
	// 商戶圖示
	Picture string `gorm:"not null;type:text;"`
	///// 手續費
	// 存款手續費
	DepositFee uint `gorm:"not null;default:0;"`
	// 提款手續費
	WithdrawalFee uint `gorm:"not null;default:0;"`
	// 掛單手續費
	OrderFee uint `gorm:"not null;default:0;"`
	// 回收手續費
	RecyclingFee uint `gorm:"not null;default:0;"`
	// 回收代幣數量下限
	RecyclingLimit uint `gorm:"not null;default:0;"`
	// 可出售Y幣
	Salable bool `gorm:"not null;default:true;"`
	// 會員可出款
	Dispensable bool `gorm:"not null;default:true;"`
	// 安全存量
	SaftyAmount uint `gorm:"not null;default:0;"`
	// 待審核 / 已通過 / 已拒絕
	Status uint `gorm:"not null;defualt:0;"`
	// Verified bool   `gorm:"defaut:false;not null"`
	Enable bool   `gorm:"default:false;not null"`
	APIKey string `gorm:"not null;type:char(50)"`
	// 管端登入ip限制
	LimitLoginIP  bool   `gorm:"default:true;not null"`
	Whitelistings string `gorm:"type:json"`
	// 測試用白名單
	TestWhitelistings string `gorm:"type:json"`

	///// 體系 /////
	// 如果是某個體系的主商戶，則此欄位有值
	SystemID uint
	// 如果屬於某個體系的成員，則此欄位有值
	AssociatedSystemID uint
	Setting            MerchantSetting

	common.CreatedAtAndUpdatedAt
}

// 商戶銀行卡 此商戶底下所有的帳號都可以看到銀行卡資訊
type MerchantBankCard struct {
	common.ID
	MerchantID uint `gorm:"not null;default:0;"`
	Merchant
	BankID     uint   `gorm:"not null;defualt:0;"`
	BranchName string `gorm:"not null;type:char(20);"`
	// 銀行帳號
	Code string `gorm:"not null;type:char(20);"`
	// 卡片使用者名稱
	Name string `gorm:"not null;type:char(20);"`
	common.CreatedAtAndUpdatedAt
	common.DeletedAt
}

// 預設域名列表 可以在創建商戶時選擇
type MerchantDomain struct {
	common.ID
	Name string `gorm:"not null;type:char(50)"`
	common.CreatedAtAndUpdatedAt
}

// 帳號類別 推播管理
type MerchantfBoardcast struct {
	common.ID
	MerchantID uint `gorm:"not null;default:0;"`
	Merchant
	// todo 補幣/ 補Y幣 差別?
	//商戶 補幣審核通過 / Y幣下發審核通過 / Y幣補幣審核通過
	Type uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 商戶 交收體系
// 一個商戶只能在一個體系
type MerchantSystem struct {
	common.ID
	// 主商戶
	MainMerchant Merchant
	Name         string `gorm:"char(30); not null;"`
	// 此體系底下的商戶
	Merchants []Merchant `gorm:"foreignKey:AssociatedSystemID;"`
	common.CreatedAtAndUpdatedAt
}

// 跑馬燈
type MerchantMarquee struct {
	common.ID
	// 發給所有集團
	AllCorporations bool `gorm:"not null;default:false"`
	// 單一集團
	CorporationID uint
	Corporation   MerchantCorporation
	// 所有商戶
	AllMerchants bool `gorm:"not null;default:false"`
	// 單一商戶
	MerchantID uint
	Merchant   Merchant

	Content   string `gorm:"not null;type:char(50);"`
	StartedAt time.Time
	EndedAt   time.Time
	Verified  bool `gorm:"defaut:false;not null"`
	// 附檔
	common.Attachment
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 公告
type MerchantBulletin struct {
	common.ID
	// 單一集團ID 有值時發給單一集團，null時則發給全部集團
	CorporationID uint
	MerchantCorporation
	Content  string `gorm:"not null;type:char(100);"`
	Verified bool   `gorm:"defaut:false;not null"`
	common.CreatedAtAndUpdatedAt
}

// 商戶控端 系統設定
type MerchantConsoleSystemSetting struct {
	// 例行性維護
	Routine bool `gorm:"not null;default:false"`
	// 金流系統維護
	PaymentFlow bool `gorm:"not null;default:false"`
	StartedAt   time.Time
	EndedAt     time.Time
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// ////////////////// 商戶管端 ///////////////////////
type MerchantSetting struct {
	// 商戶配置頁面
	MerchantID  uint
	Page        bool `gorm:"not null;default:false;"`
	ApiKey      bool `gorm:"not null;default:false;"`
	IPWhitelist bool `gorm:"not null;default:false;"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 營運管理
type MerchantSystemSetting struct {
	common.ID
	Initial    uint `gorm:"not null;default:0;"`
	AlarmRatio uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// todo 關於商戶 補幣 上交 計算手續費 凍結 等等跟貨幣有關table 未全

// /// user ////////
type User struct {
	common.ID
	Account  string `gorm:"not null;type:char(30);uniqueIndex:uqidx_account_type;"`
	Password string `gorm:"not null;type:char(30);"`
	Name     string `gorm:"not null;type:char(30);"`
	Note     string `gorm:"default:null;type:char(30)"`
	// 商戶管端 / 商戶控端 / 錢包管端 / 錢包user / 造市商管端 / 造市商user
	Type          uint   `gorm:"not null;default:0;uniqueIndex:uqidx_account_type;"`
	OtpEnable     bool   `gorm:"default:false;not null"`
	OtpVerified   bool   `gorm:"default:false;not null"`
	OtpSecret     string `gorm:"default:null"`
	OtpAuth_url   string `gorm:"default:null"`
	LastLoginedIP string `gorm:"not null;type:char(15);"`
	LastLoginedAt time.Time
	// 啟用
	// 商控 後台帳號管理 新增帳號直接啟用且不用審核
	common.Enable
	// 凍結 暫時停權
	Freezed bool `gorm:"not null;default:false;"`
	// 連續登入失敗被封鎖
	LockedAt time.Time

	GroupID         uint `gorm:"not null;default:0;"`
	Group           `gorm:"foreignKey:GroupID;"`
	WalletUser      WalletUser
	MerchantUser    MerchantUser
	MarketMakerUser MarketMakerUser
	common.CreatedAtAndUpdatedAt
	common.Deleted
}

// 群組 (共用)
type Group struct {
	common.ID
	// todo 是否要把商管的腳色納入
	// 商控 / 商管 / 錢包管 / 造市商管
	Type  uint   `gorm:"not null;default:0;"`
	Name  string `gorm:"not null;type:char(20);"`
	Level uint
	Note  string `gorm:"type:char(30);default:null"`
	// 這個群組可以用的造市商控端功能列表，json與bit flag格式
	FunctionSetting string `gorm:"not null;type:json;"`
	common.CreatedAtAndUpdatedAt
}

// //// 造市商 app ///////////
// app user
type MarketMakerUser struct {
	common.ID
	UserID uint `gorm:"not null;"`
	// 是否為特殊代理
	IsSpecialAgent bool `gorm:"not null;default:false;"`
	// 此user的一級代理是誰
	// TopUserID *uint `gorm:"not null;default:0;"`
	// TopUser   *MarketMakerUser
	Level uint `gorm:"not null;default:0"`
	// 通訊軟體平台
	Messager        string `gorm:"not null;type:char(30);"`
	MessagerAccount string `gorm:"not null;type:char(30);"`
	// 推薦人 上級代理人
	RecommenderID       *uint `gomr:"not null;defualt:0;"`
	Recommender         *MarketMakerUser
	RecommenderVerified bool `gorm:"not null;default:false;"`
	// 帳號開通
	Active bool `gorm:"not null;default:false;"`
	// // 暫時停用
	// Suspended bool `gorm:"not null;default:fasle;"`
	// 允許編輯銀行卡
	CanEditBankCard bool   `gorm:"not null;default:true;"`
	Phone           string `gorm:"not null;type:char(15);"`
	RegisteredIP    string `gorm:"not null;type:char(15);"`

	/////// 代收代付設定 ////////
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

	MarketMakerBankCardSettings []MarketMakerUserBankCardSetting
	common.CreatedAtAndUpdatedAt
}

// 造市商會員銀行卡交易限制
type MarketMakerUserBankCardSetting struct {
	common.ID
	MarketMakerUserID uint `gorm:"not null;default:0;"`
	// 代收 1 / 代付 2 / 代收付 3
	Type       uint   `gorm:"not null;default:0;"`
	Enable     bool   `gorm:"not null;default:true;"`
	BankID     uint   `gorm:"not null;default:0;"`
	BranchName string `gorm:"not null;default:not null;type:char(20);"`
	// 卡片所有人名稱
	Name string `gorm:"not null;default:not null;type:char(20);"`
	// 單筆上限
	Limit uint `gorm:"not null;default:0;"`
	// 每日上限
	DayLimit uint `gorm:"not null;default:0;"`

	common.CreatedAtAndUpdatedAt
	common.Deleted
}

// ///// 錢包app ///////
// 錢包app使用者
type WalletUser struct {
	common.ID
	UserID       uint   `gorm:"not null;default:0;"`
	RegisteredIP string `gorm:"not null;type:char(15);"`
	Verified     bool   `gorm:"not null;default:false;"`
	// 交易密碼
	TransactionPassword string `gorm:"type:char(6)"`
	WalletUserVerify    WalletUserVerify
	WalletUserBankCards []WalletUserBankCard
	common.CreatedAtAndUpdatedAt
}

// 先保留
// 當有任何操作錢包行為時，例如掃code上分
// 當上分行為完成後，call back娛樂城我們的wallet id和他們給的transaction id
// 錢包和遊戲的binding
// type WalletUserMerchant struct {
// 	WalletUserId uint
// 	MerchantId   uint
// 	// 娛樂城 帳號id
// 	UUID string
// }

// 錢包app 實名 照片 ID card認證
type WalletUserVerify struct {
	common.ID
	WalletUserID uint
	RealName     string `gorm:"not null;type:char(20);"`
	IDCardNumber string `gorm:"not null;type:char(20);"`
	BirthDay     time.Time
	IDCardFront  common.Attachment `gorm:"embedded;"`
	IDCardBack   common.Attachment `gorm:"embedded;"`
	// 手持ID card照片
	UserIDCardPhoto common.Attachment `gorm:"embedded;"`
	// 待審核 已同意 已拒絕
	Status uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 錢包使用者 銀行卡
type WalletUserBankCard struct {
	common.ID
	WalletUserID uint       `gorm:"not null;defalt:0;"`
	WalletUser   WalletUser //`gorm:"reference:WalletUserID;"`
	BankID       uint       `gorm:"not null;default:0;"`
	BranchName   string     `gorm:"not null;default:not null;type:char(20);"`
	BankCode     string     `gorm:"not null;type:char(20);"`
	// 新增銀行卡時不再填入名稱，直接只用實名驗證的名稱

	common.CreatedAtAndUpdatedAt
	common.Deleted
}

//////// 商戶管端 ////////////

// 商管腳色
type MerchantRole struct {
	common.ID
	// 集團管理員 商戶管理員 站長 開發人員 行銷人員
	Name string `gorm:"not null;type:char(20)"`
	// todo 權限分化
	common.CreatedAtAndUpdatedAt
}

// 商管帳號
type MerchantUser struct {
	common.ID
	UserID        uint `gorm:"not null;"`
	CorporationID uint `gorm:"not null;"`
	// 如商戶為null 則為此集團的跨商戶帳號
	MerchantID uint
	// todo 使用role 還是 group
	RoleID uint `gorm:"not null;default:0;"`
	Role   MerchantRole
	// 	// 如果腳色是站長，且帳號為跨商戶的話，則要記錄所屬的所有站台
	MerchantUserStantionMasters []MerchantUserStantionMaster //`gorm:"foreignKey:MerchantUserRoleID;"`
	common.CreatedAtAndUpdatedAt
}

// 記錄跨商戶站長資料
type MerchantUserStantionMaster struct {
	common.ID
	MerchantUserID uint `gorm:"not null;default:0;"`
	MerchantID     uint `gorm:"not null;default:0;"`
	common.CreatedAt
}

////// wallet ///////
/////////// 放錢包控端 跟 app相關東西 並非錢包本身///////////

// 控端銀行
type WalletConsoleBank struct {
	common.ID
	// todo web有填入銀行ID的地方 有需要保留?
	BankID   string `gorm:"unique;not null;type:char(30);"`
	BankName string `gorm:"unique;not null;type:char(30);"`
	// 待審核 / 已通過 / 已拒絕
	Status uint `gorm:"not null;"`
	Enable bool `gorm:"not null;default:true;"`
	common.Applicant
	common.CreatedAtAndUpdatedAt
}

// 阻擋註冊IP
type WalletConsoleRegistrationIPBlocking struct {
	IP string `gorm:"primaryKey;type:char(15);"`
	common.CreatedAtAndUpdatedAt
}

// todo: 鎖單限制? 感覺不合理
type WalletConsoleSystemSetting struct {
	common.ID
	// 掛單數量限制
	OrderLimit uint `gorm:"not null;default:0;"`
	// 鎖單數量限制
	LockedOrderLimit uint `gorm:"not null;default:0;"`
	// 取消數量限制
	CanceledOrderLimit uint `gorm:"not null;default:0;"`
	// 手續費
	Fee         uint `gorm:"not null;default:0;"`
	AutoMatched bool `gorm:"not null;default:true;"`
	common.CreatedAtAndUpdatedAt
}

// 同時吃單上限
// todo 達到吃單上限時 會不交易嗎?
type WalletConsoleTradeSetting struct {
	common.ID
	// 總代理 / 代理 / 造市商 / 自然人 / 商戶
	Type int `gorm:"not null;default:0;"`
	Call int `gorm:"not null;default:0;"`
	Put  int `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 拆單數量
type WalletConsoleSeparatedBill struct {
	Number uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 錢包 獎勵活動
type WalletConsoleActivity struct {
	common.ID
	Name      string    `gorm:"not null;type:char(50);"`
	StartedAt time.Time `gorm:"not null;"`
	EndedAt   time.Time `gorm:"not null;"`
	// 活動消息標題
	Title   string `gorm:"not null;type:char(50);"`
	Content string `gorm:"not null;type:text;"`
	// todo 額度設定 領取內容
	common.CreatedAtAndUpdatedAt
}

// 公告
type WalletConsoleBulletin struct {
	common.ID
	Title   string `gorm:"notn null;type:char(50);"`
	Content string `gorm:"not null;type:text;"`
	// 置頂
	OnTop    bool `gorm:"not null;default:false;"`
	Verified bool `gorm:"defaut:false;not null"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 個人消息
type WalletConsoleMessage struct {
	common.ID
	Title    string `gorm:"notn null;type:char(50);"`
	Content  string `gorm:"not null;type:text;"`
	Verified bool   `gorm:"defaut:false;not null"`
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// 個人消息接收者
type WalletConsoleMessageUser struct {
	common.ID
	MessageID    uint `gorm:"unique_index:uqidx_message_wallet_user;"`
	WalletUserID uint `gorm:"unique_index:uqidx_message_wallet_user;"`
	// todo 待確認是否有這個功能
	// 已傳送
	Sended bool `gorm:"not null;default:false;"`
	common.CreatedAt
}

// 推撥
// todo 不確定是否有存db的需要? 或存redis 先定義型別
type BoardcastBase struct {
	Title  string
	Conten string
}

type BoardcastByUser struct {
	BoardcastBase
	WalletUserIDs []string
}

type BoardcastByDevice struct {
	BoardcastBase
	// 0 ios 1 android
	Device uint
}

// todo 公告內容 個人消息 是跟個人推撥一樣嗎?

// //////// log ////////////
// //// 審核相關 ///////////
// 審核類型
type VerificationType struct {
	common.ID
	common.SystemType
	Name string `gorm:"not null;type:char(20)"`
	common.CreatedAtAndUpdatedAt
}

// 審核標題
type VerificationTitle struct {
	common.ID
	Name   string `gorm:"not null;type:char(20)"`
	TypeID uint   `gorm:"not null;default:0;"`
	// 審核類型
	VerificationType VerificationType `gorm:"foreignKey:TypeID;"`
	Level            uint             `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// 審核列表
type Verification struct {
	common.ID
	// 標題
	TitleID           uint              `gorm:"not null;default:0;"`
	VerificationTitle VerificationTitle `gorm:"foreignKey:TitleID;"`
	// 事項
	Item string `gorm:"not null;type:char(30);"`
	// 事由
	Reason string `gorm:"not null;type:char(30);"`
	// 修改前資料明細 json
	Before string `gorm:"not null;type:json"`
	// 修改後資料明細 json
	After string `gorm:"not null;type:json"`

	// 1 用戶申請 / 2 管理者同意 / 3 管理者拒絕 / 4 申請人取消
	Status uint `gorm:"not null;default:0;"`
	// 附檔 url
	common.Attachment
	// 申請者
	common.Applicant
	// 審核人
	common.Approver
	// 當審核通過 直接只用下方資訊修改資料內容
	Database string `gorm:"not null;type:char(10);"`
	Table    string `gorm:"not null;type:char(10);"`
	DataID   uint   `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

////// 登入相關紀錄 ////////

// 登入錯誤次數過多被鎖定
type UserLockLog struct {
	common.ID
	UserID uint `gorm:"not null;default:0;"`
	// todo 錢包控端 app user看到有紀錄ip，確認其他系統或app是否也有
	common.IP
	LockedAt   time.Time
	UnLockedAt time.Time
	common.CreatedAtAndUpdatedAt
}

// 登入紀錄
// 商戶控端 管端都有
type LoginLog struct {
	common.ID
	UserID uint `gorm:"not null;default:0;"`
	// 登入平台
	// todo 是否改成用type和數字
	// PC / android / ios / 5min / yapay
	Application string `gorm:"not null;type:char(10)"`
	// 登入IP
	common.IP
	// login statuse 成功 / 失敗
	Statue bool `gorm:"not null;default:false;"`
	// 失敗原因
	Note string `gorm:"default:null;type:char(30)"`
	common.CreatedAt
}

// /// 集團相關紀錄 //////
// 商戶集團管理 -> 商戶集團ip白名單紀錄
type CorporationWhitelistingLog struct {
	common.ID
	CorporationID string `gorm:"type:uuid;not null;uniqueIndex:corp_id_ip"`
	IP            string `gorm:"type:char(15);not null;uniqueIndex:corp_id_ip"`
	//狀態 1新增 / 0刪除
	Statue bool `gorm:"default:true;default:true;"`
	// 操作者ID
	common.Operator
	common.CreatedAtAndUpdatedAt
}

// /// 商戶相關紀錄 /////
// 商戶管理IP白名單紀錄
type MerchantWhitelistingLog struct {
	common.ID
	MerchantID uint `gorm:"not null;default:0;"`
	////// 變動前ip紀錄 //////
	BeforeIPWhitelisting     string `gorm:"type:json;"`
	BeforeTestIPWhitelisting string `gorm:"type:json;"`
	////// 變動後ip紀錄 //////
	AfterIPWhitelisting     string `gorm:"type:json;"`
	AfterTestIPWhitelisting string `gorm:"type:json;"`
	// 控端管端都可以編輯商戶ip
	// 控端 0 / 管端 1
	Source uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAt
}

// TODO 研究丟到cloud watch or 其他log db
// TODO 下方所有log 是否要在收斂成一個類別 用type去切換
type MerchantPasswordResetLog struct {
	common.ID
	MerchantID uint `gorm:"not null;default:0;"`
	common.Operator
	common.CreatedAt
}

type UnbindMerchantPhoneLog struct {
	common.ID
	MerchantID uint   `gorm:"not null;default:0;"`
	Phone      string `gorm:"type:char(15);default:null;"`
	common.Operator
	common.CreatedAt
}

// 控端帳號鎖定
// type AccountLockLog struct {
// 	common.ID
// 	LockTime time.Time
// 	UserID   uint   `gorm:"not null;default:0;"`
// 	Reason   string `gorm:"type:char(30);not null;"`
// 	common.CreatedAtAndUpdatedAt
// }

// // 管端帳號鎖定
// type MerchantLockLog struct {
// 	common.ID
// 	LockTime   time.Time
// 	MerchantID uint `gorm:"not null;"`
// 	common.CreatedAtAndUpdatedAt
// }

// // google 驗證操作紀錄
type MerchantOTPLog struct {
	common.ID
	MerchantID uint `gorm:"not null;"`
	// 操作內容 啟用/停用 2fa
	Operation string `gorm:"not null;type:char(30);"`
	common.Operator
}

///// 造市商 ////////

// 造市商收補幣
type MarketMakerSupplementOrRetractLog struct {
	common.ID
	// todo 目前沒有補幣功能，造市商如何拿碧?
	// 補幣 supplement 1 / 回收 retract 2
	Type  int    `gorm:"not null;"`
	Title string `gorm:"char(30);"`
	// 事項
	Content  string `gorm:"char(30);"`
	Reason   string `gorm:"char(30);"`
	Amount   uint   `gorm:"not null;"`
	Discount uint   `gorm:"not null;"`
	common.Applicant
	common.CreatedAtAndUpdatedAt
}

// 計算成交訂單任務獎金
type MarketMakerTaskLog struct {
	common.ID
	OrderID uint `gorm:"not null;default:0;"`
	// 獎金比例
	BounusRatio decimal.Decimal `gorm:"not null;default:0;"`
	// 實際收到獎金
	Bounus decimal.Decimal `gorm:"not null;default:0;"`
	common.CreatedAt
}

// 紀錄造市商銀行卡當日是否達到次數上限 或總金額上限
type MarketMakerUserBankCardLog struct {
	common.ID
	MarketMakerUserBankCardSettingID uint `gorm:"not null;default:0;"`
	// 到達次數上限
	Overlimited bool `gorm:"not null;default:false;"`
	Amount      uint `gorm:"not null;default:0;"`
	Count       uint `gorm:"not null;default:0;"`
	common.CreatedAtAndUpdatedAt
}

// ///// 錢包控端 ///////
// 補幣 / 收幣
type WalletConsoleRefillAndRecycleLog struct {
	common.ID
	WalletID string `gorm:"not null;"`
	trade.CoinType
	Amount uint `gorm:"not null;default:0;"`
	common.CreatedAt
}

// 回收代幣
type WalletConsoleRecycleLog struct {
	common.ID
	WalletID string `gorm:"not null;"`
	trade.CoinType
	// 申請回收金額
	Amount uint `gorm:"not null;default:0;"`
	// 實際回收金額
	RecycleAmount uint `gorm:"not null;default:0;"`
	// 手續費
	Fee float32 `gorm:"not null;default:0;"`
	common.CreatedAt
}

////// merchant ////////

// todo 放在trade 還是 log

// 入款(會員存款) 紀錄
type DespositAndWithdrawLog struct {
	common.ID
	// 入款 / 出款
	Type         uint `gorm:"not null;defautl:0;"`
	UserWalletID uint
	// 手續費率
	FeeRatio uint `gorm:"not null;"`
	// 實收手續費
	Fee              uint `gorm:"not null;"`
	MerchantWalletID uint
	Amount           uint `gorm:"not null;"`
	// 成功 / 用戶未付款 / 已付款 開分失敗
	Status uint `gorm:"not null;"`
	common.CreatedAt
	CompletedAt time.Time
}

// 商戶補幣
type MerchantRefillLog struct {
	common.ID
	WalletID string
	trade.CoinType
	// 補幣金額
	Amount uint
	// 手續費
	Fee uint
	// 匯款資訊
	Payment string
	// 上傳附檔
	Attachment string
	common.CreatedAt
}

// 商戶交收
type MerchantSettlementLog struct {
	common.ID
	// 付款方錢包ID
	PayerWalletID string
	Amount        int
	Fee           int
	// 收款方錢包ID
	ReceiverWalletID string
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
	trade.CoinType
	// 申請回收金額
	Amount int
	common.CreatedAt
}
