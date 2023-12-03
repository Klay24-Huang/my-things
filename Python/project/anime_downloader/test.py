from pydub import AudioSegment

def combine_mp3(file1, file2, output_file):
    audio1 = AudioSegment.from_mp3(file1)
    audio2 = AudioSegment.from_mp3(file2)

    # 合併兩個音軌
    combined_audio = audio1 + audio2

    # 將合併後的音軌儲存為新的檔案
    combined_audio.export(output_file, format="mp3")

# 例子
file1 = rf"C:\others\anime_download\The Eminence in Shadow\1_part1.mp3"
file2 = rf"C:\others\anime_download\The Eminence in Shadow\1_part2.mp3"
output_file = rf"C:\others\anime_download\The Eminence in Shadow\test.mp3"

combine_mp3(file1, file2, output_file)
