package main

import (
	"example/di/buzz"

	"github.com/google/wire"
)

var buzzSet = wire.NewSet( //nolint:deadcode,unused,varcheck
	buzz.New,
)
