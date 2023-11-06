import yaml
import os
import pyautogui
from PIL import ImageGrab

# load config
with open('config.yml', 'r') as yaml_file:
    config = yaml.safe_load(yaml_file)

# make direct
folder_path = f'{config['base_path']}\\{config['category']}\\{config['book']['name']}'
if not os.path.exists(folder_path):
    os.makedirs(folder_path)

# screenshot and save# screenshot and save
start_at = config['book']['start_at']
prefix = '#'
left_top = config['book']['top_left']  # 例如 (100, 100)
right_bottom = config['book']['bottom_right']  # 例如 (300, 300)

for page in range(config['book']['total_pages']):

    screenshot = ImageGrab.grab(bbox=(left_top[0], left_top[1], right_bottom[0], right_bottom[1]))

    # file name
    if prefix == '#' and current_page >= start_at:
        prefix = ''

    current_page = page + 1

    file_name = f'{prefix}{current_page}.png'
    screenshot.save(f"{folder_path}//{file_name}")

    # next page
    pyautogui.press('right')
