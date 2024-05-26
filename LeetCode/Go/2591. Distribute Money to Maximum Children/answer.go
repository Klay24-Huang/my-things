package q2591

func distMoney(money int, children int) int {
	if money < children {
		return -1
	}

	if money < 8 {
		return 0
	}

	arr := make([]int, children)
	for i := range arr {
		arr[i] = 1
		money--
	}

	for i := range arr {
		if money-7 < 0 {
			arr[i] += money
			money = 0
			break
		}
		arr[i] += 7
		money -= 7
	}

	if money > 0 {
		arr[len(arr)-1] += money
	}

	if arr[len(arr)-1] == 4 {
		arr[len(arr)-1] -= 1
		arr[len(arr)-2] += 1
	}

	count := 0
	for _, item := range arr {
		if item == 8 {
			count++
		} else {
			break
		}
	}
	// fmt.Println(arr)
	return count
}

// func distMoney(money int, children int) int {
// 	if money < children {
// 		return -1
// 	}

// 	if money < 8 {
// 		return 0
// 	}

// 	money -= children

// 	if money == 0 {
// 		return 0
// 	}

// 	i := children
// 	for i > 0 && money-7 > 0 {
// 		money -= 7
// 		i--
// 	}

// 	if i == children {
// 		return 0
// 	}

// 	adjust := 0
// 	if money > 0 {
// 		adjust = -1
// 	}

// 	if money < 0 {
// 		adjust = -2
// 	}

// 	return children + adjust
// }
