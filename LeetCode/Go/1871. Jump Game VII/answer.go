package q1871

import "fmt"

func canReach(s string, minJump int, maxJump int) bool {
	n := len(s)
	dp := make([]bool, n)
	dp[0] = true

	count := 0
	for i := minJump; i < n; i++ {
		if dp[i-minJump] {
			count++
		}
		if i > maxJump && dp[i-maxJump-1] {
			count--
		}
		if s[i] == '0' && count > 0 {
			dp[i] = true
		}
	}
	fmt.Println(count)
	return dp[n-1]
}
