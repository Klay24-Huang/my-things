package main

import (
	"log"
)

func main() {
	// userRepository := user.NewRepository()
	// userService := user.NewService(userRepository)
	// log.Println(userService.GetById(1))

	userService := InitializeUserService()
	log.Println(userService.GetById(1))
}

// wire gen ./...
