package solution

import "testing"

func Test_predictPartyVictory(t *testing.T) {
	Radiant := "Radiant"
	Dire := "Dire"
	type args struct {
		senate string
	}
	tests := []struct {
		name string
		args args
		want string
	}{
		{
			name: "case: 1",
			args: args{
				senate: "RD",
			},
			want: Radiant,
		},
		{
			name: "case 2",
			args: args{
				senate: "RDD",
			},
			want: Dire,
		},
		{
			name: "case 3",
			args: args{
				senate: "DR",
			},
			want: Dire,
		},
		{
			name: "case 4",
			args: args{
				senate: "DDRRR",
			},
			want: Dire,
		},
	}
	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := predictPartyVictory(tt.args.senate); got != tt.want {
				t.Errorf("predictPartyVictory() = %v, want %v", got, tt.want)
			}
		})
	}
}
