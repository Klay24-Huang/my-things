import keyboard
import pyautogui

# 监听键盘事件
def key_listener(e):
    global running
    if e.event_type == keyboard.KEY_DOWN:
        if e.name == 'q' and keyboard.is_pressed('w'):
            current_mouse_x, current_mouse_y = pyautogui.position()
            # 打印坐标
            print(f"当前鼠标坐标: x, y= [{current_mouse_x} , {current_mouse_y}]")            

# 设置键盘事件监听
keyboard.on_press(key_listener)

# 保持程序运行
keyboard.wait('esc')  # 按下 "Esc" 键结束程序
