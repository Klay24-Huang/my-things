import os
from moviepy.editor import VideoFileClip

# 設定資料夾路徑
folder_path = 'C:\\Users\\sean2\\Downloads\\dungeon meshi'

# 遍歷資料夾中的所有檔案
for filename in os.listdir(folder_path):
    if filename.endswith(".mkv"):
        video_path = os.path.join(folder_path, filename)
        audio_path = os.path.join(folder_path, os.path.splitext(filename)[0] + '.mp3')
        
        # 擷取音訊並儲存為 MP3
        with VideoFileClip(video_path) as video:
            audio = video.audio
            audio.write_audiofile(audio_path)

print("音訊擷取完成。")
