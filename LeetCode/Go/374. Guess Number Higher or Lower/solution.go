package solution

func guess(num int) int {
	return 0
}

func guessNumber(n int) int {
	start := 1
	end := n
	currentNum := (start + end) / 2

	for info := guess(currentNum); info != 0; {
		if info == -1 {
			end = currentNum - 1
		}

		if info == 1 {
			start = currentNum + 1
		}
		currentNum = (start + end) / 2
		info = guess(currentNum)
		//fmt.Println(info, currentNum)
	}
	return currentNum
}
