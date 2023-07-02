package wallet

import (
	"appserver/src/database/new/common"
)

// /////////////////// yapay ///////////////////////
type WalletGroup struct {
	common.ID
	common.Group
	common.CreateAtAndUpdateAt
}

type WalletBank struct {
	common.ID
	BankID   string `gorm:"unique;not null;type:varchar(30);"`
	BankName string `gorm:"unique;not null;type:varchar(30);"`
	common.Applicant
	common.CreateAtAndUpdateAt
}
