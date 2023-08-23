package q8

import (
	"math"
	"strconv"
	"strings"
	"unicode"
)

func myAtoi(s string) int {
	started := false
	var recorder strings.Builder

	for _, char := range s {
		isEmpty := char == ' '
		isDot := char == '.'

		if isEmpty && !started {
			// skip empty space from prefix
			continue
		}

		if isEmpty && started {
			// end by first word
			break
		}

		if isDot && started {
			// end by a dot from first word
			break
		}

		if !isEmpty && !started {
			// first rune record
			started = true
		}

		if recorder.Len() > 0 && !unicode.IsDigit(char) {
			// end by not digit rune in first word
			break
		}

		recorder.WriteRune(char)
	}

	firstWord := recorder.String()
	ans, err := strconv.ParseInt(firstWord, 10, 64)

	if err != nil {
		if strings.Contains(err.Error(), "value out of range") {
			firstChar := firstWord[0:1]
			if firstChar == "-" {
				return math.MinInt32
			} else {
				return math.MaxInt32
			}
		}

		return 0
	}

	max := int64(math.MaxInt32)
	min := int64(math.MinInt32)

	if ans > max {
		ans = max
	}

	if ans < min {
		ans = min
	}

	return int(ans)
}
