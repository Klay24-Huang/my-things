package user

import (
	context "context"

	middleware "example.com/grpc-unit-test-example/service/middleware/utils"
)

type IRepository interface {
	Create(ctx context.Context, in *CreateUserRequest) (*CreateUserReply, error)
}

// 隔離讀寫資料概念
type Repository struct {
	middlewareClient middleware.MiddlewareClient
}

func NewRepository(middlewareClient middleware.MiddlewareClient) *Repository {
	return &Repository{middlewareClient: middlewareClient}
}

// 假裝寫入user data到db
func (r *Repository) Create(ctx context.Context, in *CreateUserRequest) (*CreateUserReply, error) {
	reply, err := r.middlewareClient.CreateUser(ctx, &middleware.CreateUserRequest{
		Name:     in.GetName(),
		Password: in.GetPassword(),
	})

	if err != nil {
		return nil, err
	}

	return &CreateUserReply{Result: reply.Result}, nil
}
