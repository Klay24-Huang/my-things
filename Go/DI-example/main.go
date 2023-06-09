package main

import (
	"example/di/bar"
	"example/di/buzz.go"
	"example/di/foo"
	"example/di/fuzz"
	"fmt"
)

type application struct {
	fuzzService fuzz.FuzzService
	buzzService buzz.BuzzService
}

func newAppliction(
	fuzzService fuzz.FuzzService,
	buzzService buzz.BuzzService,
) *application {
	return &application{
		fuzzService: fuzzService,
		buzzService: buzzService,
	}
}

func main() {
	barService := bar.New()
	fooService := foo.New(barService)
	fooService.PrintBar()

	app, err := InitializeApplication()
	if err != nil {
		fmt.Println("error")
	}

	app.buzzService.PrintBuzz()
}

// wire gen ./...
// go run main.go wire_gen.go
