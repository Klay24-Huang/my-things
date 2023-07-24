package main

import (
	"context"
	"log"
	"net"
	"testing"

	user "example.com/grpc-unit-test-example/service/user/utils"
	"github.com/golang/mock/gomock"
	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"
	"google.golang.org/grpc/test/bufconn"
)

func TestFoo(t *testing.T) {
	t.Log("foo")
}

// func TestMockServer(t *testing.T) {
// 	t.Log("start")
// 	ctx := context.Background()

// 	ctl := gomock.NewController(t)
// 	defer ctl.Finish()
// 	mockUserService := user.NewMockIService(ctl)
// 	gomock.InOrder(
// 		mockUserService.EXPECT().CreateUser(ctx, gomock.Any()).AnyTimes().Return(&user.CreateUserReply{
// 			Result: "",
// 		}, nil),
// 	)
// 	userClient, closer := userMockServer(t, ctx, mockUserService)
// 	defer closer()

// 	_, err := userClient.CreateUser(ctx, &user.CreateUserRequest{
// 		Name:     "",
// 		Password: "123",
// 	})
// 	if err != nil {
// 		t.Error("error")
// 	}
// }

func TestCreateUser(t *testing.T) {
	t.Log("start")
	// 所有測試情境
	tests := []struct {
		// 測試情境名稱
		name string
		// request
		in *user.CreateUserRequest
		// 預期測試結果
		expected bool
	}{
		// 所有測試情境
		{
			name: "user name can't be empty.",
			in: &user.CreateUserRequest{
				Name:     "",
				Password: "123",
			},
			expected: false,
		},
		{
			name: "user password can't be empty.",
			in: &user.CreateUserRequest{
				Name:     "John",
				Password: "",
			},
			expected: false,
		},
		{
			name: "create user successfully.",
			in: &user.CreateUserRequest{
				Name:     "abc",
				Password: "456",
			},
			expected: true,
		},
	}
	ctx := context.Background()

	ctl := gomock.NewController(t)
	defer ctl.Finish()
	// // mock repository related functions
	// mockUserRepostiry := user.NewMockIRepository(ctl)
	// gomock.InOrder(
	// 	mockUserRepostiry.EXPECT().Create(ctx, gomock.Any()).AnyTimes().Return(&user.CreateUserReply{
	// 		Result: "",
	// 	}, nil),
	// )

	// userServie := user.NewService(mockUserRepostiry)

	// userClient, closer := userMockServer(t, ctx, userServie)
	mockUserService := user.NewMockIService(ctl)
	// gomock.InOrder(
	// 	mockUserService.EXPECT().CreateUser(ctx, gomock.Any()).AnyTimes().Return(&user.CreateUserReply{
	// 		Result: "",
	// 	}, nil),
	// )
	gomock.InOrder(
		mockUserService.EXPECT().CreateUser(ctx, tests[0].in).Return(&user.CreateUserReply{
			Result: "",
		}, nil),
	)
	userClient, closer := userMockServer(t, ctx, mockUserService)

	defer closer()

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			reply, err := userClient.CreateUser(ctx, tt.in)
			// 是否創建user成功
			// var err error
			// t.Log(userClient)
			successful := err == nil
			if successful != tt.expected {
				t.Errorf("case: '%s' failed, in %v, reply %v", tt.name, tt.in, reply)
			}
		})
	}
}

func userMockServer(t *testing.T, ctx context.Context, s user.IService) (user.UserClient, func()) {
	buffer := 101024 * 1024
	lis := bufconn.Listen(buffer)

	baseServer := grpc.NewServer()
	user.RegisterUserServer(baseServer, user.NewServer(s))
	go func() {
		if err := baseServer.Serve(lis); err != nil {
			log.Printf("error serving server: %v", err)
		}
	}()

	conn, err := grpc.DialContext(ctx, "",
		grpc.WithContextDialer(func(context.Context, string) (net.Conn, error) {
			return lis.Dial()
		}), grpc.WithTransportCredentials(insecure.NewCredentials()))
	if err != nil {
		log.Printf("error connecting to server: %v", err)
		t.FailNow()
	}

	closer := func() {
		err := lis.Close()
		if err != nil {
			log.Printf("error closing listener: %v", err)
			t.FailNow()
		}
		baseServer.Stop()
	}

	client := user.NewUserClient(conn)

	return client, closer
}
