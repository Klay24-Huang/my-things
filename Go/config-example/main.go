package main

import (
	"fmt"
	"log"
	"os"

	"github.com/joho/godotenv"
)

func main() {
	err := godotenv.Load("config.env")
	if err != nil {
		log.Fatal("Error loading .env file")
	}

	foo := os.Getenv("FOO")
	fmt.Println(foo)
	// now do something with s3 or whatever
}
