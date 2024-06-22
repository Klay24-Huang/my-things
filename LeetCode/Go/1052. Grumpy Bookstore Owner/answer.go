package q1052

func maxSatisfied(customers []int, grumpy []int, minutes int) int {
	ans := 0
	for i, customer := range customers {
		isGrumpy := grumpy[i] == 1
		if isGrumpy {
		} else {
			ans += customer
		}
	}

	for i := 0; i < minutes; i++ {
		isGrumpy := grumpy[i] == 1
		if isGrumpy {
			ans += customers[i]
		}
	}

	temp := ans
	for i := minutes; i < len(customers); i++ {
		lastGrumpy := grumpy[i-minutes] == 1
		if lastGrumpy {
			temp -= customers[i-minutes]
		}

		currentGrumpy := grumpy[i] == 1
		if currentGrumpy {
			temp += customers[i]
		}

		if temp > ans {
			ans = temp
		}
	}

	return ans
}
