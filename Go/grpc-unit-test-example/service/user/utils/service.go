package user

import (
	context "context"
	"errors"
)

type IService interface {
	CreateUser(ctx context.Context, in *CreateUserRequest) (*CreateUserReply, error)
}

// 隔離商業邏輯處理
type Service struct {
	repository IRepository
}

func NewService(r IRepository) *Service {
	return &Service{repository: r}
}

// 處理新增user 商業邏輯
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

	// 送到repository 做資料儲存
	return s.repository.Create(ctx, in)
}
