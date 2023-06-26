package main

import "testing"

func TestMyFunc(t *testing.T) {
	num := runMyFunc()
	if num != 1 {
		t.Error("failed.")
	}
}
