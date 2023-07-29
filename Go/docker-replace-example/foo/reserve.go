package foo

import (
	"bytes"
	"fmt"
	"io/ioutil"
	"os"
)

func main() {

	input, err := ioutil.ReadFile("original.txt")
	if err != nil {
		fmt.Println(err)
		os.Exit(1)
	}

	output := bytes.Replace(input, []byte("replaceme"), []byte("ok"), -1)

	if err = ioutil.WriteFile("modified.txt", output, 0666); err != nil {
		fmt.Println(err)
		os.Exit(1)
	}
}
