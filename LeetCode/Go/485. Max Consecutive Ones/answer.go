package q485

func findMaxConsecutiveOnes(nums []int) int {
	maxLength, currentLength := 0, 0

	checkLength := func() {
		if currentLength > maxLength {
			maxLength = currentLength
		}
	}

	for _, num := range nums {
		if num == 0 {
			checkLength()
			currentLength = 0
		} else {
			currentLength++
		}
	}
	checkLength()
	return maxLength
}
