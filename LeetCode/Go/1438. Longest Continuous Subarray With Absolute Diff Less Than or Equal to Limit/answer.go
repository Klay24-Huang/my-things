package q1438

import "fmt"

func longestSubarray(nums []int, limit int) int {
	ans := 1
	min := nums[0]
	max := nums[0]
	subArray := make([]int, 0)
	subArray = append(subArray, 0) // sort sub array of nums by index
	queue := make([]int, 0)
	queue = append(queue, 0) // index of current sub array in order
	// deleteIndex := make(map[int]bool, 0)
	for i := 1; i < len(nums); i++ {
		//print(fmt.Sprintf("i is %d", i))
		queue = append(queue, i)
		subArray = addIndex(nums, subArray, i)
		//print(fmt.Sprintf("sub array is %o", subArray))
		//print(fmt.Sprintf("queue is %o", queue))
		min = nums[subArray[0]]
		max = nums[subArray[len(subArray)-1]]
		if abs(max-min) <= limit {
			if len(subArray) > ans {
				ans = len(subArray)
			}
		} else {
			for abs(max-min) > limit {
				// deleteIndex[queue[0]] = true
				deleteIndex := queue[0]
				queue = queue[1:]
				//print(fmt.Sprintf("remove index %d", deleteIndex))
				subArray = removeIndex(nums, subArray, deleteIndex)
				min = nums[subArray[0]]
				max = nums[subArray[len(subArray)-1]]
			}
			//print(fmt.Sprintf("after remove. sub array is %o", subArray))
		}
	}
	return ans
}

func removeIndex(nums []int, subArray []int, index int) []int {
	num := nums[index]
	l := 0
	r := len(subArray) - 1
	for l <= r {
		mid := (l + r) / 2
		currentNum := nums[subArray[mid]]
		if currentNum > num {
			r = mid - 1
		} else {
			l = mid + 1
		}
	}
	l--
	if l == 0 {
		return subArray[1:]
	}

	if l == len(subArray)-1 {
		return subArray[:len(subArray)-1]
	}

	newArray := subArray[0:l]
	newArray = append(newArray, subArray[l+1:]...)
	return newArray
}

func addIndex(nums []int, subArray []int, index int) []int {
	//print(fmt.Sprintf("addIndex. Sub array is  %o", subArray))
	num := nums[index]
	l := 0
	r := len(subArray) - 1
	for l <= r {
		mid := (l + r) / 2
		currentNum := nums[subArray[mid]]
		if currentNum > num {
			r = mid - 1
		} else {
			l = mid + 1
		}
	}
	//print(fmt.Sprintf("l is %d", l))
	if l == 0 {
		newArray := []int{index}
		return append(newArray, subArray...)
	}

	if l == len(subArray) {
		return append(subArray, index)
	}

	newArray := make([]int, l)
	copy(newArray, subArray[0:l])
	//print(fmt.Sprintf("new array left %o", newArray))
	newArray = append(newArray, index)
	//print(fmt.Sprintf("new array mid %o", newArray))
	newArray = append(newArray, subArray[l:]...)
	//print(fmt.Sprintf("sub array right %o", subArray))
	//print(fmt.Sprintf("new array right %o", subArray[l:]))
	//print(fmt.Sprintf("return new array %o", newArray))
	return newArray
}

func abs(num int) int {
	if num < 0 {
		return -num
	}
	return num
}

func print(s string) {
	fmt.Println(s)
}
