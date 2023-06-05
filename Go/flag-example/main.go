package main

import (
	"flag"
	"fmt"
)

var (
	intflag    int
	boolflag   bool
	stringflag string
)

func init() {
	flag.IntVar(&intflag, "intflag", 0, "int flag value")
	flag.BoolVar(&boolflag, "boolflag", false, "bool flag value")
	flag.StringVar(&stringflag, "stringflag", "default", "string flag value")
}

func main() {
	flag.Parse()

	fmt.Println("int flag:", intflag)
	fmt.Println("bool flag:", boolflag)
	fmt.Println("string flag:", stringflag)
}

// bool flag needs =
// go run main.go -intflag=12 -boolflag=1 -stringflag=test
// https://gobyexample.com/command-line-flags

// sometimes debug mod will be strange
// remove all breakpoints and run again
