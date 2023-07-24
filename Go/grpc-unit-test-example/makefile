grpc-gen-user:
	protoc --go_out=. --go_opt=paths=source_relative \
	--go-grpc_out=. --go-grpc_opt=paths=source_relative \
	./service/user/utils/user.proto

grpc-gen-middleware:
	protoc --go_out=. --go_opt=paths=source_relative \
	--go-grpc_out=. --go-grpc_opt=paths=source_relative \
	./service/middleware/utils/middleware.proto

mockgen-user:
	mockgen --source=./service/user/utils/repository.go \
	--destination=./service/user/utils/repository_mock.go \
	--package=user
    
test:
	go test ./...