using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
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

        public static InlineKeyboardButton[][] BuildInlineKeyboard(string[] stringArray)
        {
            var keyboardInline = new InlineKeyboardButton[1][];
            var keyboardButtons = new InlineKeyboardButton[stringArray.Length];
            for (var i = 0; i < stringArray.Length; i++)
            {
                keyboardButtons[i] = new InlineKeyboardButton
                {
                    Text = stringArray[i],
                    CallbackData = "some data"
                };
            }
            keyboardInline[0] = keyboardButtons;
            return keyboardInline;
        }
    }
}