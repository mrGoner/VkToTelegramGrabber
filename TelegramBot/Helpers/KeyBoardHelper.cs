using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Helpers
{
    internal static class KeyBoardBuilder
    {
        public static IReplyMarkup BuildMarkupKeyboard(string[] stringArray)
        {
            var keyboardButtons = new KeyboardButton[stringArray.Length];
            for (var i = 0; i < stringArray.Length; i++)
            {
                keyboardButtons[i] = new KeyboardButton
                {
                    Text = stringArray[i]
                };
            }

            var keyboard = new ReplyKeyboardMarkup(keyboardButtons, true);

            return keyboard;
        }

        public static IReplyMarkup BuildInlineKeyboard(KeyValuePair<string, string>[] stringArray)
        {
            var keyboardInline = new InlineKeyboardButton[1][];
            var keyboardButtons = new InlineKeyboardButton[stringArray.Length];
            for (var i = 0; i < stringArray.Length; i++)
            {
                keyboardButtons[i] = new InlineKeyboardButton
                {
                    Text = stringArray[i].Key,
                    CallbackData = stringArray[i].Value
                };
            }
            keyboardInline[0] = keyboardButtons;

            return new InlineKeyboardMarkup(keyboardInline);
        }
    }
}