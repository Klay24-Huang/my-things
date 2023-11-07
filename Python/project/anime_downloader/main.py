import keyboard
import worker

# 监听键盘事件
def key_listener(e):
    global running
    if e.event_type == keyboard.KEY_DOWN:
        if e.name == 'q' and keyboard.is_pressed('w'):
            # start screenshot
            worker.start_screenshot_task()
        elif e.name == 'q' and keyboard.is_pressed('e'):
            # stop manual
            worker.stop()

# 设置键盘事件监听
keyboard.on_press(key_listener)

# 保持程序运行
keyboard.wait('esc')  # 按下 "Esc" 键结束程序


# 下載動作
# 從下載資料夾 轉移到儲存資料夾
# 從影片檔抽取音源
# 音源檔除去op ed片段