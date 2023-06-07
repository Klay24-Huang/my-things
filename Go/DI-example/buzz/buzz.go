package buzz

import (
	"example/di/fuzz"
)

type BuzzService struct {
	fuzzService fuzz.FuzzService
}

func New(fuzzService fuzz.FuzzService) BuzzService {
	return BuzzService{
		fuzzService: fuzzService,
	}
}

// func PrintFoo() {
// 	fmt.Println("print foo")
// }

func (b BuzzService) PrintBuzz() {
	b.fuzzService.PrintFuzz()
}
