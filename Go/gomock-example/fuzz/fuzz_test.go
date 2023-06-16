package fuzz

import (
	"example/gomock/mock"
	"testing"

	"github.com/golang/mock/gomock"
)

func Test_GetFuzz(t *testing.T) {
	ctl := gomock.NewController(t)
	defer ctl.Finish()

	str := "some text"
	mockBar := mock.NewMockBar(ctl)
	gomock.InOrder(
		mockBar.EXPECT().Get(str).Return(nil),
	)

	fuzz := NewFuzz(mockBar)
	err := fuzz.GetBar(str)
	if err != nil {
		t.Errorf("fuzz.getBar error%v", err)
	}
}
