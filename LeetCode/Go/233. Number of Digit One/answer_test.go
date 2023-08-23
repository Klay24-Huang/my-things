package q233

import "testing"

func TestExample_1(t *testing.T) {
	num := 13
	answer := 6
	result := countDigitOne(num)
	if result != answer {
		t.Errorf("Example 1 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_2(t *testing.T) {
	num := 0
	answer := 0
	result := countDigitOne(num)
	if result != answer {
		t.Errorf("Example 2 not correct, answer: %d, result: %d", answer, result)
	}
}

// func TestExample_3(t *testing.T) {
// 	num := 1000000000
// 	answer := 900000001
// 	result := countDigitOne(num)
// 	if result != answer {
// 		t.Errorf("Example 3 not correct, answer: %d, result: %d", answer, result)
// 	}
// }

// func TestExample_4(t *testing.T) {
// 	num := 123456789
// 	answer := 130589849
// 	result := countDigitOne(num)
// 	if result != answer {
// 		t.Errorf("Example 4 not correct, answer: %d, result: %d", answer, result)
// 	}
// }

func TestExample_5(t *testing.T) {
	num := 1000
	answer := 301
	result := countDigitOne(num)
	if result != answer {
		t.Errorf("Example 5 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_6(t *testing.T) {
	num := 100000
	answer := 50001
	result := countDigitOne(num)
	if result != answer {
		t.Errorf("Example 6 not correct, answer: %d, result: %d", answer, result)
	}
}

func TestExample_7(t *testing.T) {
	num := 10000000
	answer := 7000001
	result := countDigitOne(num)
	if result != answer {
		t.Errorf("Example 7 not correct, answer: %d, result: %d", answer, result)
	}
}
