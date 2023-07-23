package main

import (
	"context"
	"log"
	"net"
	"testing"

	user "example.com/grpc-unit-test-example/service/user/utils"
	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"
	"google.golang.org/grpc/test/bufconn"
)

func TestCreateUser(t *testing.T) {
	ctx := context.Background()

	userClient, closer := userMockServer(ctx)
	defer closer()

	tests := []struct {
		// 測試情境名稱
		name string
		// request
		in *user.CreateUserRequest
		// 預期測試結果
		expect bool
	}{
		// 所有測試情境
		{
			name: "user name can't be empty.",
			in: &user.CreateUserRequest{
				Name:     "",
				Password: "123",
			},
			expect: false,
		},
		{
			name: "user password can't be empty.",
			in: &user.CreateUserRequest{
				Name:     "John",
				Password: "",
			},
			expect: false,
		},
		{
			name: "create user successfully.",
			in: &user.CreateUserRequest{
				Name:     "John",
				Password: "123",
			},
			expect: true,
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			reply, err := userClient.CreateUser(ctx, tt.in)
			// 是否創建user成功
			successful := err == nil
			if successful != tt.expect {
				t.Errorf("case: '%s' failed, in %v, reply %v", tt.name, tt.in, reply)
			}
		})
	}
}

func userMockServer(ctx context.Context) (user.UserClient, func()) {
	buffer := 101024 * 1024
	lis := bufconn.Listen(buffer)

	s := grpc.NewServer()
	user.RegisterUserServer(s, user.NewServer(userRepository))
	go func() {
		if err := s.Serve(lis); err != nil {
			log.Printf("error serving server: %v", err)
		}
	}()

	conn, err := grpc.DialContext(ctx, "",
		grpc.WithContextDialer(func(context.Context, string) (net.Conn, error) {
			return lis.Dial()
		}), grpc.WithTransportCredentials(insecure.NewCredentials()))
	if err != nil {
		log.Printf("error connecting to server: %v", err)
	}

	closer := func() {
		err := lis.Close()
		if err != nil {
			log.Printf("error closing listener: %v", err)
		}
		s.Stop()
	}

	client := user.NewUserClient(conn)

	return client, closer
}
