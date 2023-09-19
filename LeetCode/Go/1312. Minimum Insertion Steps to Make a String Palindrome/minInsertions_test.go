package solution

import "testing"

func Test_minInsertions(t *testing.T) {
	type args struct {
		s string
	}
	tests := []struct {
		name string
		args args
		want int
	}{
		{
			name: "case 1",
			args: args{s: "ad"},
			want: 1,
		},
		{
			name: "case 2",
			args: args{s: "aa"},
			want: 0,
		},
		{
			name: "case 3",
			args: args{s: "mbadm"},
			want: 2,
		},
		{
			name: "case 4",
			args: args{s: "leetcode"},
			want: 5,
		},
		{
			name: "case 5",
			args: args{s: "tldjbqjdogipebqsohdypcxjqkrqltpgviqtqz"},
			want: 25,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := minInsertions(tt.args.s); got != tt.want {
				t.Errorf("minInsertions() = %v, want %v", got, tt.want)
			}
		})
	}
}
