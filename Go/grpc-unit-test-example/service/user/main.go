package main

import (
	"flag"
	"fmt"
	"log"
	"net"

	"google.golang.org/grpc"
	"google.golang.org/grpc/credentials/insecure"

	middleware "example.com/grpc-unit-test-example/service/middleware/utils"
	user "example.com/grpc-unit-test-example/service/user/utils"
)

var (
	port           = flag.Int("port", 50051, "The user server port")
	middlewareAddr = flag.String("middleware_addr", "localhost:50052", "middleware address")
)

func main() {
	flag.Parse()
	// create listen
	lis, err := net.Listen("tcp", fmt.Sprintf(":%d", *port))
	if err != nil {
		log.Fatalf("user server failed to listen: %v", err)
	}
	// create middleware client
	middlewareClient, err := getMiddlewareClient()
	if err != nil {
		log.Printf("could not create middleware client: %v", err)
	}

	// create repository
	userRepository := user.NewRepository(middlewareClient)

	// create server
	s := grpc.NewServer()
	user.RegisterUserServer(s, user.NewServer(userRepository))
	log.Printf("user server listening at %v", lis.Addr())
	if err := s.Serve(lis); err != nil {
		log.Fatalf("user server failed to serve: %v", err)
	}
}

func getMiddlewareClient() (middleware.MiddlewareClient, error) {
	// Set up a connection to the server.
	conn, err := grpc.Dial(*middlewareAddr, grpc.WithTransportCredentials(insecure.NewCredentials()))
	if err != nil {
		return nil, err
	}
	defer conn.Close()

	return middleware.NewMiddlewareClient(conn), nil
}
