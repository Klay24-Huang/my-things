package foo

import (
	bar "example/di/bar"
	"fmt"
)

type FooService struct {
	barService bar.BarService
}

func New(barService bar.BarService) FooService {
	return FooService{
		barService: barService,
	}
}

func PrintFoo() {
	fmt.Println("print foo")
}

func (f FooService) PrintBar() {
	f.barService.PrintBar()
}
