package bar

import (
	"fmt"
)

type BarService struct {
}

func New() BarService {
	return BarService{}
}

func (barService BarService) PrintBar() {
	fmt.Println("bar")
}
