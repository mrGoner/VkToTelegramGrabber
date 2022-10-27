using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using System;

namespace TelegramBot.Helpers
{
    internal static class KeyBoardBuilder
    {
        public static IReplyMarkup EmptyKeyboard => BuildMarkupKeyboard(Array.Empty<string>());
        public static IReplyMarkup BuildMarkupKeyboard(string[] _stringArray)
        {
            var keyboardButtons = new KeyboardButton[_stringArray.Length];
            for (var i = 0; i < _stringArray.Length; i++)
            {
                keyboardButtons[i] = new KeyboardButton(_stringArray[i]);
            }

            var keyboard = new ReplyKeyboardMarkup(keyboardButtons) { ResizeKeyboard = true, OneTimeKeyboard = true };

            return keyboard;
        }

        public static IReplyMarkup BuildInlineKeyboard(KeyValuePair<string, string>[] _stringArray)
        {
            var keyboardInline = new InlineKeyboardButton[1][];
            var keyboardButtons = new InlineKeyboardButton[_stringArray.Length];
            for (var i = 0; i < _stringArray.Length; i++)
            {
                keyboardButtons[i] = new InlineKeyboardButton(_stringArray[i].Key)
                {
                    CallbackData = _stringArray[i].Value
                };
            }
            keyboardInline[0] = keyboardButtons;

            return new InlineKeyboardMarkup(keyboardInline);
        }
    }
}