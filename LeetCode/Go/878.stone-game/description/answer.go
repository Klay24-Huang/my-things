package q878

func stoneGame(piles []int) bool {
	last := len(piles) - 1
	alice := 0
	bob := 0
	return step(alice+piles[0], bob, piles[1:]) || step(alice+piles[last], bob, piles[:last])
}

func step(alice int, bob int, piles []int) bool {
	if len(piles) == 0 {
		return alice > bob
	}

	last := len(piles) - 1
	if len(piles)%2 == 0 {
		// alice's turn
		return step(alice+piles[0], bob, piles[1:]) || step(alice+piles[last], bob, piles[:last])
	} else {
		// bob's turn
		return step(alice, bob+piles[0], piles[1:]) || step(alice, bob+piles[last], piles[:last])
	}
}
