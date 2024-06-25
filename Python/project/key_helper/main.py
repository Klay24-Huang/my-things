import tkinter as tk
import pyautogui

def press_down():
    pyautogui.press('down')

def resize(event):
    # 設置按鈕的相對寬度和高度，使其佔視窗70%的面積
    btn_down.place(relwidth=0.7, relheight=0.7, relx=0.5, rely=0.5, anchor='center')

# 創建主視窗
root = tk.Tk()
root.title("方向鍵模擬器")

# 設置視窗大小
root.geometry("300x300")  # 初始大小
root.minsize(200, 200)    # 最小大小

# 創建「往下」按鈕並綁定事件
btn_down = tk.Button(root, text="↓", command=press_down)
btn_down.place(relwidth=0.7, relheight=0.7, relx=0.5, rely=0.5, anchor='center')  # 初始置中

# 綁定視窗大小改變事件
root.bind('<Configure>', resize)

# 運行主循環
root.mainloop()
