package q202

import (
	"strconv"
)

func isHappy(n int) bool {
	myMap := make(map[int]struct{})

	var runIsHappy func(n int) bool
	runIsHappy = func(n int) bool {
		if _, exists := myMap[n]; exists {
			return false
		}
		myMap[n] = struct{}{}

		nToString := strconv.Itoa(n)
		sum := 0
		for _, char := range nToString {
			num := int(char - '0')
			sum += num * num
			// fmt.Println(sum)
		}

		if sum == 1 {
			return true
		}

		return runIsHappy(sum)
	}
	return runIsHappy(n)
}
