package q1588

func sumOddLengthSubarrays(arr []int) int {
	ans := 0
	windowLength := 1
	arrLen := len(arr)

	goWindow := func(windowLength int) {
		tempSum := 0
		for i := 0; i < windowLength; i++ {
			tempSum += arr[i]
			// fmt.Println(ans)
		}
		ans += tempSum
		for i := windowLength; i < arrLen; i++ {
			tempSum += arr[i]
			tempSum -= arr[i-windowLength]
			ans += tempSum
		}
	}

	for windowLength <= arrLen {
		goWindow(windowLength)
		windowLength += 2
	}
	return ans
}
