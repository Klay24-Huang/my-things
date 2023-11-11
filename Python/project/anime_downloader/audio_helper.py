from moviepy.editor import *
from pydub import AudioSegment
from pydub.playback import play

def start():
    print(0)

def extract_audio(video_path: str, audio_path: str):
    # 載入影片文件
    video = VideoFileClip(video_path)

    # 抽取音源
    audio = video.audio

    # 將音源保存為音訊文件
    audio.write_audiofile(audio_path)

    # 釋放資源
    audio.close()
    video.close()


def remove_audio_segment(input_file, output_file, start_time, end_time):
    # 讀取音訊文件
    audio = AudioSegment.from_file(input_file, format="mp3")

    # 計算開始和結束時間的毫秒
    start_time_ms = time_to_ms(start_time)
    end_time_ms = time_to_ms(end_time)

    # 刪除指定時間範圍的音訊
    audio = audio[:start_time_ms] + audio[end_time_ms:]

    # 將結果保存到輸出文件
    audio.export(output_file, format="mp3")

def time_to_ms(time_str):
    # 將時間字符串（例如"1:30"）轉換為毫秒
    minutes, seconds = map(int, time_str.split(":"))
    return (minutes * 60 + seconds) * 1000

# 使用示例
input_file = "your_audio_file.mp3"
output_file = "output_audio_file.mp3"
start_time = "1:30"
end_time = "3:15"

remove_audio_segment(input_file, output_file, start_time, end_time)

