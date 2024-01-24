import os
import requests
from bs4 import BeautifulSoup
import cssutils

def download_images(images, folder):
    if not os.path.exists(folder):
        os.makedirs(folder)

    for img in images:
        img_url = img['src']
        if not img_url.startswith('https:'):
            # If not, prepend 'https://'
            img_url = 'https:' + img_url
        img_data = requests.get(img_url).content
        img_name = os.path.join(folder, os.path.basename(img_url))
        with open(img_name, 'wb') as img_file:
            img_file.write(img_data)

def download_and_save_html(url, output_html):
    response = requests.get(url)
    soup = BeautifulSoup(response.text, 'html.parser')

    # 下載並保存所有圖片
    # images = soup.find_all('img')
    # download_images(images, 'images')

    # 提取CSS
    # style_tags = soup.find_all('style')
    # css_content = ''
    # for style_tag in style_tags:
    #     css_content += style_tag.string + '\n'

    # # 將外部CSS文件加入
    # link_tags = soup.find_all('link', rel='stylesheet')
    # for link_tag in link_tags:
    #     css_url = link_tag['href']
    #     css_response = requests.get(css_url)
    #     css_content += css_response.text + '\n'

    # 保存HTML文件
    with open(output_html, 'w', encoding='utf-8') as html_file:
        html_file.write(soup.prettify())

    # 保存CSS文件
    # with open('styles.css', 'w', encoding='utf-8') as css_file:
    #     css_file.write(css_content)

if __name__ == '__main__':
    spa_url = 'https://sora.komica1.org/78/pixmicat.php?res=29658444'
    output_html_file = 'output.html'

    download_and_save_html(spa_url, output_html_file)


    
# pip freeze > requirements.txt