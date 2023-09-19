package solution

import "testing"

func Test_trobonacci(t *testing.T) {
	type args struct {
		n int
	}
	tests := []struct {
		name string
		args args
		want int
	}{
		{
			name: "case 1",
			args: args{
				n: 4,
			},
			want: 4,
		},
		{
			name: "case 2",
			args: args{
				n: 25,
			},
			want: 1389537,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := trobonacci(tt.args.n); got != tt.want {
				t.Errorf("trobonacci() = %v, want %v", got, tt.want)
			}
		})
	}
}
