from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from bs4 import BeautifulSoup

def check_for_video(url):
    options = webdriver.ChromeOptions()
    options.add_argument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36")
    options.add_argument("--disable-images")
    # options.add_argument("--headless")
    # 使用Selenium打开浏览器
    driver = webdriver.Chrome(options=options)

    try:
        # 打开指定的网页
        driver.get(url)

        # 获取页面内容
        page_source = driver.page_source

        # 使用BeautifulSoup解析HTML内容
        soup = BeautifulSoup(page_source, 'html.parser')

        # 查找页面中的视频标签
        video_tags = soup.find_all('video')

        if video_tags:
            print("页面包含视频")
        else:
            print("页面不包含视频")

    except Exception as e:
        print(f"发生错误: {str(e)}")

    finally:
        # 关闭浏览器
        driver.quit()

# 指定要检查的网页URL
url_to_check = "https://aniwatch.to/watch/the-witch-and-the-beast-18850?ep=115003"

# 调用函数检查页面是否包含视频
check_for_video(url_to_check)


# 紀錄目前安裝套件
# pip freeze > requirements.txt