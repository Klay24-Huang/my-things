import yaml

_config = None

def get():
    if _config == None:
        with open('config.yml', 'r') as yaml_file:
            _config = yaml.safe_load(yaml_file)
    
    return _config