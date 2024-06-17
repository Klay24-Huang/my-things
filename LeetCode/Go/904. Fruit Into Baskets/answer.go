package q904

func totalFruit(fruits []int) int {
	typeRecorder := make([]int, 0)
	typeCounter := make([]int, 0)
	dp := make([]int, 0)
	ans := 0
	currentFruit := fruits[0]
	fruitsLen := len(fruits)
	count := 0
	for i, fruit := range fruits {
		if fruit == currentFruit {
			count++
		}

		// next type change
		endOfCurrentType := (i < fruitsLen-1 && fruits[i+1] != fruit) || i == fruitsLen-1
		if endOfCurrentType {
			typeRecorder = append(typeRecorder, currentFruit)
			typeCounter = append(typeCounter, count)
			// not last fruits
			if i != fruitsLen-1 {
				nextFruit := fruits[i+1]
				count = 0
				currentFruit = nextFruit
			}

			temp := 0
			lastIndex := len(typeCounter) - 1
			temp += typeCounter[lastIndex]
			type1 := typeRecorder[lastIndex]
			forwardIndex := lastIndex - 1
			type2 := -1
			// prev type
			if forwardIndex >= 0 {
				type2 = typeRecorder[forwardIndex]
				temp += typeCounter[forwardIndex]
				forwardIndex--
			}

			for ; forwardIndex >= 0; forwardIndex-- {
				if typeRecorder[forwardIndex] != type1 && typeRecorder[forwardIndex] != type2 {
					break
				}
				temp += typeCounter[forwardIndex]
			}

			dp = append(dp, temp)
			if temp > ans {
				ans = temp
			}
		}
	}
	// fmt.Println(typeRecorder)
	// fmt.Println(typeCounter)
	return ans
}
