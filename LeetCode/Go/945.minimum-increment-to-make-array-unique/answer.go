package q945

import (
	"fmt"
	"sort"
)

func minIncrementForUnique(nums []int) int {
	sort.Ints(nums)
	fmt.Println(nums)
	ans := 0
	x := 0 // 需要處理得數字數量
	prev := 0
	current := nums[0]
	for i := 1; i < len(nums); i++ {
		num := nums[i]
		if num == current {
			x++
		} else {
			temp := current
			current = num
			prev = temp
			for x > 0 && prev+1 != current {
				ans += x
				x--
				prev++
			}
			if num == prev+1 {
				ans += x
				//fmt.Println(ans)
			}

		}
		//fmt.Println(num, ans, prev, current, x)
	}
	//fmt.Println(ans)
	for x > 0 {
		ans += x
		x--
	}
	return ans
}
