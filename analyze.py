import os
import re

search_dir = r'c:\Users\LENOVO\Desktop\KayuSushis'
exclude_dirs = {'.vs', 'bin', 'obj', 'packages', 'node_modules', '_export', '.git'}
exclude_exts = {'.zip', '.dll', '.exe', '.pdb', '.png', '.jpg', '.jpeg', '.gif', '.ico', '.cache'}

pattern = re.compile(r'74wfik', re.IGNORECASE)

files_to_rename = []
dirs_to_rename = []
files_with_content = {}

for root, dirs, files in os.walk(search_dir):
    dirs[:] = [d for d in dirs if d not in exclude_dirs]
    
    for d in dirs:
        if pattern.search(d):
            dirs_to_rename.append(os.path.join(root, d))
            
    for f in files:
        if any(f.endswith(ext) for ext in exclude_exts):
            continue
            
        file_path = os.path.join(root, f)
        if pattern.search(f):
            files_to_rename.append(file_path)
            
        try:
            with open(file_path, 'r', encoding='utf-8') as file:
                lines = file.readlines()
                matches = 0
                for line in lines:
                    if pattern.search(line):
                        matches += 1
                if matches > 0:
                    files_with_content[file_path] = matches
        except Exception:
            pass

print('Dirs to rename:', len(dirs_to_rename))
for d in dirs_to_rename:
    print('  -', d.replace(search_dir, ''))

print('\nFiles to rename:', len(files_to_rename))
for f in files_to_rename:
    print('  -', f.replace(search_dir, ''))

print('\nFiles with content references:', len(files_with_content))
for f, count in sorted(files_with_content.items(), key=lambda x: x[1], reverse=True)[:50]:
    print('  -', f.replace(search_dir, ''), f'({count} occurrences)')

