package main

import (
	"context"
	"fmt"
	"grpc-golang/pb"
	"log"
	"net"
	"time"

	"google.golang.org/grpc"
)

type server struct {
	pb.GreetingServiceServer
}

func (s *server) Greeting(ctx context.Context, req *pb.GreetingServiceRequest) (*pb.GreetingServiceReply, error) {
	return &pb.GreetingServiceReply{
		Message: fmt.Sprintf("Hello, %s", req.Name),
	}, nil
}

func unaryInterceptor(ctx context.Context, req interface{}, info *grpc.UnaryServerInfo, handler grpc.UnaryHandler) (interface{}, error) {
	// log.Println("request ", req)
	m, err := handler(ctx, req)
	// log.Println("response ", m, err)
	// 记录请求参数 耗时 错误信息等数据
	start := time.Now()
	end := time.Now()
	log.Printf("RPC: %s,req:%v start time: %s, end time: %s, err: %v", info.FullMethod, req, start.Format(time.RFC3339), end.Format(time.RFC3339), err)
	return m, err
}

func main() {
	listener, err := net.Listen("tcp", ":8080")
	if err != nil {
		panic(err)
	}

	s := grpc.NewServer(grpc.UnaryInterceptor(unaryInterceptor))
	pb.RegisterGreetingServiceServer(s, &server{})
	if err := s.Serve(listener); err != nil {
		log.Fatalf("failed to serve: %v", err)
	}
}
