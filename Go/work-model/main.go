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
		&user.MerchantUserRole{},
		&user.MerchantUserStantionMaster{},
	)

}
