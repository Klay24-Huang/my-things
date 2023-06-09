// Code generated by Wire. DO NOT EDIT.

//go:generate go run github.com/google/wire/cmd/wire
//go:build !wireinject
// +build !wireinject

package main

import (
	"example/di/buzz.go"
	"example/di/fuzz"
)

// Injectors from wire.go:

func InitializeApplication() (*application, error) {
	fuzzService := fuzz.New()
	buzzService := buzz.New(fuzzService)
	mainApplication := newAppliction(fuzzService, buzzService)
	return mainApplication, nil
}
