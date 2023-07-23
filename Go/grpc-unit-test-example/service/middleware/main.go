package main

import (
	"flag"
	"fmt"
	"log"
	"net"

	middleware "example.com/grpc-unit-test-example/service/middleware/utils"
	"google.golang.org/grpc"
)

var (
	port = flag.Int("port", 50052, "The middleware server port")
)

func main() {
	flag.Parse()
	// create listen
	lis, err := net.Listen("tcp", fmt.Sprintf(":%d", *port))
	if err != nil {
		log.Fatalf("middleware server failed to listen: %v", err)
	}
	// create server
	s := grpc.NewServer()
	middleware.RegisterMiddlewareServer(s, &middleware.Server{})
	log.Printf("middleware server listening at %v", lis.Addr())
	if err := s.Serve(lis); err != nil {
		log.Fatalf("middleware server failed to serve: %v", err)
	}
}
