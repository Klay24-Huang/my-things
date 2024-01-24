package main

import (
	"fmt"
	"io"
	"net/http"
	"net/url"
	"os"
	"strings"

	"golang.org/x/net/html"
)

func main() {
	// 輸入要下載的網站URL
	url := "https://sora.komica1.org/78/pixmicat.php?res=29658444"
	fmt.Printf("下載網站: %s\n", url)

	// 下載並保存HTML頁面
	htmlContent, err := downloadHTML(url)
	if err != nil {
		fmt.Printf("錯誤: 無法下載HTML - %s\n", err)
		return
	}

	// 解析HTML，轉換相對URL為絕對URL
	absoluteHTML, err := convertRelativeURLs(url, htmlContent)
	if err != nil {
		fmt.Printf("錯誤: 無法轉換相對URL - %s\n", err)
		return
	}

	// 將HTML保存到文件
	saveHTMLToFile(absoluteHTML, "output.html")
}

// 下載HTML頁面內容
func downloadHTML(url string) (string, error) {
	response, err := http.Get(url)
	if err != nil {
		return "", err
	}
	defer response.Body.Close()

	if response.StatusCode != http.StatusOK {
		return "", fmt.Errorf("HTTP錯誤: %s", response.Status)
	}

	htmlContent, err := io.ReadAll(response.Body)
	if err != nil {
		return "", err
	}

	return string(htmlContent), nil
}

// 解析HTML，轉換相對URL為絕對URL
func convertRelativeURLs(baseURL, htmlContent string) (string, error) {
	tokenizer := html.NewTokenizer(strings.NewReader(htmlContent))
	var absoluteHTML strings.Builder

	for {
		tokenType := tokenizer.Next()

		switch tokenType {
		case html.ErrorToken:
			return absoluteHTML.String(), nil
		case html.StartTagToken, html.SelfClosingTagToken:
			token := tokenizer.Token()

			if token.Data == "img" {
				for i, attr := range token.Attr {
					if attr.Key == "src" {
						// 轉換相對URL為絕對URL
						absoluteURL := resolveURL(baseURL, attr.Val)
						token.Attr[i].Val = absoluteURL
					}
				}
			}
		}

		absoluteHTML.WriteString(tokenizer.Token().String())
	}

	return absoluteHTML.String(), nil
}

// 轉換相對URL為絕對URL
func resolveURL(baseURL, relativeURL string) string {
	u, err := url.Parse(relativeURL)
	if err != nil {
		return relativeURL
	}

	base, err := url.Parse(baseURL)
	if err != nil {
		return relativeURL
	}

	return base.ResolveReference(u).String()
}

// 將HTML保存到文件
func saveHTMLToFile(htmlContent, fileName string) error {
	file, err := os.Create(fileName)
	if err != nil {
		return fmt.Errorf("無法創建文件 %s - %s", fileName, err)
	}
	defer file.Close()

	_, err = file.WriteString(htmlContent)
	if err != nil {
		return fmt.Errorf("無法寫入文件 %s - %s", fileName, err)
	}

	fmt.Printf("HTML保存成功到文件: %s\n", fileName)
	return nil
}

// https://chat.openai.com/c/0c4527b0-9e6d-4894-971d-b9984f91548f
