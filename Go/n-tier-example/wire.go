//go:build wireinject
// +build wireinject

package main

import (
	"example/n-tier-example/service/user"

	"github.com/google/wire"
)

func InitializeUserService() *user.UserService {
	wire.Build(user.NewRepository, user.NewService)
	return &user.UserService{}
}
