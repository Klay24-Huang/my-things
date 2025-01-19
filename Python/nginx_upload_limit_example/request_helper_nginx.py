import requests
import threading

url = "http://localhost:5000/upload"
large_data = "A" * (1024 * 1024 + 1)  # 1MB + 1 byte

def send_request_continuously():
    """持續發送請求的函數"""
    while True:
        try:
            response = requests.post(url, data=large_data)
            print(f"Status code: {response.status_code}")
            print(f"Response body: {response.text}")
        except Exception as e:
            print(f"Request failed: {e}")

# 創建多個線程來持續發送請求
threads = []
for i in range(10):  # 10 個並發請求
    thread = threading.Thread(target=send_request_continuously)
    threads.append(thread)
    thread.start()

# 主線程保持運行，讓所有線程都繼續工作
for thread in threads:
    thread.join()
