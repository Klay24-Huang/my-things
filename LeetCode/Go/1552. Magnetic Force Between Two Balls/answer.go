package q1552

import (
	"sort"
)

func maxDistance(position []int, m int) int {
	sort.Ints(position)
	if m == 2 {
		return position[1] - position[0]
	}
	hashSet := make(map[int]bool)
	for _, val := range position {
		hashSet[val] = true
	}

	l := 1
	r := len(position) - 1
	maxVal := position[r]
	firstNum := position[0]
	lastNum := position[r]
	gap := 0
	for l < r {
		index := (l + r) / 2
		num := position[index]
		gap = max(num-firstNum, lastNum-num)
		count := 2
		for num < maxVal {
			_, exists := hashSet[num]
			if exists {
				count++
			}
			num += gap
		}
		// gap is too big
		if count < m {
			r = index - 1
		}

		// gap is too small or probable answer
		if count >= m {
			l = index + 1
		}
	}
	return gap
}

// func maxDistance(position []int, m int) int {

// 	hashSet := make(map[int]bool)
// 	min := math.MaxInt32
// 	max := 0

// 	for _, val := range position {
// 		hashSet[val] = true
// 		if val < min {
// 			min = val
// 		}

// 		if val > max {
// 			max = val
// 		}
// 	}
// 	if m == 2 {
// 		return max - min
// 	}

// 	r := max - min // max gap
// 	l := 1         // min gap
// 	gap := 0
// 	for r > l {
// 		gap = (r + l) / 2
// 		count := 2 // first and last basket already put the balls
// 		index := min + gap
// 		fmt.Printf("gap is :%d \n", gap)
// 		for index < max {
// 			_, exists := hashSet[index]
// 			if exists {
// 				count++
// 			}

// 			if count > m {
// 				break
// 			}
// 			index += gap
// 		}

// 		// gap is too big
// 		if count < m {
// 			r = gap - 1
// 		}

// 		// gap is too small or probable answer
// 		if count >= m {
// 			l = gap + 1
// 		}
// 	}
// 	return l - 1
// }
