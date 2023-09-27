package solution

import "fmt"

func combinationSum3(k int, n int) [][]int {
	ans := make([][]int, 0)
	currArr := []int{}
	sum := 0
	i := 1

	goPrev := func() {
		fmt.Println("before go prev", i, sum, currArr, ans)
		lenCurrArr := len(currArr)
		//[1,2,3] ans 7, i =3 remove 3
		// 1 + 2 +3 - 3
		sum -= currArr[lenCurrArr-1]
		// i = 3
		i := currArr[lenCurrArr-1]
		// [1, 2]
		currArr = currArr[:lenCurrArr-1]
		fmt.Println("after go prev", i, sum, currArr, ans)
	}

	for i <= 9 {
		fmt.Println("start", i, sum, currArr, ans)
		if i > n {
			break
		}

		if len(currArr) < k {
			// // fmt.Println("before append", i, sum, currArr, ans)
			// // fmt.Println("address i", &i)
			if len(ans) == 1 {
				fmt.Println("address ans ", &ans[0][0], &ans[0][1], &ans[0][2])
				// fmt.Println("address ans ", &ans[0][0], &ans[0][0], &ans[0][0])
			}
			if len(currArr) == 2 {
				fmt.Println("address currArr ", &currArr[0], &currArr[1])
			}
			if len(currArr) == 3 {
				fmt.Println("address currArr ", &currArr[0], &currArr[1], &currArr[2])
			}
			// fmt.Println("address i ", &i)
			j := i
			fmt.Println("value i ", i)
			fmt.Println("address i ", &i)
			fmt.Println("address j ", &j)
			currArr = append(currArr, j)
			// fmt.Println("after append", i, sum, currArr, ans)
			sum += i
		}

		// 判斷是否為答案
		if len(currArr) == k {
			if i == 9 && sum < n {
				goPrev()
			}

			if sum == n {
				forAnsArr := currArr
				ans = append(ans, forAnsArr)
				goPrev()
			}

			if sum > n {
				// goPrev()
				break
			}
			goPrev()
		}
		i = i + 1

	}

	return ans
}

// func combinationSum3(k int, n int) [][]int {
// 	ans := make([][]int, 0)

// 	foo := make([]int, 0)

// 	for i := 0; i < 3; i++ {
// 		foo = append(foo, i)
// 		fmt.Println("foo array,", foo)
// 		// fmt.Println("address i", &i)
// 	}

// 	for _, item := range foo {
// 		fmt.Println("foo array address", item, &item)
// 	}
// 	return ans
// }
