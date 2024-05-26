package q15

import "sort"

func threeSum(nums []int) [][]int {
	sort.Ints(nums)
	results := [][]int{}

	for i := 0; i < len(nums)-2; i++ {
		if i > 0 && nums[i] == nums[i-1] {
			continue // skip duplicates for the first number
		}

		left, right := i+1, len(nums)-1

		for left < right {
			sum := nums[i] + nums[left] + nums[right]
			if sum == 0 {
				results = append(results, []int{nums[i], nums[left], nums[right]})

				for left < right && nums[left] == nums[left+1] {
					left++ // skip duplicates for the second number
				}
				for left < right && nums[right] == nums[right-1] {
					right-- // skip duplicates for the third number
				}

				left++
				right--
			} else if sum < 0 {
				left++
			} else {
				right--
			}
		}
	}

	return results
}
