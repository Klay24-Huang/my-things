package q42

func trap(height []int) int {
	indexesOfLeftWalls := make([]int, 0)
	usedHeight := make([]int, 0)
	count := 0
	for i, h := range height {
		if h > 0 {
			if len(indexesOfLeftWalls) > 0 {
				checkPrevWallsFlag := true
				preWallLowerOrEqual := true
				for checkPrevWallsFlag {
					indexOfPrevH := peek(indexesOfLeftWalls)
					prevH := height[indexOfPrevH]
					preWallLowerOrEqual = prevH <= h
					lowerHigh := h // shorter height of this gap's walls
					if preWallLowerOrEqual {
						lowerHigh = prevH
					}
					prevUsedHeigh := peek(usedHeight)
					count += (lowerHigh - prevUsedHeigh) * (indexOfPrevH - i - 1)

					if preWallLowerOrEqual {
						indexesOfLeftWalls = pop(indexesOfLeftWalls)
					}

					if len(indexesOfLeftWalls) == 0 || !preWallLowerOrEqual {
						checkPrevWallsFlag = false
					}
				}
				if preWallLowerOrEqual {
					usedHeight = usedHeight[:0]
				}
				usedHeight = append(usedHeight, h)
			}
			indexesOfLeftWalls = append(indexesOfLeftWalls, i)
		}
	}
	return count
}

// peek last append item
func peek(arr []int) int {
	lenArr := len(arr)
	if lenArr == 0 {
		return 0
	}
	return arr[lenArr-1]
}

func pop(arr []int) []int {
	lenArr := len(arr)
	if lenArr == 0 {
		return arr
	}
	return arr[:lenArr-1]
}
