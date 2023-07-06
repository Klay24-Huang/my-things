package login

import (
	"appserver/src/database/new/common"
	"time"
)

type Session struct {
	common.Key `gorm:"primaryKey;"`
	LogoutAt   time.Time
	common.CreatedAt
}
