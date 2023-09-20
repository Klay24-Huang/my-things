package solution

func gcdOfStrings(str1 string, str2 string) string {
	longStr := str1
	shortStr := str2
	if len(str1) < len(str2) {
		longStr = str2
		shortStr = str1
	}

	for i := len(shortStr); i > 0; i-- {
		if len(shortStr)%i != 0 {
			continue
		}

		subStr := shortStr[:i]
		// fmt.Println(subStr)
		ok := findSubStr(shortStr, longStr, subStr)
		if ok {
			return subStr
		}
	}

	return ""
}

func findSubStr(shortStr string, longStr string, subStr string) bool {
	subStrLen := len(subStr)
	for i := 0; i < len(shortStr); i = i + subStrLen {
		s := shortStr[i : i+subStrLen]
		if s != subStr {
			return false
		}
	}

	for i := 0; i < len(longStr); i = i + subStrLen {
		if i+subStrLen > len(longStr) {
			return false
		}
		s := longStr[i : i+subStrLen]
		if s != subStr {
			return false
		}
	}

	return true
}
