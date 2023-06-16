package main

import "testing"

func TestMyMain(t *testing.T) {
	userService := InitializeUserService()
	user := userService.GetById(1)
	if user.Id != 1 {
		t.Fatal("failed")
	}
}
