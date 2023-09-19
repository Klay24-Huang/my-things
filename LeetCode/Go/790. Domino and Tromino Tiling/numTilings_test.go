package solution

import "testing"

func Test_numTilings(t *testing.T) {
	type args struct {
		n int
	}
	tests := []struct {
		name string
		args args
		want int
	}{
		{name: "case 1",
			args: args{
				n: 5,
			},
			want: 24,
		},
		{name: "case 2",
			args: args{
				n: 10,
			},
			want: 1255,
		},
		{name: "case 3",
			args: args{
				n: 1000,
			},
			want: 979232805,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := numTilings(tt.args.n); got != tt.want {
				t.Errorf("numTilings() = %v, want %v", got, tt.want)
			}
		})
	}
}
