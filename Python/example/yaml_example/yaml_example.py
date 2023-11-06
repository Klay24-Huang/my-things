import yaml

# 读取 YAML 配置文件
with open('config.yml', 'r') as yaml_file:
    config = yaml.safe_load(yaml_file)

# 现在，config 是一个包含配置数据的字典
print(config)
