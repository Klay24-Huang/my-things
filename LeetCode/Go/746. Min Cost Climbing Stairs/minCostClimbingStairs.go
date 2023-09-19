package solution

func minCostClimbingStairs(cost []int) int {
	n := len(cost)
	dp := make([]int, n)
	dp[0] = cost[0]
	dp[1] = cost[1]

	for i := 2; i < n; i++ {
		dp[i] = cost[i] + min(dp[i-2], dp[i-1])
	}

	// fmt.Println(dp)
	return min(dp[n-2], dp[n-1])
}

func min(num1, num2 int) int {
	if num1 < num2 {
		return num1
	}
	return num2
}
