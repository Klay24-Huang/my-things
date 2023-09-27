package solution

import (
	"reflect"
	"testing"
)

func Test_letterCombinations(t *testing.T) {
	type args struct {
		digits string
	}
	tests := []struct {
		name string
		args args
		want []string
	}{
		// {
		// 	name: "case: only 2",
		// 	args: args{
		// 		digits: "2",
		// 	},
		// 	want: []string{"a", "b", "c"},
		// },
		// {
		// 	name: "case: should be empty",
		// 	args: args{
		// 		digits: "",
		// 	},
		// 	want: []string{},
		// },
		// {
		// 	name: "case: 23",
		// 	args: args{
		// 		digits: "23",
		// 	},
		// 	want: []string{"ad", "ae", "af", "bd", "be", "bf", "cd", "ce", "cf"},
		// },
		{
			name: "case: 234",
			args: args{
				digits: "234",
			},
			want: []string{"adg", "adh", "adi", "aeg", "aeh", "aei", "afg", "afh", "afi", "bdg", "bdh", "bdi", "beg", "beh", "bei", "bfg", "bfh", "bfi", "cdg", "cdh", "cdi", "ceg", "ceh", "cei", "cfg", "cfh", "cfi"},
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := letterCombinations(tt.args.digits); !reflect.DeepEqual(got, tt.want) {
				t.Errorf("letterCombinations() = %v, want %v", got, tt.want)
			}
		})
	}
}
