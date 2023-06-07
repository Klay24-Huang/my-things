//go:build wireinject
// +build wireinject

package main

import (
	"github.com/google/wire"
)

func InitializeApplication() (*application, error) {
	wire.Build(
		injectSet,
		newAppliction,
	)
	return &application{}, nil
}
