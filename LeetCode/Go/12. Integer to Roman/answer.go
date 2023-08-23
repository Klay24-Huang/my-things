package q12

import "strings"

type Recorder struct {
	key   string
	value int
}

func IntToRoman(num int) string {

	romans := []Recorder{
		{"M", 1000},
		{"CM", 900},
		{"D", 500},
		{"CD", 400},
		{"C", 100},
		{"XC", 90},
		{"L", 50},
		{"XL", 40},
		{"X", 10},
		{"IX", 9},
		{"V", 5},
		{"IV", 4},
		{"I", 1},
	}

	var answer strings.Builder

	for _, recorder := range romans {
		if num < recorder.value {
			continue
		}

		for num >= recorder.value {
			num -= recorder.value
			answer.WriteString(recorder.key)
		}

		if num == 0 {
			break
		}
	}

	return answer.String()
}
