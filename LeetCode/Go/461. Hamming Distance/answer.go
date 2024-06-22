package q461

func hammingDistance(x int, y int) int {
	ans := 0
	for i := 0; i < 32; i++ {
		bitX := (x >> i) & 1
		bitY := (y >> i) & 1
		if bitX != bitY {
			ans++
		}
	}
	return ans
}
