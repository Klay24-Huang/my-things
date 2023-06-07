package main

import (
	"example/di/buzz.go"
	"example/di/fuzz"

	"github.com/google/wire"
)

var injectSet = wire.NewSet( //nolint:deadcode,unused,varcheck
	buzz.New,
	fuzz.New,
)
