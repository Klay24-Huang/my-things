import time
import yaml
import os
import pyautogui
from PIL import ImageGrab
import threading

running = True

def stop():
    global running
    running = False
    print('stop by manual')

def start_screenshot_task():
    global running
    thread = threading.Thread(target= screenshot)
    thread.start()

def screenshot():
    print("start")
    # load config
    with open('config.yml', 'r') as yaml_file:
        config = yaml.safe_load(yaml_file)

    # make direct
    folder_path = f'{config['base_path']}\\{config['category']}\\{config['book']['name']}'
    if not os.path.exists(folder_path):
        os.makedirs(folder_path)

    # screenshot and save
    start_at = config['book']['start_at']
    prefix = '#'
    left_top = config['location']['left_top']  # 例如 (100, 100)
    right_bottom = config['location']['right_bottom']  # 例如 (300, 300)

    for page in range(config['book']['total_pages']):
        if not running:
            break

        screenshot = ImageGrab.grab(bbox=(left_top[0], left_top[1], right_bottom[0], right_bottom[1]))

        # file name
        current_page = page + 1
        if current_page >= start_at:
            prefix = ''
            current_page -= (start_at - 1)

        file_name = f'{prefix}{current_page}.png'
        screenshot.save(f"{folder_path}//{file_name}")

        # only screen shot once in test mode
        if config['is_test']:
            break

        # next page
        pyautogui.press('right')

        time.sleep(config['sleep'])