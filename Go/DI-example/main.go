package main

import (
	"example/di/bar"
	"example/di/foo"
)

func main() {
	barService := bar.New()
	fooService := foo.New(barService)
	fooService.PrintBar()
}
