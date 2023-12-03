import download_helper
import file_helper
import audio_helper


def start():
    # 下載
    # download_helper.start()
    # 從下載資料夾 轉移到儲存資料夾
    file_helper.move_to_destination()
    # 從影片檔抽取音源
    # 音源檔除去op ed片段
    # audio_helper.start()
    print("all work finished.")
