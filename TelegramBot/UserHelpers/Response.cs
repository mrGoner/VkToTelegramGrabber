using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.UserHelpers;

public class Response
{
    public ReplyMarkup? ReplyMarkup { get; }
    public string ReplyMessage { get; }

    public Response(string replyMessage)
    {
        ReplyMessage = replyMessage;
    }

    public Response(string replyMessage, ReplyMarkup replyMarkup)
    {
        ReplyMessage = replyMessage;
        ReplyMarkup = replyMarkup;
    }
}