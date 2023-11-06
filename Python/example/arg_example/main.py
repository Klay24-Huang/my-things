import argparse

# 创建参数解析器
parser = argparse.ArgumentParser(description="这是一个演示参数传递的脚本")

# 添加参数
parser.add_argument("--arg1", help="第一个参数的说明")
parser.add_argument("--arg2", help="第二个参数的说明")
parser.add_argument("--arg3", help="第三个参数的说明")

# 解析命令行参数
args = parser.parse_args()

# 访问参数值
arg1 = args.arg1
arg2 = args.arg2
arg3 = args.arg3

# 打印参数值
print(f"参数1: {arg1}")
print(f"参数2: {arg2}")
print(f"参数3: {arg3}")

# py main.py --arg1 value1 --arg2 value2 --arg3 value3
