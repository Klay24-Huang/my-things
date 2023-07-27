package main

import (
	"fmt"
	"log"

	"github.com/spf13/viper"
)

func main() {
	viper.SetConfigName("a")
	viper.SetConfigType("yaml")
	viper.AddConfigPath(".")

	err := viper.ReadInConfig()
	if err != nil {
		log.Fatalln(err.Error())
	}

	test := viper.GetString("foo.bar")

	log.Println(test)

	fmt.Println("this is a")
}
