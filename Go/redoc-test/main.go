package main

import (
	"github.com/Kotodian/go-redoc"
	ginredoc "github.com/Kotodian/go-redoc/gin"
	"github.com/gin-gonic/gin"
)

func main() {
	doc := redoc.Redoc{
		Title:       "Example API",
		Description: "Example API Description",
		SpecFile:    "./openapi.yaml",
		SpecPath:    "/openapi.yaml",
		DocsPath:    "/docs",
	}

	r := gin.Default()
	r.Use(ginredoc.New(doc))

	r.GET("/ping", func(c *gin.Context) {
		c.JSON(200, gin.H{
			"message": "pong",
		})
	})
	r.Run() // listen and serve on 0.0.0.0:8080
}
