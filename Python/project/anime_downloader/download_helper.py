import config_helper
import pyautogui
import time

def start():
    config = config_helper.get()
    anime_location_config = config['anime']['position']
    wait_for_anime_play = config['download_tool']['wait']['play']

    # click english dub
    dub_btn_location  = anime_location_config['dub']
    pyautogui.moveTo(dub_btn_location[0], dub_btn_location[1], duration=1)
    pyautogui.click()
    time.sleep(wait_for_anime_play)

    download_tool_config = config['download_tool']
    for index in range(config['anime']['episode']['count']):
        # click play btn
        # play_btn_location = anime_location_config['play']
        # pyautogui.moveTo(play_btn_location[0], play_btn_location[1], duration=1)
        # pyautogui.click()

        # open coconut download window
        move_mouse_and_click(download_tool_config['position']['icon'])

        # open download page
        # coconut will auto switch to loading page
        move_mouse_and_click(download_tool_config['position']['open_download_page'])

        # wait loading and download
        time.sleep(download_tool_config['wait']['loading'])
        move_mouse_and_click(download_tool_config['position']['loading'])
        # close loading page
        pyautogui.hotkey('ctrl', 'w')

        # go to next episode
        move_mouse_and_click(anime_location_config['next'])

        time.sleep(wait_for_anime_play)


def move_mouse_and_click(location):
    pyautogui.moveTo(location[0], location[1], duration=1)
    pyautogui.click()