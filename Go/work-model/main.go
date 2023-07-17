package main

import (
	"appserver/src/database/new/user"
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
	DATABASE = "test"
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
	userDbInfo := &DatabaseInfo{
		Host:     "localhost",
		Port:     "3307",
		Database: "user",
		User:     "admin",
		Password: "password",
	}

	userDb := getGormDB(*userDbInfo)
	userDb.AutoMigrate(
		&user.User{},
		&user.PublicKey{},
		&user.Group{},
		&user.MarketMakerUser{},
		&user.MarketMakerUserBankCardSetting{},
		&user.WalletUser{},
		&user.WalletUserVerify{},
		&user.WalletUserBankCard{},
		&user.Role{},
		&user.MerchantUser{},
		&user.MerchantUserStantionMaster{},
	)

	// walletDbInfo := &DatabaseInfo{
	// 	Host:     "localhost",
	// 	Port:     "3307",
	// 	Database: "wallet",
	// 	User:     "admin",
	// 	Password: "password",
	// }

	// walletDb := getGormDB(*walletDbInfo)

	// walletDb.AutoMigrate(
	// 	&wallet.Bank{},
	// 	&wallet.RegistrationIPBlocking{},
	// 	&wallet.SystemSetting{},
	// 	&wallet.TradeSetting{},
	// 	&wallet.SeparatedBill{},
	// 	&wallet.Activity{},
	// 	&wallet.Bulletin{},
	// )

	// tradeDbInfo := &DatabaseInfo{
	// 	Host:     "localhost",
	// 	Port:     "3307",
	// 	Database: "trade",
	// 	User:     "admin",
	// 	Password: "password",
	// }

	// tradeDb := getGormDB(*tradeDbInfo)

	// tradeDb.AutoMigrate(
	// 	&trade.CoinType{},
	// 	&trade.Order{},
	// 	&trade.LockedOrder{},
	// 	&trade.Transaction{},
	// 	&trade.CanceledTransaction{},
	// 	&trade.Wallet{},
	// 	&trade.WalletDetail{},
	// )

	// logDbInfo := &DatabaseInfo{
	// 	Host:     "localhost",
	// 	Port:     "3307",
	// 	Database: "log",
	// 	User:     "admin",
	// 	Password: "password",
	// }

	// logDb := getGormDB(*logDbInfo)

	// logDb.AutoMigrate(
	// 	&log.VerificationType{},
	// 	&log.VerificationTitle{},
	// 	&log.Verification{},
	// 	&log.UserLockLog{},
	// 	&log.LoginLog{},
	// 	&log.CorporationWhitelistingLog{},
	// 	&log.MerchantWhitelistingLog{},
	// 	&log.MerchantPasswordResetLog{},
	// 	&log.UnbindMerchantPhoneLog{},
	// 	&log.MerchantOTPLog{},
	// 	&log.MarketMakerUserBankCardLog{},
	// 	// todo 交易相關 可能需要移到帳本
	// 	&log.MarketMakerTaskLog{},
	// 	&log.WalletConsoleRefillAndRecycleLog{},
	// 	&log.WalletConsoleRecycleLog{},
	// )

	// loginDbInfo := &DatabaseInfo{
	// 	Host:     "localhost",
	// 	Port:     "3307",
	// 	Database: "login",
	// 	User:     "admin",
	// 	Password: "password",
	// }

	// loginDb := getGormDB(*loginDbInfo)

	// loginDb.AutoMigrate(
	// 	&login.Session{},
	// )

	// marketMakerDbInfo := &DatabaseInfo{
	// 	Host:     "localhost",
	// 	Port:     "3307",
	// 	Database: "market_maker",
	// 	User:     "admin",
	// 	Password: "password",
	// }

	// marketMakerDb := getGormDB(*marketMakerDbInfo)

	// marketMakerDb.AutoMigrate(
	// 	&marketmaker.BonusSetting{},
	// 	&marketmaker.TradeSetting{},
	// 	&marketmaker.AccountQuotaSetting{},
	// 	&marketmaker.MatchSetting{},
	// 	&marketmaker.UserAgreementSetting{},
	// 	&marketmaker.RemindSetting{},
	// 	&marketmaker.PleaseNoteSetting{},
	// 	&marketmaker.IOSSignatureSetting{},
	// 	// todo 確認是否還有此功能
	// 	// &marketmaker.Task{},
	// 	// &marketmaker.Bounus{},
	// )

	// merchantDbInfo := &DatabaseInfo{
	// 	Host:     "localhost",
	// 	Port:     "3307",
	// 	Database: "merchant",
	// 	User:     "admin",
	// 	Password: "password",
	// }

	// merchantDb := getGormDB(*merchantDbInfo)

	// merchantDb.AutoMigrate(
	// 	&merchant.System{},
	// 	&merchant.Corporation{},
	// 	&merchant.Merchant{},
	// 	&merchant.BankCard{},
	// 	&merchant.Domain{},
	// 	&merchant.Boardcast{},
	// 	&merchant.Marquee{},
	// 	&merchant.Bulletin{},
	// 	&merchant.ConsoleSystemSetting{},
	// 	&merchant.Setting{},
	// 	&merchant.SystemSetting{},
	// )

	// // test decimal

	// testDecimalDbInfo := &DatabaseInfo{
	// 	Host:     "localhost",
	// 	Port:     "3307",
	// 	Database: "test_decimal",
	// 	User:     "admin",
	// 	Password: "password",
	// }

	// testDecimalDb := getGormDB(*testDecimalDbInfo)

	// testDecimalDb.AutoMigrate(
	// 	&testdecimal.TestDecimal{},
	// )

	// num, _ := decimal.NewFromString("1.005")
	// testData := &testdecimal.TestDecimal{
	// 	Num: num,
	// }
	// testDecimalDb.Create(testData)
}
