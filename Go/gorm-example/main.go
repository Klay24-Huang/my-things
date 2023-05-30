package main

import (
	"fmt"
	"time"

	"gorm.io/driver/mysql"
	"gorm.io/gorm"
	"gorm.io/gorm/schema"
)

type Employee struct {
	ID        int64     // primary key, column name is `id`
	Name      string    // column name is `name`
	Age       int       // column name is `age`
	CreatedAt time.Time // column name is `created_at`
}

const (
	HOST     = "localhost"
	PORT     = "3306"
	DATABASE = "test"
	USER     = "user"
	PASSWORD = "user"
	// SSL      = "disable"
)

func getGormDB() *gorm.DB {
	dsn := fmt.Sprintf(
		// "host=%s port=%s user=%s password=%s dbname=%s sslmode=%s",
		// HOST, PORT, USER, PASSWORD, DATABASE, SSL)
		"%s:%s@tcp(%s:%s)/%s?charset=utf8mb4&parseTime=True&loc=Local",
		USER, PASSWORD, HOST, PORT, DATABASE)

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
	db := getGormDB()

	emp := Employee{}
	db.First(&emp) // SELECT * FROM employee ORDER BY id LIMIT 1;

	fmt.Println(emp) // {1 john 33 2022-11-29 18:44:54.114161 +0000 UTC}
}
