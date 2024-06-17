package q29

// func divide(dividend int, divisor int) int {
// 	count := 0
// 	isMinus := 0
// 	recorder := make(map[int]int, 0)

// 	if dividend < 0 {
// 		isMinus++
// 		dividend = -dividend
// 	}

// 	if divisor < 0 {
// 		isMinus++
// 		divisor = -divisor
// 	}

// 	num := divisor
// 	if dividend > divisor {
// 		tempCount := count
// 		if tempCount == 0 {
// 			tempCount++
// 		} else {
// 			tempCount *= 2
// 		}
// 		recorder[tempCount] = num
// 	}

// 	if isMinus == 1 {
// 		count = -count
// 	}
// 	return count
// }

func divide(dividend int, divisor int) int {
	isMinus := 0

	if dividend < 0 {
		isMinus++
		dividend = -dividend
	}

	if divisor < 0 {
		isMinus++
		divisor = -divisor
	}

	count := 0
	for dividend >= divisor && divisor != 1 {
		count++
		dividend -= divisor
		//fmt.Println(dividend)
	}

	if divisor == 1 {
		count = dividend
	}

	if isMinus == 1 {
		count = -count
	}

	if count > 2147483647 {
		return 2147483647
	}

	if count < -2147483648 {
		return -2147483648
	}

	return count
}
