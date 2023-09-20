package solution

import "testing"

func Test_gcdOfStrings(t *testing.T) {
	type args struct {
		str1 string
		str2 string
	}
	tests := []struct {
		name string
		args args
		want string
	}{
		// {
		// 	name: "case: 1",
		// 	args: args{
		// 		str1: "ABCABCABC",
		// 		str2: "ABC",
		// 	},
		// 	want: "ABC",
		// },
		// {
		// 	name: "case: 2",
		// 	args: args{
		// 		str1: "ABABAB",
		// 		str2: "ABAB",
		// 	},
		// 	want: "AB",
		// },
		// {
		// 	name: "case: 3",
		// 	args: args{
		// 		str1: "LEET",
		// 		str2: "CODE",
		// 	},
		// 	want: "",
		// },
		// {
		// 	name: "case: 4",
		// 	args: args{
		// 		str1: "AAA",
		// 		str2: "AAAE",
		// 	},
		// 	want: "",
		// },
		{
			name: "case: 5",
			args: args{
				str1: "A",
				str2: "AAA",
			},
			want: "A",
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := gcdOfStrings(tt.args.str1, tt.args.str2); got != tt.want {
				t.Errorf("gcdOfStrings() = %v, want %v", got, tt.want)
			}
		})
	}
}
