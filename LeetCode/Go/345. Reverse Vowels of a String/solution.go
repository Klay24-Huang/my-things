package solution

func reverseVowels(s string) string {
	runeArr := []rune(s)
	start := 0
	end := len(s) - 1

	myMap := map[rune]bool{
		'a': true,
		'e': true,
		'i': true,
		'o': true,
		'u': true,
		'A': true,
		'E': true,
		'I': true,
		'O': true,
		'U': true,
	}

	for start < end {
		startFind, endFind := myMap[runeArr[start]], myMap[runeArr[end]]
		if !(startFind) {
			start++
			continue
		}

		if !(endFind) {
			end--
			continue
		}

		temp := runeArr[start]
		runeArr[start] = runeArr[end]
		runeArr[end] = temp
		// fmt.Println(string(runeArr))
		start++
		end--
	}
	return string(runeArr)
}
