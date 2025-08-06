from moviepy.editor import VideoFileClip
import os

def time_str_to_seconds(time_str):
    parts = [int(p) for p in time_str.strip().split(":")]
    if len(parts) == 1:
        return parts[0]
    elif len(parts) == 2:
        return parts[0] * 60 + parts[1]
    elif len(parts) == 3:
        return parts[0] * 3600 + parts[1] * 60 + parts[2]
    else:
        raise ValueError(f"無效的時間格式: {time_str}")

def extract_audio_segments(video_path, segments=None, output_dir="output_mp3s"):
    supported_video_exts = [".mp4", ".mov", ".avi", ".mkv", ".webm"]
    ext = os.path.splitext(video_path)[1].lower()
    if ext not in supported_video_exts:
        raise ValueError(f"不支援的影片格式: {ext}")

    video = VideoFileClip(video_path)
    os.makedirs(output_dir, exist_ok=True)

    if not segments:
        # 沒給段落，輸出整段音訊
        output_path = os.path.join(output_dir, "full_audio.mp3")
        video.audio.write_audiofile(output_path)
        print(f"✅ 已輸出整段音訊: {output_path}")
    else:
        # 有段落，依段落輸出
        for idx, (start_str, end_str) in enumerate(segments):
            start = time_str_to_seconds(start_str)
            end = time_str_to_seconds(end_str)
            audio_clip = video.subclip(start, end).audio
            output_name = f"segment_{idx+1}_{start_str.replace(':', '_')}_to_{end_str.replace(':', '_')}.mp3"
            output_path = os.path.join(output_dir, output_name)
            audio_clip.write_audiofile(output_path)
            print(f"✅ 已輸出: {output_path}")
    
    video.close()

# ✅ 範例段落時間
# segments = [
#     ("00:00:10", "00:00:20"),
#     ("00:01:00", "00:01:15"),
# ]
segments = []

# ✅ 輸入任意支援格式影片
extract_audio_segments(r"C:\others\video\MAYOHR 雲端人資系統-Apollo XE - 11_23線上課程班_FD_PT_PY模組教學-20231123_125944-會議錄製.mp4")
