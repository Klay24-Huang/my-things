package q2333

import (
	"sort"
)

func minSumSquareDiff(nums1 []int, nums2 []int, k1 int, k2 int) int64 {
	kSum := k1 + k2
	arr := make([]int, len(nums1)+1)
	arr[0] = 0
	for i := range nums1 {
		arr[i+1] = abs(nums1[i] - nums2[i])
	}
	sort.Ints(arr)
	// fmt.Println(arr)
	var ans int64
	ans = 0
	count := 1 // count for sequenced num have same value

	if kSum == 0 {
		lastNum := arr[len(arr)-1]
		ans += int64(lastNum * lastNum)
	}

	// start from second to last item
	for i := len(arr) - 2; i >= 0; i-- {
		item := arr[i]
		if kSum == 0 {
			ans += int64(item * item)
			// fmt.Println("---: ", ans)
			continue
		}

		previousItem := arr[i+1]
		if item != previousItem {
			gap := previousItem - item
			consumption := gap * count
			// fmt.Println("in: ", previousItem, item, gap, consumption)
			if consumption >= kSum {
				x := kSum / count
				y := kSum % count
				// fmt.Println(kSum, count)
				if y > 0 {
					nums := (previousItem - x - 1)
					ans += int64(nums * nums * y)
					// fmt.Println("a: ", ans)
				}
				nums := (previousItem - x)
				ans += int64(nums * nums * (count - y))
				// fmt.Println("b: ", ans)
				kSum = 0
				ans += int64(item * item)
				// fmt.Println("c: ", ans)
			} else {
				count++
				kSum -= consumption
			}
		} else {
			count++
		}
	}
	return ans
}

func abs(num int) int {
	if num < 0 {
		return -num
	}
	return num
}
