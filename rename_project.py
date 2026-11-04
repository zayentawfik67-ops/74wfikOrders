import os
import glob
import shutil

ROOT_DIR = r"c:\Users\LENOVO\Desktop\KayuSushis"

IGNORE_DIRS = {".git", "bin", "obj", "node_modules", ".vs", "packages"}
IGNORE_EXTS = {".dll", ".exe", ".png", ".jpg", ".jpeg", ".pdb", ".cache", ".sqlite", ".db", ".vsidx", ".suo"}

def should_process_dir(dir_name):
    return dir_name.lower() not in IGNORE_DIRS

def should_process_file(file_name):
    _, ext = os.path.splitext(file_name)
    return ext.lower() not in IGNORE_EXTS

def replace_in_file(filepath):
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
    except (UnicodeDecodeError, PermissionError, OSError):
        return # Skip binary or locked files
        
    new_content = content.replace("KayuSushi", "KayuSushi")
    new_content = new_content.replace("KayuSushi", "KayuSushi")
    
    if new_content != content:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        print(f"Updated content in {filepath}")

def rename_files_and_dirs(root_path):
    dir_renames = []
    file_renames = []
    
    for root, dirs, files in os.walk(root_path, topdown=True):
        # Filter dirs in place to skip ignored directories
        dirs[:] = [d for d in dirs if should_process_dir(d)]
        
        for file in files:
            if should_process_file(file):
                filepath = os.path.join(root, file)
                replace_in_file(filepath)
                
            if "KayuSushi" in file:
                old_path = os.path.join(root, file)
                new_file = file.replace("KayuSushi", "KayuSushi")
                new_path = os.path.join(root, new_file)
                file_renames.append((old_path, new_path))
                
        for d in dirs:
            if "KayuSushi" in d:
                old_path = os.path.join(root, d)
                new_dir = d.replace("KayuSushi", "KayuSushi")
                new_path = os.path.join(root, new_dir)
                dir_renames.append((old_path, new_path))

    # Perform file renames first
    for old_path, new_path in file_renames:
        try:
            os.rename(old_path, new_path)
            print(f"Renamed file: {old_path} -> {new_path}")
        except Exception as e:
            print(f"Error renaming file {old_path}: {e}")

    # Perform dir renames in reverse order so deep dirs are renamed first
    dir_renames.reverse()
    for old_path, new_path in dir_renames:
        try:
            os.rename(old_path, new_path)
            print(f"Renamed dir: {old_path} -> {new_path}")
        except Exception as e:
            print(f"Error renaming dir {old_path}: {e}")

if __name__ == "__main__":
    print("Starting renaming process...")
    rename_files_and_dirs(ROOT_DIR)
    print("Done!")
