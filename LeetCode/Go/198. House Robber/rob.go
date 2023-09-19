package solution

func rob(num []int) int {
	n := len(num)

	if n == 1 {
		return num[0]
	}

	if n == 2 {
		return max(num[0], num[1])
	}

	dp := make([]int, n)
	dp[0] = num[0]
	dp[1] = num[1]
	dp[2] = num[0] + num[2]

	for i := 3; i < n; i++ {
		dp[i] = num[i] + max(dp[i-3], dp[i-2])
	}

	return max(dp[n-1], dp[n-2])
}

func max(num1, num2 int) int {
	if num1 > num2 {
		return num1
	}
	return num2
}
