package main

import "fmt"

type type1 []struct {
	Field1 string
	Field2 int
}
type type2 []struct {
	Field1 string
	Field2 string
}

func main() {
	t1 := type1{{"A", 1}, {"B", 2}}
	t2 := type2(t1)
	fmt.Println(t2)
}
