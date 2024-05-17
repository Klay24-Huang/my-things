package q1

func twoSum(nums []int, target int) []int {
	myMap := make(map[int]int) // value , index
	for i, value := range nums {
		num := nums[i]
		if index, exists := myMap[target-num]; exists {
			return []int{index, i}
		}
		myMap[value] = i
	}
	return []int{}
}
