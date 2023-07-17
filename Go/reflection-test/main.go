package main

import "fmt"

type foo struct {
	a int
}

type bar struct {
}

var factory map[string]interface{} = map[string]interface{}{
	"foo":          foo{},
	"foo.with.val": foo{2},
}

func main() {
	foo1 := factory["foo"]
	foo2 := factory["foo"]
	fmt.Println("foo1", &foo1, foo1)
	fmt.Println("foo2", &foo2, foo2)

	foowv1 := factory["foo.with.val"].(foo)
	foowv1.a = 123
	foowv2 := factory["foo.with.val"]
	fmt.Println("foowv1", &foowv1, foowv1)
	fmt.Println("foowv2", &foowv2, foowv2)

	bar, ok := factory["foo.with.val"].(bar)
	fmt.Println("ok", ok)
	fmt.Println("bar", &bar, bar)
}
