package q10

import "fmt"

func isMatch(s string, p string) bool {
	//fmt.Printf("from start, s is %s, p is %s \n", s, p)
	start := '*'
	indexS := 0 //  index of string
	indexP := 0 // index of pattern
	lenS := len(s)
	lenP := len(p)
	sRunes := []rune(s)
	pRunes := []rune(p)
	for indexS < lenS && indexP < lenP {
		// check next rune of patter is * or not
		nextRuneIsStar := false
		if indexP+1 < lenP {
			nextRuneIsStar = pRunes[indexP+1] == start
		}

		if nextRuneIsStar {
			// current rune of pattern can occur 0 or more times
			// occur 0 time
			occurZeroTimeMatch := isMatch(s[indexS:], p[indexP+2:])
			if occurZeroTimeMatch {
				return true
			}
			// occur n time
			for i := indexS; i < lenS; i++ {
				if !validRune(sRunes[i], pRunes[indexP]) {
					break
				} else {
					if isMatch(s[i+1:], p[indexP+2:]) {
						return true
					}
				}
			}
			return false
		} else {
			// current rune of pattern is letter or dot
			isValidRune := validRune(sRunes[indexS], pRunes[indexP])
			if !isValidRune {
				return false
			}
		}
		indexS++
		indexP++
	}

	// remain pattern probably not need to use
	// a*c*b*
	if indexS == lenS && indexP != lenP {
		for i := indexP + 1; i < lenP; i += 2 {
			if pRunes[i] == start {
				//fmt.Println("in")
				indexP = i + 1
			} else {
				break
			}
		}
	}

	fmt.Printf("%d %d %d %d", indexS, lenS, indexP, lenP)
	return indexS == lenS && indexP == lenP
}

func validRune(sRune rune, pRune rune) bool {
	isDot := pRune == '.'
	if isDot {
		return true
	}

	return sRune == pRune
}
