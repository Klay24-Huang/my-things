package main

func findMaxAverage(nums []int, k int) float64 {
	sum := 0
	temp := 0

	for i, num := range nums {
		if i < k {
			sum += num
			temp = sum
			continue
		}

		// temp := sum
		temp -= nums[i-k]
		temp += num
		if temp > sum {
			sum = temp
		}

		// log.Printf("index is:%v, temp is: %v, sum is: %v", i, temp, sum)
	}
	return float64(sum) / float64(k)
}
