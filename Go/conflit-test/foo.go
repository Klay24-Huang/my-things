package main

type defaultSetting struct {
	a int
	b int
	c int
}

func methodA(df defaultSetting, d int, e int) {

}

func methodB(df defaultSetting, f int, g int) {

}

func main() {
	settings := []defaultSetting{}

	s1 := defaultSetting{
		a: 1,
		b: 2,
		c: 3,
	}
	settings = append(settings, s1)

	s2 := defaultSetting{
		a: 11,
		b: 12,
		c: 13,
	}
	settings = append(settings, s2)

	for _, s := range settings {

		methodA(s, 4, 5)
		methodB(s, 6, 7)
	}
}
