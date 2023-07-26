package main

import (
	"context"
	"log"
	"os"

	"github.com/joho/godotenv"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/bson/primitive"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

type member struct {
	ID primitive.ObjectID `bson:"_id"`
}

func getMongoDb(uri string) *mongo.Client {
	if uri == "" {
		log.Fatal("You must set your 'MONGODB_URI' environmental variable. See\n\t https://www.mongodb.com/docs/drivers/go/current/usage-examples/#environment-variable")
	}

	client, err := mongo.Connect(context.TODO(), options.Client().ApplyURI(uri))
	if err != nil {
		panic(err)
	}

	// defer func() {
	// 	if err := client.Disconnect(context.TODO()); err != nil {
	// 		panic(err)
	// 	}
	// }()

	return client

}

func main() {
	if err := godotenv.Load("config.env"); err != nil {
		log.Println("No .env file found")
	}
	// 連線 .11 mongo db
	mongo11Uri := os.Getenv("MONGO_11")
	mongo11Client := getMongoDb(mongo11Uri)
	memberCollection := mongo11Client.Database("walletdb").Collection("members")

	// 取出5min app 會員cursor
	cursor, err := memberCollection.Find(context.TODO(), bson.D{})
	if err != nil {
		log.Fatal(err)
	}

	// 轉型
	var members []member
	if err = cursor.All(context.TODO(), &members); err != nil {
		log.Fatal(err)
	}

	// member id to string
	var memberids []string
	for _, member := range members {
		memberids = append(memberids, member.ID.Hex())
	}

	// 連線 .12
	mongo12Uri := os.Getenv("MONGO_12")
	mongo12Client := getMongoDb(mongo12Uri)
	userWalletCollection := mongo12Client.Database("Wallet").Collection("userWallet")
	// 刪除無對應會員的錢包
	filter := bson.M{"uid": bson.M{"$nin": memberids}}
	_, err = userWalletCollection.DeleteMany(context.TODO(), filter)

	if err != nil {
		log.Fatal(err)
	}

	// 確認是否有對應不到會員的錢包存在
	// cursor, err = userWalletCollection.Find(context.TODO(), filter)

	// if err != nil {
	// 	log.Fatal(err)
	// }
	// log.Print(cursor)

}
