import yaml

_config = None

def load_config():
    global _config

    if _config is None:
        with open('config.yml', 'r') as yaml_file:
            _config = yaml.safe_load(yaml_file)

def get():
    load_config()
    return _config

def get_destination_path():
    load_config()
    return rf'{_config['path']['destination']}\{_config['anime']['name']}'
