package main

import (
	"log"
	"net/http"

	"github.com/gin-gonic/gin"
)

func main() {
	r := gin.Default()
	r.GET("/ping", func(c *gin.Context) {
		c.JSON(200, gin.H{
			"message": "pong",
		})
	})

	r.POST("post", func(c *gin.Context) {
		// log.Println(c)
		json := make(map[string]interface{})
		c.BindJSON(&json)
		log.Printf("%v", &json)
		c.JSON(http.StatusOK, gin.H{
			"message": "ok",
		})
	})

	r.Run() // listen and serve on 0.0.0.0:8080
}
