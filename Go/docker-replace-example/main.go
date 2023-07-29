package main

import (
	"bytes"
	"fmt"
	"io/ioutil"
	"os"
)

func main() {

	template, err := ioutil.ReadFile("template.dockerfile")
	if err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	env, err := ioutil.ReadFile("env.dockerfile")
	if err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	output := bytes.Replace(template, []byte("### insert"), env, -1)

	if err = ioutil.WriteFile("foo.dockerfile", output, 0666); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}
}
