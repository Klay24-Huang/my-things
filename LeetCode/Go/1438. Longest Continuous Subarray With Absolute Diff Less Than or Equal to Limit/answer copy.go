package q1438

// import "fmt"

// func longestSubarray(nums []int, limit int) int {
// 	lengthNums := len(nums)
// 	r := lengthNums
// 	l := 1

// 	for l <= r {
// 		subArraysLength := (r + l) / 2
// 		fmt.Printf("l %d, r %d, sub length %d \n", l, r, subArraysLength)
// 		isPossibleAnswer := false
// 		for i := 0; i < lengthNums-subArraysLength+1; i++ {
// 			isPossibleAnswer = checkSubArray(nums[i:i+subArraysLength], limit)
// 			if isPossibleAnswer {
// 				break
// 			}
// 		}

// 		if isPossibleAnswer {
// 			l = subArraysLength + 1
// 			continue
// 		}
// 		r = subArraysLength - 1
// 	}
// 	return l - 1
// }

// func checkSubArray(nums []int, limit int) bool {
// 	for i := 0; i < len(nums)-1; i++ {
// 		for j := 1; j < len(nums); j++ {
// 			if abs(nums[i]-nums[j]) > limit {
// 				return false
// 			}
// 		}
// 	}
// 	return true
// }

// func abs(num int) int {
// 	if num < 0 {
// 		return -num
// 	}
// 	return num
// }
