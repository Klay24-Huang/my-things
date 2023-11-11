import keyboard
import worker

# 监听键盘事件
def key_listener(e):
    global running
    if e.event_type == keyboard.KEY_DOWN:
        if e.name == 'q' and keyboard.is_pressed('w'):
            # start screenshot
            worker.start()
        elif e.name == 'q' and keyboard.is_pressed('e'):
            # stop manual
            worker.stop()

# 设置键盘事件监听
keyboard.on_press(key_listener)

# 保持程序运行
keyboard.wait('esc')  # 按下 "Esc" 键结束程序


