package main

func uniquePaths(m int, n int) int {
	dp := make([][]int, m)
	for i := range dp {
		dp[i] = make([]int, n)
	}

	for i := range dp[0] {
		dp[0][i] = 1
	}

	for j := range dp {
		dp[j][0] = 1
	}

	for i, row := range dp {
		if i == 0 {
			continue
		}
		for j := range row {
			if j == 0 {
				continue
			}
			dp[i][j] = dp[i-1][j] + dp[i][j-1]
		}
	}

	return dp[m-1][n-1]
}
