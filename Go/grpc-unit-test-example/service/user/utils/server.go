package user

import context "context"

type Server struct {
	UnimplementedUserServer
	repository IRepository
}

func NewServer(r IRepository) *Server {
	return &Server{repository: r}
}

// 處理商業邏輯的地方
func (s *Server) CreateUser(ctx context.Context, in *CreateUserRequest) (*CreateUserReply, error) {
	userName := in.GetName()
	userPassword := in.GetPassword()
	var result string

	// 做一些帳號資訊上的判斷
	if userName == "" {
		result = "User's name can't be empty."
		return &CreateUserReply{Result: result}, nil
	}

	if userPassword == "" {
		result = "User's password can't be empty."
		return &CreateUserReply{Result: result}, nil
	}

	result = "User Created."

	return &CreateUserReply{Result: result}, nil
}
