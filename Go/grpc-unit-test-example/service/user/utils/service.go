package user

import (
	context "context"
	"errors"
)

type IService interface {
	CreateUser(ctx context.Context, in *CreateUserRequest) (*CreateUserReply, error)
}

type Service struct {
	repository IRepository
}

func NewService(r IRepository) *Service {
	return &Service{repository: r}
}

// 處理商業邏輯的地方
func (s *Service) CreateUser(ctx context.Context, in *CreateUserRequest) (*CreateUserReply, error) {
	userName := in.GetName()
	userPassword := in.GetPassword()
	var result string

	// 做一些帳號資訊上的判斷
	if userName == "" {
		result = "user's name can't be empty"
		return nil, errors.New(result)
	}

	if userPassword == "" {
		result = "user's password can't be empty"
		return nil, errors.New(result)
	}

	return s.repository.Create(ctx, in)
}
