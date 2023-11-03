import pyautogui
from PIL import ImageGrab

# 左上角坐标和右下角坐标
left_top = (100, 100)  # 例如 (100, 100)
right_bottom = (300, 300)  # 例如 (300, 300)

# 截取屏幕截图
screenshot = ImageGrab.grab(bbox=(left_top[0], left_top[1], right_bottom[0], right_bottom[1]))

# 保存截图为文件（可选）
screenshot.save("screenshot.png")

# 显示截图（可选）
screenshot.show()
