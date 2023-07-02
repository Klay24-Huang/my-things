package login

import "appserver/src/database/new/common"

type Session struct {
	common.ID
	common.Key
	common.CreatedAt
}
