package solution

func pivotIndex(nums []int) int {
	left := 0
	right := sum(nums[1:])

	for i := range nums {
		if left == right {
			return i
		}

		if i == len(nums)-1 {
			break
		}

		left += nums[i]
		right -= nums[i+1]
	}

	return -1
}

func sum(arr []int) int {
	sum := 0
	for _, valueInt := range arr {
		sum += valueInt
	}
	return sum
}
