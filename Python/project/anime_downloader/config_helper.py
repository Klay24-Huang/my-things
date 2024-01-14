import yaml
import os

_config = None
_config_name = 'config_yoga_3'

# def set_config_name(config_name):
#     global _config_name
#     if not config_name.endswith(".yaml"):
#         config_name += ".yaml"
#     _config_name = config_name

def load_config():
    global _config

    if _config is None:
        with open(f'{_config_name}.yml', 'r') as yaml_file:
            _config = yaml.safe_load(yaml_file)

def get():
    load_config()
    return _config

def get_destination_path():
    load_config()
    return os.path.join(_config['path']['destination'], _config['anime']['name'])
