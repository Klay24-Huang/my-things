package user

import (
	context "context"
)

type Server struct {
	UnimplementedUserServer
	service IService
}

func NewServer(s IService) *Server {
	return &Server{service: s}
}

// 處理商業邏輯的地方
func (s *Server) CreateUser(ctx context.Context, in *CreateUserRequest) (*CreateUserReply, error) {
	// 將security proxy傳進來的泛型data轉型
	// 如果失敗直接return

	// 送到service 處理商業邏輯
	return s.service.CreateUser(ctx, in)
}
