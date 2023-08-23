package main

func findMaxAverage(nums []int, k int) float64 {
	sum := float64(0)
	temp := float64(0)
	fk := float64(k)

	for i, num := range nums {
		if i < k {
			sum += float64(num)
			temp = sum

			if i == k-1 {
				sum = sum / fk
				temp = temp / fk
			}
			continue
		}

		// temp := sum
		temp -= float64(nums[i-k]) / fk
		temp += float64(num) / fk
		if temp > sum {
			sum = temp
		}

		// log.Printf("index is:%v, temp is: %v, sum is: %v", i, temp, sum)
	}
	return sum
}
