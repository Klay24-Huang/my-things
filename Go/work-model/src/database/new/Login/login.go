package login

import (
	"appserver/src/database/new/common"
	"time"
)

type Session struct {
	Key        string `gorm:"not null;type:char(255);primaryKey;"`
	LogoutedAt time.Time
	common.CreatedAt
}
