package main

import (
	"fmt"

	"github.com/fatih/structs"
)

type Foo struct {
	A string
}

func main() {
	name := structs.Names(&Foo{})
	fmt.Println(name)
}
