package q995

func minKBitFlips(nums []int, k int) int {
	count := 0
	queue := make([]int, 0)
	for i, num := range nums {
		lenOfQueue := len(queue)
		if lenOfQueue > 0 && i > queue[0] {
			queue = queue[1:]
		}
		lenOfQueue = len(queue)
		// fmt.Println(queue)

		flip := lenOfQueue%2 == 1
		if flip {
			if num == 1 {
				num = 0
			} else {
				num = 1
			}
		}
		// fmt.Printf("origin num is: %d, len of queue is %d, after flip num is: %d \n", nums[i], lenOfQueue, num)

		if num == 0 {
			count++
			index := i + k - 1
			if index > len(nums)-1 {
				return -1
			}
			//fmt.Println(index)
			queue = append(queue, index)
		}
	}
	return count
}
