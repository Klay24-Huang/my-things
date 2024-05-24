package q242

func isAnagram(s string, t string) bool {
	hashMap := make(map[rune]int)
	// addRuneToMap(s, &hashMap)
	// removeRunToMap(t, &hashMap)
	for _, char := range s {
		if _, exists := hashMap[char]; exists {
			hashMap[char]++
		} else {
			hashMap[char] = 1
		}
	}

	for _, char := range t {
		if value, exists := hashMap[char]; exists {
			value--
			if value == 0 {
				delete(hashMap, char)
			} else {
				hashMap[char] = value
			}
		} else {
			return false
		}

	}

	return len(hashMap) == 0
}

// func addRuneToMap(s string, hashMap *map[rune]int) {
// 	for _, char := range s {
// 		// 這邊應該怎麼寫
// 		if _, exists := (*hashMap)[char]; exists {
// 			(*hashMap)[char]++
// 		} else {
// 			(*hashMap)[char] = 1
// 		}
// 	}
// }

// func removeRunToMap(t string, hashMap *map[rune]int) {
// 	// for _, char := range t {
// 	// 	if value, exists := (*hashMap)[char]; exists {
// 	// 		value--
// 	// 	} else {
// 	// 		(*hashMap)[char]
// 	// 	}

// 	// }
// }
