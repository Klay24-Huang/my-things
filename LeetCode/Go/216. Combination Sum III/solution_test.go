package solution

import (
	"reflect"
	"testing"
)

func Test_combinationSum3(t *testing.T) {
	type args struct {
		k int
		n int
	}
	tests := []struct {
		name string
		args args
		want [][]int
	}{
		{
			name: "case 1",
			args: args{
				k: 3,
				n: 7,
			},
			want: [][]int{{1, 2, 4}},
		},
		// {
		// 	name: "case 2",
		// 	args: args{
		// 		k: 3,
		// 		n: 9,
		// 	},
		// 	want: [][]int{{1, 2, 6}, {1, 3, 5}, {2, 3, 4}},
		// },
		// {
		// 	name: "case 3",
		// 	args: args{
		// 		k: 4,
		// 		n: 1,
		// 	},
		// 	want: [][]int{},
		// },
		// {
		// 	name: "case 4",
		// 	args: args{
		// 		k: 2,
		// 		n: 6,
		// 	},
		// 	want: [][]int{{1, 5}, {2, 4}},
		// },
		// {
		// 	name: "case 5",
		// 	args: args{
		// 		k: 3,
		// 		n: 15,
		// 	},
		// 	want: [][]int{{1, 5, 9}, {1, 6, 8}, {2, 4, 9}, {2, 5, 8}, {2, 6, 7}, {3, 4, 8}, {3, 5, 7}, {4, 5, 6}},
		// },
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := combinationSum3(tt.args.k, tt.args.n); !reflect.DeepEqual(got, tt.want) {
				t.Errorf("combinationSum3() = %v, want %v", got, tt.want)
			}
		})
	}
}
