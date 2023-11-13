from moviepy.editor import AudioFileClip
import config_helper
import file_helper

def start():
    
    # get all video file
    path = config_helper.get_destination_path()
    root, files = file_helper.get_file_and_root(path)

    # sort by create time
    sorted_files = sorted(files, key=lambda x: os.path.getctime(os.path.join(path, x)))
    
    # get section 
    all_sections = config_helper.get()['anime']['episode']['sections']

    for index, file in enumerate(sorted_files):
        video_path = os.path.join(root, file)

        # extract audio using AudioFileClip
        audio = AudioFileClip(video_path)

        # combine need section
        sections = all_sections[index]
        new_audio = None
        for section in sections:
            start_time_ms = time_to_ms(section[0])
            end_time_ms = time_to_ms(section[1])
            if new_audio is None:
                new_audio = audio.subclip(start_time_ms / 1000, end_time_ms / 1000)
            else:
                new_audio = new_audio + audio.subclip(start_time_ms / 1000, end_time_ms / 1000)

        # save audio file using os.path.join
        new_audio.write_audiofile(os.path.join(path, f'{index}.mp3'), codec='mp3')

        # no need to explicitly close audio and new_audio
