using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Helpers;

internal static class KeyBoardBuilder
{
    public static ReplyMarkup EmptyKeyboard => BuildMarkupKeyboard([]);

    public static ReplyMarkup BuildMarkupKeyboard(string[] stringArray)
    {
        var keyboardButtons = new KeyboardButton[stringArray.Length];
        
        for (var i = 0; i < stringArray.Length; i++) 
            keyboardButtons[i] = new KeyboardButton(stringArray[i]);

        var keyboard = new ReplyKeyboardMarkup(keyboardButtons) { ResizeKeyboard = true, OneTimeKeyboard = true };

        return keyboard;
    }

    public static ReplyMarkup BuildInlineKeyboard(KeyValuePair<string, string>[] stringArray)
    {
        var keyboardInline = new InlineKeyboardButton[1][];
        var keyboardButtons = new InlineKeyboardButton[stringArray.Length];
        
        for (var i = 0; i < stringArray.Length; i++)
        {
            keyboardButtons[i] = new InlineKeyboardButton(stringArray[i].Key)
            {
                CallbackData = stringArray[i].Value
            };
        }

        keyboardInline[0] = keyboardButtons;

        return new InlineKeyboardMarkup(keyboardInline);
    }
}