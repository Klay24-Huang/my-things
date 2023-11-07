import os
import shutil
import time

def filter_and_copy_files(folder_path1, file_type, file_size, file_created_at, folder_path2):
    # 检查文件夹路径是否存在
    if not os.path.exists(folder_path1) or not os.path.exists(folder_path2):
        print("文件夹路径不存在")
        return

    # 获取文件夹中的所有文件
    for root, dirs, files in os.walk(folder_path1):
        for file in files:
            file_path = os.path.join(root, file)

            # 检查文件类型是否匹配
            if file.endswith(file_type):
                # 获取文件大小
                file_info = os.stat(file_path)
                file_size_bytes = file_info.st_size

                # 获取文件创建时间
                file_created_time = file_info.st_ctime

                # 检查文件大小和创建时间是否满足条件
                if file_size_bytes > file_size and file_created_time > file_created_at:
                    # 构建目标文件路径
                    destination_path = os.path.join(folder_path2, file)

                    # 复制文件
                    shutil.copy2(file_path, destination_path)
                    print(f"已复制文件: {file_path} 到 {destination_path}")

# 使用示例
folder_path1 = "path_to_source_folder"
file_type = ".txt"
file_size = 1024  # 文件大小大于 1024 字节
file_created_at = time.mktime(time.strptime("2023-01-01 00:00:00", "%Y-%m-%d %H:%M:%S"))
folder_path2 = "path_to_destination_folder"

filter_and_copy_files(folder_path1, file_type, file_size, file_created_at, folder_path2)
