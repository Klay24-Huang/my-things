//go:build wireinject
// +build wireinject

package main

import (
	"github.com/google/wire"
)

func InitializeApplication() (*application, error) {
	wire.Build(
		buzzSet,
		fuzzSet,
		newAppliction,
	)
	return &application{}, nil
}
