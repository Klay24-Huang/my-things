package q835

func largestOverlap(img1 [][]int, img2 [][]int) int {
	ans := 0
	length := len(img1)
	outsideOrigin := -(length - 1)
	// out side
	for x1 := outsideOrigin; x1 < length; x1++ {
		for y1 := outsideOrigin; y1 <= length; y1++ {
			// // fmt.Printf("x1: %d ,y1: %d \n", x1, y1)
			// loop img1
			count := 0
			for x2 := 0; x2 < length; x2++ {
				for y2 := 0; y2 < length; y2++ {
					x3, y3 := x2+x1, y2+y1 // img 1 relative point for img2
					// fmt.Printf("img1: [%d,%d], img2: [%d,%d] \n", x2, y2, x3, y3)
					if x3 < 0 || y3 < 0 || x3 >= length || y3 >= length {
						// fmt.Println("continue")
						continue
					}
					block1 := img1[x2][y2]
					block2 := img2[x3][y3]
					if x2 == 0 && y2 == 0 && x3 == 1 && y3 == 1 {
						// fmt.Printf("check block1: %d, block2: %d \n", block1, block2)
					}
					if block1 == 1 && block1 == block2 {
						// fmt.Println("same")
						count++
					}
				}
			}
			if count > ans {
				ans = count
			}
			// fmt.Printf("count is: %d \n", count)
		}
	}
	return ans
}

// func largestOverlap(img1 [][]int, img2 [][]int) int {
// 	ans := 0
// 	length := len(img1)
// 	move := length*2 - 1
// 	// img1 move
// 	for x1 := 0; x1 <= move; x1++ {
// 		for y1 := 0; y1 <= move; y1++ {
// 			// if x1 == 3 && y1 == 3 {

// 			// }
// 			// loop img1
// 			diff := (length - 1)
// 			count := 0
// 			for x2 := 0; x2 < length; x2++ {
// 				for y2 := 0; y2 < length; y2++ {
// 					x3 := x2 - diff
// 					y3 := y2 - diff
// 					if x3 < 0 || y3 < 0 || x3 >= length || y3 >= length {
// 						continue
// 					}
// 					block1 := img1[x2][y2]
// 					block2 := img2[x3][y3]
// 					if block1 == 1 && block1 == block2 {
// 						count++
// 					}
// 				}
// 			}
// 			if count > ans {
// 				ans = count
// 			}
// 		}
// 	}
// 	return ans
// }

// func largestOverlap(img1 [][]int, img2 [][]int) int {
// 	ans := 0
// 	length := len(img1)
// 	for i := 0; i < length; i++ {
// 		for j := 0; j < length; j++ {
// 			overlapCount := checkOverlap(img1, img2, i, j)
// 			if overlapCount > ans {
// 				ans = overlapCount
// 			}
// 		}
// 	}
// 	return ans
// }

// func checkOverlap(img1 [][]int, img2 [][]int, x int, y int) int {
// 	count := 0
// 	length := len(img1)
// 	for i := 0; i < length; i++ {
// 		for j := 0; j < length; j++ {
// 			if i+x >= length || j+y >= length {
// 				continue
// 			}
// 			block2 := img2[i][j]
// 			block1 := img1[i+x][j+y]
// 			// fmt.Printf("block 2: %d, %d, %d \n", i, j, block2)
// 			// fmt.Printf("block 1: %d, %d, %d \n", i+x, j+y, block1)
// 			if block1 == 1 && block1 == block2 {
// 				count++
// 			}
// 		}
// 	}
// 	// fmt.Printf("end. count: %d \n", count)
// 	return count
// }
