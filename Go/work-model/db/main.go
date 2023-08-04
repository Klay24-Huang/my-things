package main

import (
	"appserver/db/model"
	"appserver/db/trade"
	"fmt"

	"gorm.io/driver/mysql"
	"gorm.io/gorm"
	"gorm.io/gorm/schema"
)

type DatabaseInfo struct {
	Host     string
	Port     string
	Database string
	User     string
	Password string
}

const (
	HOST     = "localhost"
	PORT     = "3307"
	DATABASE = "test2"
	USER     = "admin"
	PASSWORD = "password"
	// SSL      = "disable"
)

func getGormDB(databaseInfo DatabaseInfo) *gorm.DB {
	dsn := fmt.Sprintf(
		// "host=%s port=%s user=%s password=%s dbname=%s sslmode=%s",
		// HOST, PORT, USER, PASSWORD, DATABASE, SSL)
		"%s:%s@tcp(%s:%s)/%s?charset=utf8mb4&parseTime=True&loc=Local",
		databaseInfo.User, databaseInfo.Password, databaseInfo.Host, databaseInfo.Port, databaseInfo.Database)

	gormDB, err := gorm.Open(mysql.Open(dsn), &gorm.Config{
		NamingStrategy: schema.NamingStrategy{
			SingularTable: true, // use singular table name, table for `User` would be `user` with this option enabled
		},
	})
	if err != nil {
		panic("open gorm db error")
	}

	return gormDB
}

func main() {
	dbInfo := &DatabaseInfo{
		Host:     "localhost",
		Port:     "3307",
		Database: "test2",
		User:     "admin",
		Password: "password",
	}

	db := getGormDB(*dbInfo)
	db.AutoMigrate(
		&model.Session{},
		&model.User{},
		&model.Group{},
		&model.MarketMakerUser{},
		&model.MarketMakerUserBankCardSetting{},
		&model.WalletUser{},
		&model.WalletUserVerify{},
		&model.WalletUserBankCard{},
		&model.MerchantRole{},
		&model.MerchantUser{},
		&model.MerchantUserStantionMaster{},
		&model.WalletConsoleBank{},
		&model.WalletConsoleRegistrationIPBlocking{},
		&model.WalletConsoleSystemSetting{},
		&model.WalletConsoleTradeSetting{},
		&model.WalletConsoleSeparatedBill{},
		&model.WalletConsoleActivity{},
		&model.WalletConsoleBulletin{},
		&model.MarketMakerBonusSetting{},
		&model.MarketMakerTradeSetting{},
		&model.MarketMakerAccountQuotaSetting{},
		&model.MarketMakerMatchSetting{},
		&model.MarketMakerUserAgreementSetting{},
		&model.MarketMakerRemindSetting{},
		&model.MarketMakerPleaseNoteSetting{},
		&model.MarketMakerIOSSignatureSetting{},
		// 體系必須在merchant前建立，因為merchant會有FK指過來
		&model.MerchantSystem{},
		&model.Merchant{},
		&model.MerchantCorporation{},
		&model.MerchantBankCard{},
		&model.MerchantDomain{},
		&model.MerchantBoardcast{},
		&model.MerchantMarquee{},
		&model.MerchantBulletin{},
		&model.MerchantConsoleSystemSetting{},
		&model.MerchantSetting{},
		&model.MerchantSystemSetting{},
		// // todo 交易相關 可能需要移到帳本
		&model.MarketMakerTaskLog{},
		&model.WalletConsoleRefillAndRecycleLog{},
		&model.WalletConsoleRecycleLog{},
		// // log
		&model.VerificationType{},
		&model.VerificationTitle{},
		&model.Verification{},
		&model.UserLockLog{},
		&model.LoginLog{},
		&model.CorporationWhitelistingLog{},
		&model.MerchantWhitelistingLog{},
		&model.MerchantPasswordResetLog{},
		&model.UnbindMerchantPhoneLog{},
		&model.MerchantOTPLog{},
		&model.MarketMakerUserBankCardLog{},
		// todo trade相關，到時候會放到另外的db裡
		&trade.CoinType{},
		&trade.Order{},
		&trade.LockedOrder{},
		&trade.Transaction{},
		&trade.CanceledTransaction{},
		&trade.Wallet{},
		&trade.WalletDetail{},
	)
}
