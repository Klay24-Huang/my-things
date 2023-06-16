package main

import (
	"log"
	"testing"
)

func TestMain(m *testing.M) {
	log.Println("test main.")
	m.Run()
}
