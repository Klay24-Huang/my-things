package main

import (
	"example/di/fuzz"

	"github.com/google/wire"
)

var fuzzSet = wire.NewSet( //nolint:deadcode,unused,varcheck
	fuzz.New,
)
