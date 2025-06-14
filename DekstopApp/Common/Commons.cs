namespace DekstopApp.Common;

public class Commons
{
    
}

// import os
// from datetime import datetime
// import hashlib
// import re
// import pytz
// import shutil
//
// from .locks import *
//
//
// def show_bot_info(username: str):
//     current_date = get_date_for_hello()
//     current_time = get_time_for_hello()
//     window_width = get_terminal_width()
//     bot_version = "3.0 RC1"
//     greeting = """
//             _/      _/    _/_/_/    _/_/    _/      _/   * Вымученная дочка от Игоря
//            _/_/    _/  _/        _/    _/  _/_/  _/_/    * Плотный гагиш от ПОпкея
//           _/  _/  _/  _/        _/    _/  _/  _/  _/     * Таран от Данылы
//          _/    _/_/  _/        _/    _/  _/      _/      ...
//         _/      _/    _/_/_/    _/_/    _/      _/       * И, наверное, сила земли
//     """
//
//     line = "" + "-" * (115)
//     print(line)
//     print("" + " " * ((window_width - len(greeting) - 4) // 2) + greeting + " " * (
//             (window_width - len(greeting) - 4) // 2 + (
//             window_width - len(greeting) - 4) % 2) + "")
//     print(line)
//     print(" Дата: {}{} ".format(current_date, " " * (window_width - 12 - len(str(current_date)))))
//     print(" Время: {}{} ".format(current_time, " " * (window_width - 12 - len(str(current_time)))))
//     print(" Версия бота: {}{} ".format(bot_version, " " * (window_width - 18 - len(bot_version))))
//     print(" Ссылка на бота: https://t.me/{}{} ".format(username, " " * (window_width - 18 - len(str(username)))))
//     print(line)
//
//
// def get_time(format='%Y-%m-%d %H-%M-%S'):
//     return datetime\
//         .now(pytz.timezone('Europe/Istanbul'))\
//         .strftime(format)
//
//
// def get_hash(payload: str):
//     return hashlib.sha256(payload.encode("utf-8")).hexdigest()
//
//
// def de_emojify(text):
//     return regrex_pattern.sub(r'', text)
//
//
// def listdir_nested(dir_name):
//     list_of_file = os.listdir(dir_name)
//     all_files = list()
//     for entry in list_of_file:
//         fullPath = os.path.join(dir_name, entry)
//         if os.path.isdir(fullPath):
//             all_files = all_files + listdir_nested(fullPath)
//         else:
//             all_files.append(fullPath)
//     return all_files
//
//
// regrex_pattern = re.compile(
//     pattern="["
//     u"\U0001F600-\U0001F64F"  # emoticons
//     u"\U0001F300-\U0001F5FF"  # symbols & pictographs
//     u"\U0001F680-\U0001F6FF"  # transport & map symbols
//     u"\U0001F1E0-\U0001F1FF"  # flags (iOS)
//     "]+",
//     flags=re.UNICODE
// )
//
// sps = '  '
//
//
// def smart_trim(num, max=26, sym: str = sps, cut_emoji: bool = False, ltr=True):
//     if num is not None:
//         t = str(num)[0:max].replace('-', '–')
//         t = de_emojify(t) if cut_emoji else t
//     else:
//         t = ''
//     l = max - len(t)
//     if l > 0:
//         if not cut_emoji:
//             t = t.replace(' ', '  ')
//         if ltr:
//             t = sym * l + t
//         else:
//             t = t + sym * l
//     return t
//
//
// def trim_id(identifier):
//     s = str(identifier)
//     return f'{s[0:2]}*{s[-4:]}'
//
//
// def print_context(requested, text: str):
//     print(
//         f"{get_time()} " +
//         f"- {smart_trim(requested.id, 11, ' ', cut_emoji=True)}" +
//         f"| {smart_trim(requested.username, 14, ' ', cut_emoji=True)}" +
//         f"| {smart_trim(requested.union_id, 15, ' ', cut_emoji=True)}" +
//         f"| {smart_trim(requested.description, 15, ' ', cut_emoji=True)}| {text}"
//     )
//
//
// def get_date_for_hello(format='%Y-%m-%d'):
//     return datetime \
//         .now(pytz.timezone('Europe/Istanbul')) \
//         .strftime(format)
//
//
// def get_terminal_width():
//     terminal_width = shutil.get_terminal_size((80, 20)).columns
//     return terminal_width
//
//
// def get_time_for_hello(format='%H:%M:%S'):
//     return datetime \
//         .now(pytz.timezone('Europe/Istanbul')) \
//         .strftime(format)
//
//
// async def write_to_log_for_entity(entity: str, content: str):
//     filename = f"./logs/{entity}.log"
//     lck = await lock_for_name(f"file:{filename}")
//     async with lck:
//         with open(filename, "a") as log:
//             log.write(content)
