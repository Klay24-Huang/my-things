package q826

import (
	"sort"
)

type by struct {
	Indices []int
	Values  []int
}

func (b by) Len() int           { return len(b.Values) }
func (b by) Less(i, j int) bool { return b.Indices[i] < b.Indices[j] }
func (b by) Swap(i, j int) {
	b.Indices[i], b.Indices[j] = b.Indices[j], b.Indices[i]
	b.Values[i], b.Values[j] = b.Values[j], b.Values[i]
}

func maxProfitAssignment(difficulty []int, profit []int, worker []int) int {
	ans := 0
	sort.Sort(by{Indices: difficulty, Values: profit})
	//fmt.Println(difficulty)
	// fmt.Println(profit)
	for _, w := range worker {
		index := findDifficult(w, &difficulty)
		if index != -1 {
			ans += findProfit(index, &profit)
			// fmt.Printf("%d %d %d %d %d \n", w, index, difficulty[index], findProfit(index, &profit), ans)
		} else {
			// fmt.Printf("%d %d %d %d %d \n", w, index, 0, 0, ans)
		}
	}
	return ans
}

func findDifficult(work int, difficulty *[]int) int {
	// fmt.Println("find index for work")
	// fmt.Println(work)
	l, r := 0, len(*difficulty)-1
	lastIndex := r

	if work > (*&*difficulty)[lastIndex] {
		return lastIndex
	}

	for l <= r {
		mid := (l + r) / 2
		d := (*difficulty)[mid]
		// fmt.Println("d is : ")
		// fmt.Println(d)
		if d == work {
			if mid+1 <= lastIndex && (*difficulty)[mid+1] == work {
				l = mid + 1
			} else {
				return mid
			}
		}

		if d < work {
			if mid+1 <= lastIndex && (*difficulty)[mid+1] > work {
				return mid
			}
			l = mid + 1
		}

		if d > work {
			if mid-1 >= 0 && (*difficulty)[mid-1] < work {
				return mid - 1
			}
			r = mid - 1
		}
	}

	return -1
}

func findProfit(index int, profit *[]int) int {
	max := 0
	for i := 0; i <= index; i++ {
		p := (*profit)[i]
		if p > max {
			max = p
		}
	}
	return max
}
