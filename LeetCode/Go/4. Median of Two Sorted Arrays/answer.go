package q4

import (
	"fmt"
	"math"
)

func findMedianSortedArrays(nums1 []int, nums2 []int) float64 {
	len1 := len(nums1)
	len2 := len(nums2)

	if len1+len2 == 0 {
		return 0
	}

	// r := len1 + len2
	// if len1 > 0 {
	// 	r--
	// }

	// if len2 > 0 {
	// 	r--
	// }

	// l := 0

	l1 := 0
	r1 := len1 - 1

	l2 := 0
	r2 := len2 - 1

	min := 0
	max := 0

	for l1 <= r1 || l2 <= r2 {
		nums1End := l1 > r1
		nums2End := l2 > r2
		small1 := getNums(&nums1, l1, true, nums1End)
		small2 := getNums(&nums2, l2, true, nums2End)
		if small1 < small2 {
			l1++
			min = small1
		} else {
			l2++
			min = small2
		}

		large1 := getNums(&nums1, r1, false, nums1End)
		large2 := getNums(&nums2, r2, false, nums2End)
		if large1 > large2 {
			r1--
			max = large1
		} else {
			r2--
			max = large2
		}

		// l = l1 + l2
		// r = r1 + r2
		fmt.Printf("min is: %d, max: %d\n", min, max)
		fmt.Printf("l1 is: %d, r1: %d, l2 is:%d, r2 is %d\n", l1, r1, l2, r2)
	}

	return float64(min+max) / 2
}

func getNums(nums *[]int, index int, getSmall bool, isEnd bool) int {
	if isEnd {
		if getSmall {
			return math.MaxInt32
		}
		return -1
	}
	return (*nums)[index]
}
