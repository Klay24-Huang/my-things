package q233

import (
	"strconv"
)

func countDigitOne(n int) int {
	if n == 0 {
		return 0
	}

	if n == 1 {
		return 1
	}

	currentCount := 0
	nToString := strconv.Itoa(n)

	for _, char := range nToString {
		if char == '1' {
			currentCount++
		}
	}

	return currentCount + countDigitOne(n-1)
}

// https://leetcode.wang/leetcode-233-Number-of-Digit-One.html
