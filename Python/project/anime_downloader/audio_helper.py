from moviepy.editor import *
from pydub import AudioSegment
import config_helper
import file_helper
import os

def start():
    # get all video file
    path = config_helper.get_destination_path()
    root, files = file_helper.get_file_and_root(path)

    # sort by create time
    sorted_files = sorted(files, key=lambda x: os.path.getctime(os.path.join(path, x)))

    # get section
    all_sections = config_helper.get()['anime']['episode']['sections']

    for index, file in enumerate(sorted_files):
        # list to store file paths
        audio_files = []
        # get video and audio
        video_path = os.path.join(root, file)
        video = VideoFileClip(video_path)

        # extract audio
        audio = video.audio

        # combine need section
        sections = all_sections[index]
        print(len(sections))
        if len(sections) == 0:
            # if no section, get all 
            audio_file_path = rf'{path}\{index + 1}.mp3'
            audio.write_audiofile(audio_file_path, codec='mp3', fps=44100)
            continue
        else:
            for section in sections:
                start_time_ms = time_to_ms(section[0])
                end_time_ms = time_to_ms(section[1])
                slice_audio = audio.subclip(start_time_ms/1000, end_time_ms/1000)

                # write each slice to file
                audio_file_path = rf'{path}\{index + 1}_part{len(audio_files) + 1}.mp3'
                slice_audio.write_audiofile(audio_file_path, codec='mp3', fps=44100)
                audio_files.append(audio_file_path)

            # combine all audio files into one
            combined_audio = None
            for file in audio_files:
                print(file)
                print(os.path.exists(file))
                # 絕對路徑中包含了反斜線字符 \，而在 Python 中 \ 是轉義字符，可能會引`起問題。
                audio_segment = AudioSegment.from_file(file.replace("\\", "/"), format="mp3")
                if combined_audio is None:
                    combined_audio = audio_segment
                else:
                    combined_audio += audio_segment

            # save combined audio file
            combined_audio.export(rf'{path}\{index + 1}.mp3', format="mp3")

        # remove part mp3 files
        for audio_file in audio_files:
            os.remove(audio_file)

        # No need to explicitly close video file (handled by MoviePy)

        # 先保留以免操作错误
        # remove video

def time_to_ms(time_str):
    # 将时间字符串（例如"1:30"）转换为毫秒
    minutes, seconds = map(int, time_str.split(":"))
    return (minutes * 60 + seconds) * 1000


# def extract_audio(video_path: str, audio_path: str):
#     # 載入影片文件
#     video = VideoFileClip(video_path)

#     # 抽取音源
#     audio = video.audio

#     # 將音源保存為音訊文件
#     audio.write_audiofile(audio_path)

#     # 釋放資源
#     audio.close()
#     video.close()


# def remove_audio_segment(input_file, output_file, start_time, end_time):
#     # 讀取音訊文件
#     audio = AudioSegment.from_file(input_file, format="mp3")

#     # 計算開始和結束時間的毫秒
#     start_time_ms = time_to_ms(start_time)
#     end_time_ms = time_to_ms(end_time)

#     # 刪除指定時間範圍的音訊
#     audio = audio[:start_time_ms] + audio[end_time_ms:]

#     # 將結果保存到輸出文件
#     audio.export(output_file, format="mp3")

# # 使用示例
# input_file = "your_audio_file.mp3"
# output_file = "output_audio_file.mp3"
# start_time = "1:30"
# end_time = "3:15"

# remove_audio_segment(input_file, output_file, start_time, end_time)

