package fuzz

import "example/gomock/foo"

type Fuzz struct {
	Bar foo.Bar
}

func NewFuzz(bar foo.Bar) *Fuzz {
	return &Fuzz{Bar: bar}
}

func (fuzz *Fuzz) GetBar(str string) error {
	return fuzz.Bar.Get(str)
}

// mockgen -source=./foo/foo.go -destination=./mock/foo_mock.go -package=mock
// mockgen --source=./foo/foo.go --destination=./mock/foo_mock.go --package=mock
