# 音訊擷取工具 - 從影片切割指定區段為 MP3

這個 Python 工具可從常見影片檔案（例如 `.mp4`, `.mkv`, `.mov`, `.avi`, `.webm`）中，擷取多個時間區段，並轉存為多個 MP3 音訊檔案。

---

## 📦 環境安裝教學

### 1️⃣ 建立虛擬環境（建議使用 Python 3.8+）

```bash
# 建立虛擬環境
python -m venv venv

# 啟動虛擬環境
# Windows:
venv\Scripts\activate

# macOS / Linux:
source venv/bin/activate
```

## 安裝 Python 套件
pip install -r requirements.txt

## 🔧 安裝 ffmpeg
moviepy 使用 ffmpeg 進行影片/音訊處理，請依你的系統安裝：