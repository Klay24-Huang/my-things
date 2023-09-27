package solution

func letterCombinations(digits string) []string {
	myMap := map[rune][]string{
		'2': {"a", "b", "c"},
		'3': {"d", "e", "f"},
		'4': {"g", "h", "i"},
		'5': {"j", "k", "l"},
		'6': {"m", "n", "o"},
		'7': {"p", "q", "r", "s"},
		'8': {"t", "u", "v"},
		'9': {"w", "x", "y", "z"},
	}

	digLen := len(digits)
	ans := make([]string, 0)

	if digLen == 0 {
		return ans
	}

	recorder := make([]int, digLen)
	for i := range recorder {
		recorder[i] = -1
	}

	doWork := true
	// index of recorder or combine str
	i := 0

	appendAns := func() {
		// fmt.Println("append")
		str := ""
		// fmt.Println(recorder)
		for i, j := range recorder {
			if i != digLen-1 {
				str += myMap[rune(digits[i])][j]
			} else {
				for k, char := range myMap[rune(digits[i])] {
					recorder[i] = k
					ans = append(ans, str+char)
				}
			}
		}
	}

	var goPrev func()
	goPrev = func() {
		// fmt.Println(recorder)
		recorder[i] = -1
		i--
		// fmt.Println(i)
		if recorder[i]+1 > len(myMap[rune(digits[i])])-1 {
			goPrev()
		} else {
			recorder[i] = recorder[i] + 1
		}
	}

	for doWork {
		// fmt.Println(recorder, i)
		// index of current digit's char
		if recorder[i] == -1 {
			// first time use this digit
			recorder[i] = 0
		}
		// go next digit
		i++

		if i >= digLen {
			// go back to last index of digits
			i--
			// push str to ans
			appendAns()
			// fmt.Println(ans)
			doWork = func() bool {
				for i, singleDigLen := range recorder {
					// fmt.Println(len(myMap[rune(digits[i])])-1, singleDigLen)
					if len(myMap[rune(digits[i])])-1 != singleDigLen {
						goPrev()
						// fmt.Println(recorder)
						return true
					}
				}
				return false
			}()

		}

	}

	return ans
}
