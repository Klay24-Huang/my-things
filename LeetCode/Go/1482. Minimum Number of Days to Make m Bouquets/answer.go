package q1482

import (
	"math"
)

// func minDays(bloomDay []int, m int, k int) int {
// 	if m*k > len(bloomDay) {
// 		return -1
// 	}

// 	arrLength := len(bloomDay)
// 	hashmap := make(map[int]int, 0)
// 	maxDay := 0
// 	minDay := math.MaxInt32
// 	leftIndex := 0
// 	maxExplode := 0 // 曾經走到最遠的index

// 	for i, day := range bloomDay {
// 		hashmap[day] = i
// 		if day > maxDay {
// 			maxDay = day
// 		}

// 		if day < minDay {
// 			minDay = day
// 		}
// 	}

// 	// i == current day
// 	for i := minDay; i <= maxDay; i++ {
// 		// // fmt.printf("day is %d \n", i)
// 		rightIndex, exists := hashmap[i]
// 		if maxExplode > rightIndex {
// 			rightIndex = maxExplode
// 		}
// 		// if i <= 3 {
// 		// 	// fmt.printf("day %d, left Index is %d, right Index is %d, exists %t \n", i, leftIndex, rightIndex, exists)
// 		// }
// 		if exists {
// 			count := 0              // 計算連續開花長度
// 			flowerContinued := true // 紀錄left index前的花是否都使用
// 			tempM := m
// 			for j := leftIndex; j < arrLength; j++ {
// 				currentFlower := bloomDay[j]
// 				if currentFlower > i {
// 					flowerContinued = false
// 					count = 0
// 					continue
// 				}
// 				maxExplode = j
// 				// if i <= 12 {
// 				// 	// fmt.printf("current flower is %d \n", currentFlower)
// 				// }
// 				count++
// 				if count == k {
// 					count = 0
// 					tempM--
// 					//// fmt.printf("bouquet create at day %d, index %d, temp M %d \n", i, j, tempM)
// 					if tempM == 0 {
// 						return i
// 					}
// 					if flowerContinued {
// 						m--
// 						leftIndex = j + 1
// 						// // fmt.printf("left index is %d \n", leftIndex)
// 					}
// 				}
// 				// if i == 2 {
// 				// 	// fmt.printf("j is %d, rightIndex is %d \n", j, rightIndex)
// 				// }
// 				if j+1 < arrLength {
// 					nextFlower := bloomDay[j+1]
// 					// if i == 2 {
// 					// 	// fmt.printf("next flower is %d \n", nextFlower)
// 					// }
// 					if nextFlower != currentFlower {
// 						// end of flowerContinued flower
// 						flowerContinued = false
// 					}
// 				}
// 			}
// 		} else {
// 			continue
// 		}
// 		// if i == 2 {
// 		// 	// fmt.printf("day 2, left Index is %d, right Index is %d, exists %t \n", leftIndex, rightIndex, exists)
// 		// }
// 	}
// 	return -1
// }

func minDays(bloomDay []int, m int, k int) int {
	if m*k > len(bloomDay) {
		return -1
	}

	min := math.MaxInt32
	max := 0
	totalLength := len(bloomDay)

	for _, day := range bloomDay {
		if day > max {
			max = day
		}

		if day < min {
			min = day
		}
	}

	for min <= max {
		currentDay := (min + max) / 2
		// fmt.printf("current is %d \n", currentDay)
		recorder := make([]int, totalLength)
		for i, day := range bloomDay {
			if currentDay >= day {
				recorder[i] = 1
			}
		}
		// // fmt.println(recorder)
		if currentDay == 4969 {
			// fmt.println(recorder)
			foo := make([]int, totalLength)
			for i, day := range bloomDay {
				if 4948 >= day {
					foo[i] = 1
				}
			}
			// fmt.println(foo)
		}

		count := 0
		tempM := m
		tempMaxDay := 0
		for i, hasFlower := range recorder {
			if hasFlower == 0 {
				count = 0
				continue
			}

			day := bloomDay[i]
			count++
			if day > tempMaxDay {
				tempMaxDay = day
			}
			if count == k {
				tempM--
				//// fmt.printf("day is %d, flower is %d, flower index is %d,temp m is: %d \n", currentDay, day, i, tempM)
				count = 0
			}

			if i+1 < totalLength && recorder[i+1] != 1 {
				// // fmt.printf("count reset. flower is %d, flower index is %d \n", day, i)
				count = 0
			}

		}
		// fmt.printf("temp m is %d \n", tempM)
		if tempM == 0 {
			// fmt.println("a")
			return tempMaxDay
		}

		if tempM < 0 && min == max {
			// fmt.println("b")
			return tempMaxDay
		}

		if tempM > 0 {
			min = currentDay + 1
		}

		if tempM < 0 {
			max = currentDay - 1
		}
	}

	return -1
}
