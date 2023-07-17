package testdecimal

import "github.com/shopspring/decimal"

type TestDecimal struct {
	Num decimal.Decimal `gorm:"not null;type:decimal(20,5)"`
}
