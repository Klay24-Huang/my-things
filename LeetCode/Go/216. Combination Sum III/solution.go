package solution

import "fmt"

func combinationSum3(k int, n int) [][]int {
	ans := make([][]int, 0)
	currArr := []int{1}
	sum := 1
	i := 2

	goPrev := func() {
		// fmt.Println("go prev", sum, i, currArr)
		lenArr := len(currArr)
		sum -= currArr[lenArr-1]
		i = currArr[lenArr-1] + 1
		currArr = currArr[0 : lenArr-1]
	}

	for i <= 9 {
		fmt.Println(currArr, i, sum)
		if sum >= n {
			break
		}

		lenArr := len(currArr)
		if sum+i == n && lenArr+1 == k {
			// tempArr := currArr
			// tempArr = append(tempArr, i)
			currArr := append(currArr, i)

			ans = append(ans, currArr)
			// fmt.Println("add ans")
			goPrev()
			continue
		}

		if lenArr+1 < k {
			// more than n
			if (sum + i + i + 1) > n {
				if len(currArr) == 0 {
					// no need
					break
				}
				goPrev()
				continue
			}
			currArr = append(currArr, i)
			sum += i
		}
		i++
	}

	return ans
}
