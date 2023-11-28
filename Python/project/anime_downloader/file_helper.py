import os
import shutil
import config_helper
import mimetypes
from datetime import datetime, timedelta

def get_file_and_root(folder_path):
    root, _, files = next(os.walk(folder_path))
    return  root, files

def move_to_destination():
    config = config_helper.get()
    filter_config = config['filter']
    # create folder 
    destination_path = config_helper.get_destination_path()
    if not os.path.exists(destination_path):
        os.makedirs(destination_path)

    file_type_set = set(filter_config['file_type'])

    # get all file in download folder 
    root, files = get_file_and_root(config['path']['download'])
    for file in files:
        file_path = os.path.join(root, file)
        file_info = os.stat(file_path)
        # print(os.path.basename(file_path))
        
        # check create time
        file_create_time = datetime.fromtimestamp(file_info.st_ctime)
        # 如果沒設定時間則忽略
        min_created_before = filter_config['min_created_before']
        if  min_created_before > 0 and file_create_time < (datetime.now() - timedelta(minutes= filter_config['min_created_before'])):
            # print('skip by created time')
            continue
        # check file size
        if (file_info.st_size /(1024 * 1024)) < filter_config['size']:
            # print('skip by file size')
            continue
        # check datatype
        file_type, _ = mimetypes.guess_type(file_path)
        main_type, sub_type = file_type.split('/')
        if main_type != 'video' and sub_type not in file_type_set:
            # print('skip by file type')
            continue
        
        # copy file
        shutil.copy2(file_path, destination_path)
        print(f"copy file: {os.path.basename(file_path)}")
            



# def filter_and_copy_files(folder_path1, file_type, file_size, file_created_at, folder_path2):
#     # 检查文件夹路径是否存在
#     if not os.path.exists(folder_path1) or not os.path.exists(folder_path2):
#         print("文件夹路径不存在")
#         return

    

#     # 获取文件夹中的所有文件
#     for root, dirs, files in os.walk(folder_path1):
#         for file in files:
#             file_path = os.path.join(root, file)

#             # 检查文件类型是否匹配
#             if file.endswith(file_type):
#                 # 获取文件大小
#                 file_info = os.stat(file_path)
#                 file_size_bytes = file_info.st_size

#                 # 获取文件创建时间
#                 file_created_time = file_info.st_ctime

#                 # 检查文件大小和创建时间是否满足条件
#                 if file_size_bytes > file_size and file_created_time > file_created_at:
#                     # 构建目标文件路径
#                     destination_path = os.path.join(folder_path2, file)

#                     # 复制文件
#                     shutil.copy2(file_path, destination_path)
#                     print(f"已复制文件: {file_path} 到 {destination_path}")

# # 使用示例
# folder_path1 = "path_to_source_folder"
# file_type = ".txt"
# file_size = 1024  # 文件大小大于 1024 字节
# file_created_at = time.mktime(time.strptime("2023-01-01 00:00:00", "%Y-%m-%d %H:%M:%S"))
# folder_path2 = "path_to_destination_folder"

# filter_and_copy_files(folder_path1, file_type, file_size, file_created_at, folder_path2)
