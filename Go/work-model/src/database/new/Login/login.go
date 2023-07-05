package login

import "appserver/src/database/new/common"

type Session struct {
	common.Key `gorm:"primaryKey;"`
	common.CreatedAt
}
