import keyboard
import time
import threading
import pyautogui

# 定义一个标志，用于控制后台任务的执行
running = False

# 后台任务函数
def background_task():
    global running
    while running:
        print("hello world")
        time.sleep(1)

# 监听键盘事件
def key_listener(e):
    global running
    if e.event_type == keyboard.KEY_DOWN:
        if e.name == 'q' and keyboard.is_pressed('w'):
            running = True
            # 启动后台任务
            # thread = threading.Thread(target=background_task)
            # thread.start()
            print("啟動")
        elif e.name == 'q' and keyboard.is_pressed('e'):
            # 取得mouse locationwq

            current_mouse_x, current_mouse_y = pyautogui.position()

            # 打印坐标
            print(f"当前鼠标坐标: x={current_mouse_x}, y={current_mouse_y}")            
        elif e.name == 'q' and keyboard.is_pressed('r'):
            running = False
            print("程序结束")    

# 设置键盘事件监听
keyboard.on_press(key_listener)

# 保持程序运行
keyboard.wait('esc')  # 按下 "Esc" 键结束程序
