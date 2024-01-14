from moviepy.editor import *
from pydub import AudioSegment
import config_helper
import file_helper
import os
import time

def start():
    # get all video file
    path = config_helper.get_destination_path()
    root, files = file_helper.get_file_and_root(path)

    # sort by update time
    sorted_files = sorted(files, key=lambda x: os.path.getmtime(os.path.join(path, x)))

    # get config
    config = config_helper.get()

    # get section
    all_sections = config['anime']['episode']['sections']

    for index, file in enumerate(sorted_files):
        print(os.path.join(root, file))
        # list to store file paths
        audio_files = []
        # get video and audio
        video_path = os.path.join(root, file)
        video = VideoFileClip(video_path)

        # extract audio
        audio = video.audio

        # combine need section
        sections = all_sections[index]
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

            # combine all audio files into one using concatenate    
            combined_audio = concatenate_audioclips([AudioFileClip(file) for file in audio_files])

            # save combined audio file
            
            combined_audio.write_audiofile(rf'{path}\{config["anime"]["name"]}_{str(index + 1).zfill(2)}.mp3', codec='mp3', fps=44100)

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

