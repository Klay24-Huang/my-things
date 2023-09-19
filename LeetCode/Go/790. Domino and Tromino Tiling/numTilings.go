package solution

func numTilings(n int) int {
	if n == 1 {
		return 1
	}

	if n == 2 {
		return 2
	}

	if n == 3 {
		return 5
	}

	dp := make([]int, n+1)
	dp[1] = 1
	dp[2] = 2
	dp[3] = 5

	for i := 4; i <= n; i++ {
		dp[i] = (dp[i-1]*2 + dp[i-3]) % 1000000007
	}

	// fmt.Println(dp)
	return dp[n]
}
