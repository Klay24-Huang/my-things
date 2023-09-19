package solution

import "testing"

func Test_rob(t *testing.T) {
	type args struct {
		num []int
	}
	tests := []struct {
		name string
		args args
		want int
	}{
		{
			name: "case 1",
			args: args{
				num: []int{
					1, 2, 3, 1,
				},
			},
			want: 4,
		},
		{
			name: "case 2",
			args: args{
				num: []int{
					2, 7, 9, 3, 1,
				},
			},
			want: 12,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := rob(tt.args.num); got != tt.want {
				t.Errorf("rob() = %v, want %v", got, tt.want)
			}
		})
	}
}
