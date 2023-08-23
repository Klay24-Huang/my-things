package q12

import "testing"

func TestExample_1(t *testing.T) {
	// 3, III
	answer := "III"
	result := IntToRoman(3)
	if result != answer {
		t.Errorf("Example 1 not correct, answer: %s, result: %s", answer, result)
	}
}

func TestExample_2(t *testing.T) {
	// 58, LVIII
	answer := "LVIII"
	result := IntToRoman(58)
	if result != answer {
		t.Errorf("Example 2 not correct, answer: %s, result: %s", answer, result)
	}
}

func TestExample_3(t *testing.T) {
	// 1994, MCMXCIV
	answer := "MCMXCIV"
	result := IntToRoman(1994)
	if result != answer {
		t.Errorf("Example 3 not correct, answer: %s, result: %s", answer, result)
	}
}
