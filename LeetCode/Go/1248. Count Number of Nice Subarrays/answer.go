package q1248

import "fmt"

func numberOfSubarrays(nums []int, k int) int {
	ans := 0
	recorder := make([]int, 0)
	for i, num := range nums {
		isOdd := num%2 == 1
		if isOdd {
			recorder = append(recorder, i)
		}
	}

	if len(recorder) < k {
		return 0
	}
	preIndex := -1
	lastIndex := len(nums)
	for i := k - 1; i < len(recorder); i++ {
		l := recorder[i-(k-1)]
		r := recorder[i]
		nextIndex := lastIndex
		if i+1 < len(recorder) {
			nextIndex = recorder[i+1]
		}
		leftPossible := l - preIndex
		rightPossible := nextIndex - r
		ans += leftPossible * rightPossible
		//fmt.Printf("l is %d, preIndex is %d, nextIndex is %d, r is %d \n", l, preIndex, nextIndex, r)
		//fmt.Printf("ans is %d \n", ans)
		preIndex = l
	}
	return ans
}

func numberOfSubarrays(nums []int, k int) int {
	ret := 0
	i := 0 // start of tail
	tail := 0
	for _, n := range nums {
		// if we hit 1, reset count
		// if cnt 0
		if n%2 == 1 {
			tail = 0
			k--
		}
		for k <= 0 {
			tail++
			if nums[i]%2 == 1 {
				k++
			}
			i++
		}
		ret += tail
		fmt.Println(ret)
	}
	return ret
}
