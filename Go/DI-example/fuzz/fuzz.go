package fuzz

import (
	"fmt"
)

type FuzzService struct {
}

func New() FuzzService {
	return FuzzService{}
}

func (fuzz FuzzService) PrintFuzz() {
	fmt.Println("fuzz")
}
