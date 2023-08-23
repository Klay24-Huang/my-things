package main

import (
	"testing"
)

func Test_findMaxAverage(t *testing.T) {
	type arg struct {
		nums []int
		k    int
	}

	type test struct {
		title string
		arg
		expect float64
	}

	tests := []test{
		{
			title: "case 1",
			arg: arg{
				nums: []int{1, 12, -5, -6, 50, 3},
				k:    4,
			},
			expect: 12.75,
		},
		{
			title: "case 2",
			arg: arg{
				nums: []int{5},
				k:    1,
			},
			expect: 5,
		},
		{
			title: "case 3",
			arg: arg{
				nums: []int{0, 4, 0, 3, 2},
				k:    1,
			},
			expect: 4,
		},
		{
			title: "case 5",
			arg: arg{
				nums: []int{4, 0, 4, 3, 3},
				k:    5,
			},
			expect: 2.8,
		},
	}

	for _, tt := range tests {
		t.Run(tt.title, func(t *testing.T) {
			if ans := findMaxAverage(tt.arg.nums, tt.arg.k); ans != tt.expect {
				t.Errorf("title = %s, expect: %v, got: %v", tt.title, tt.expect, ans)
			}
		})
	}
}
