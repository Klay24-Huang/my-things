#導入webdriver​
from selenium import webdriver
import yaml

# don't need to set up driver path
# https://stackoverflow.com/questions/76461596/unable-to-use-selenium-webdriver-getting-two-exceptions
# def real_yaml():
#     with open('config.yaml', 'r')as stream:
#         return yaml.load(stream, Loader=yaml.FullLoader)

# config = real_yaml()
# print(config)
# print(config['driver_path'])

#建立一個chrome的webdriver
driver = webdriver.Chrome()

#開啟的網址是google首頁
driver.get("https://www.google.com")

#防止網頁馬上開了就關，再執行一次檔案就可以關閉頁面
input()