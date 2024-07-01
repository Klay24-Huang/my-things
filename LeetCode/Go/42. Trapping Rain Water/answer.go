package q42

func trap(height []int) int {
	indexesOfLeftWalls := make([]int, 0)
	usedHeight := make([]int, 0)
	count := 0
	for i, h := range height {
		if h > 0 {
			if len(indexesOfLeftWalls) > 0 {
				indexOfPrevH := peek(indexesOfLeftWalls)
				if i-indexOfPrevH == 1 {
					// no gap
					continue
				}

				prevH := height[indexOfPrevH]
				shorterHigh := h // shorter height of this gap's walls
				if prevH <= h {
					shorterHigh = prevH
				}
				prevUsedHeigh := peek(usedHeight)
				count += (shorterHigh - prevUsedHeigh) * (indexOfPrevH - i - 1)

				if prevH <= h {
					indexesOfLeftWalls = pop(indexesOfLeftWalls)
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
