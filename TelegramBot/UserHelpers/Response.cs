using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.UserHelpers;

public class Response
{
    public ReplyMarkup ReplyMarkup { get; }
    public string ReplyMessage { get; }

    public Response(string _replyMessage)
    {
        ReplyMessage = _replyMessage;
    }

    public Response(string _replyMessage, ReplyMarkup _replyMarkup)
    {
        ReplyMessage = _replyMessage;
        ReplyMarkup = _replyMarkup;
    }
}