package q8

import "testing"

func TestExample_1(t *testing.T) {
	answer := 42
	result := myAtoi("42")
	if result != answer {
		t.Errorf("Example 1 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_2(t *testing.T) {
	answer := -42
	result := myAtoi("   -42")
	if result != answer {
		t.Errorf("Example 2 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_3(t *testing.T) {
	answer := 4193
	result := myAtoi("4193 with words")
	if result != answer {
		t.Errorf("Example 3 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_4(t *testing.T) {
	answer := 0
	result := myAtoi(".42")
	if result != answer {
		t.Errorf("Example 4 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_5(t *testing.T) {
	answer := -2147483648
	result := myAtoi("-91283472332")
	if result != answer {
		t.Errorf("Example 4 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_6(t *testing.T) {
	answer := -12
	result := myAtoi("  -0012a42")
	if result != answer {
		t.Errorf("Example 6 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_7(t *testing.T) {
	answer := -12
	result := myAtoi("  -0000000000000000000000000000000000000000000012")
	if result != answer {
		t.Errorf("Example 7 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_8(t *testing.T) {
	answer := 2147483647
	result := myAtoi("20000000000000000000000000000")
	if result != answer {
		t.Errorf("Example 8 not correct, answer: %d, result: %d", answer, result)
	}
}
