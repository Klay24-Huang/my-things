package solution

func minInsertions(s string) int {
	length := len(s)
	dp := make([][]int, length)
	for i := range dp {
		dp[i] = make([]int, length)
		for j := range dp[i] {
			dp[i][j] = -1
		}
	}

	var helper func(int, int) int
	helper = func(l, r int) int {
		if !(l < r) {
			return 0
		}

		if dp[l][r] > -1 {
			return dp[l][r]
		}

		if s[l] == s[r] {
			dp[l+1][r-1] = helper(l+1, r-1)
			dp[l][r] = dp[l+1][r-1]
			return dp[l][r]
		}

		dp[l][r] = 1 + min(helper(l+1, r), helper(l, r-1))
		return dp[l][r]
	}

	return helper(0, length-1)
}

func min(num1 int, num2 int) int {
	if num1 < num2 {
		return num1
	}
	return num2
}
