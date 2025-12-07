# -*- coding: utf-8 -*-
import re

file_path = 'UIChinesePatcher.cs'

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# 匹配字典初始化语法 { "key", "value" } 或 { "key", "value" },
pattern = r'(\s+)\{\s+"([^"]+)"\s+,\s+"([^"]+)"\s+\},?\s*'

def replace_func(match):
    indent = match.group(1)
    key = match.group(2)
    value = match.group(3)
    return f'{indent}addKey("{key}", "{value}");\n'

new_content = re.sub(pattern, replace_func, content)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(new_content)

print("Done!")
