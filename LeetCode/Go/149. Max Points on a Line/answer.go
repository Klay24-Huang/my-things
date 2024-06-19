package q149

import (
	"fmt"
)

type Recorder struct {
	FirstPoint []int
	Count      int
}

func maxPoints(points [][]int) int {
	if len(points) == 1 {
		return 1
	}

	max := 0
	hashMap := make(map[string][]*Recorder, 0)
	for i, pointLeft := range points {
		for j := i + 1; j < len(points); j++ {
			// fmt.Printf("i is: %d, j is %d \n", i, j)
			pointRight := points[j]
			x := pointLeft[0] - pointRight[0]
			y := pointLeft[1] - pointRight[1]
			// if x < 0 {
			// 	y = -1
			// }
			x, y = modifyXAndY(x, y)
			//// // fmt.Printf("modify xy is: [%d,%d] \n", x, y)
			key := fmt.Sprintf("%d:%d", x, y)
			var recorder *Recorder
			recorders, exists := hashMap[key]
			if !exists {
				recorder = &Recorder{
					FirstPoint: pointLeft,
					Count:      2,
				}
				recorders = append(recorders, recorder)

			} else {
				for _, preRecorder := range recorders {
					prePoint := preRecorder.FirstPoint
					isSameLine := isSameLine(pointLeft, prePoint, x, y)
					if isSameLine && (prePoint[0] == pointLeft[0] && prePoint[1] == pointLeft[1]) {
						preRecorder.Count++
						recorder = preRecorder
					} else {
						continue
					}
				}

				if recorder == nil {
					recorder = &Recorder{
						FirstPoint: pointLeft,
						Count:      2,
					}
					recorders = append(recorders, recorder)
				}
			}
			hashMap[key] = recorders

			if recorder.Count > max {
				max = recorder.Count
				// fmt.Printf("max is: %d \n", max)
			}
		}
	}
	// // fmt.Println(hashMap)
	return max
}

func isSameLine(point1 []int, point2 []int, x int, y int) bool {
	// // fmt.Printf("same line x y: %d %d", point1[0]-point2[0], point1[1]-point2[1])
	x2, y2 := point1[0]-point2[0], point1[1]-point2[1]
	if x2 == 0 && y2 == 0 {
		return true
	}
	x2, y2 = modifyXAndY(x2, y2)
	if x != x2 || y != y2 {
		return false
	}
	return true
}

// 斜率 x,y 用最小公因數精簡
func modifyXAndY(x int, y int) (newX int, newY int) {
	a, b := findMaxCommonFactor(x, y)
	commonFactor := a
	if a == 0 {
		commonFactor = b
	}
	return x / commonFactor, y / commonFactor
}

func findMaxCommonFactor(x int, y int) (int, int) {
	// // fmt.Printf("x is: %d, y is %d \n", x, y)
	if x == 0 || y == 0 {
		// // fmt.Println("return")
		return x, y
	}

	xIsMinus := false
	yIsMinus := false

	if x < 0 {
		xIsMinus = true
		x = -x
	}

	if y < 0 {
		yIsMinus = true
		y = -y
	}

	// 輾轉相除
	for x >= y && y != 0 {
		x -= y
	}

	for y >= x && x != 0 {
		y -= x
	}

	if xIsMinus {
		x = -x
	}

	if yIsMinus {
		y = -y
	}

	return findMaxCommonFactor(x, y)
}
