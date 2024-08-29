import os
import re

def process_vtt_file(input_file_path, output_file_path):
    with open(input_file_path, 'r', encoding='utf-8') as file:
        vtt_content = file.read()
    
    pattern = r'(\d{2}:\d{2}:\d{2}\.\d{3}) --> (\d{2}:\d{02}:\d{2}\.\d{3}).*?\n((?:<c\.bg_transparent>.*?</c\.bg_transparent>\n?)*)'
    
    matches = re.findall(pattern, vtt_content, re.DOTALL)
    
    output_lines = []
    for match in matches:
        start_time = match[0]
        texts = re.sub(r'<.*?>', '', match[2])
        texts = texts.replace('\n', ' ')
        output_lines.append(f"{start_time} {texts}")
    
    with open(output_file_path, 'w', encoding='utf-8') as file:
        file.write("WEBVTT\n\n")
        for i, line in enumerate(output_lines, start=1):
            file.write(f"{i}\n{line}\n\n")

def process_vtt_files_in_folder(input_folder_path):
    output_folder_path = f"{input_folder_path}_edited"
    
    if not os.path.exists(output_folder_path):
        os.makedirs(output_folder_path)
    
    for filename in os.listdir(input_folder_path):
        if filename.endswith(".vtt"):
            input_file_path = os.path.join(input_folder_path, filename)
            output_file_path = os.path.join(output_folder_path, filename)
            process_vtt_file(input_file_path, output_file_path)

# 使用範例
input_folder_path = 'C:\others\subtitles\我的幸福婚約'  # 替換為你的資料夾路徑
process_vtt_files_in_folder(input_folder_path)





# file_path = '我的幸福婚約.S01E04.WEBRip.Netflix.en[cc].vtt'  # 替換為你的VTT檔案路徑