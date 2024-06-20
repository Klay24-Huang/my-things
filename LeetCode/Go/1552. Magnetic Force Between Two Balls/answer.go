package q1552

import "math"

func maxDistance(position []int, m int) int {
	hashSet := make(map[int]bool)
	min := math.MaxInt32
	max := 0

	for _, val := range position {
		hashSet[val] = true
		if val < min {
			min = val
		}

		if val > max {
			max = val
		}
	}

	if m == 2 {
		return max - min
	}

	betweens := m - 1
	bestGap := (max - min) / betweens
	for bestGap > 1 {
		count := 2 // first and last basket already put the balls
		index := min + bestGap
		for index < max {
			_, exists := hashSet[index]
			if exists {
				count++
			}
		}
		if count == m {
			return bestGap
		}
	}

	return bestGap
}
