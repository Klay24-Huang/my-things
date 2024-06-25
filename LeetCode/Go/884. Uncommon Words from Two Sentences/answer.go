package q884

import "strings"

func uncommonFromSentences(s1 string, s2 string) []string {
	words1 := strings.Split(s1, " ")
	words2 := strings.Split(s2, " ")
	words := append(words1, words2...)
	hashSet := make(map[string]int)
	for _, word := range words {
		_, exists := hashSet[word]
		if !exists {
			hashSet[word] = 1
		} else {
			hashSet[word]++
		}
	}

	ans := make([]string, 0)
	for key, val := range hashSet {
		if val == 1 {
			ans = append(ans, key)
		}
	}
	return ans
}
