using System;
using System.Threading;
using System.Threading.Tasks;
using VkGrabber;
using VkApi;

namespace TelegramBot.UserHelpers;

public class UserRegisterHelper : IUserHelper
{
    private bool _waitingForId;
    private bool _waitingForToken;
    private Vk? _vkApi;
    private UserManager? _userManager;
    private long _userId;

    public static string Command => "/register";

    public event WorkComplete? WorkCompleteEventHandler;

    public async ValueTask<Response?> ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        if (_userManager is null)
            throw new InvalidOperationException("User manager is null, seems helper not initialized");

        if (_vkApi is null)
            throw new InvalidOperationException("Vk api is null, seems helper not initialized");
        
        if (message == Command)
        {
            if (await _userManager.GetUserAsync(_userId.ToString(), cancellationToken) == null)
            {
                _waitingForId = true;
                return new Response("Для регистрации необходимо выполнить следующие шаги: \n" +
                                    "1) Перейти по ссылке https://vk.ru/apps?act=manage и создать новое standalone приложение \n" +
                                    "2) Прислать id приложения (находится в настройках созданного приложения) \n" +
                                    "3) В ответ я сгенерирую ссылку, перейдя по которой необходимо будет прислать мне access token \n \n" +
                                    "Кажется сложным? Исходники бота можно найти по ссылке https://github.com/mrGoner/VkToTelegramGrabber и развернуть своего собственного! \n" +
                                    "Если же готов продолжить - присылай id созданного приложения");
            }

            WorkCompleteEventHandler?.Invoke(_userId);

            return new Response(
                "Ты уже зарегистрирован, если хочешь ввести другой токен - необходимо удалить текущую учетную запись");
        }

        if (_waitingForId)
        {
            if (int.TryParse(message, out var id))
            {
                _waitingForId = false;
                _waitingForToken = true;

                var url = _vkApi.GetAuthUrl(id, VkApi.Requests.Permissions.Offline |
                                                 VkApi.Requests.Permissions.Wall |
                                                 VkApi.Requests.Permissions.Groups |
                                                 VkApi.Requests.Permissions.Friends |
                                                 VkApi.Requests.Permissions.Video);

                return new Response(
                    $"Перейди по данной ссылке {url}, после чего пришли мне access_token из строки браузера");
            }

            return new Response("Id не распознан, попробуй еще раз");
        }

        if (_waitingForToken)
        {
            if (!string.IsNullOrWhiteSpace(message) && message.Length > 60)
            {
                _waitingForToken = false;
                await _userManager.AddUserAsync(_userId.ToString(), message, cancellationToken);

                WorkCompleteEventHandler?.Invoke(_userId);

                return new Response("Успешно зарегистрировано! Вызови команду для добавления групп!");
            }

            return new Response("Токен не распознан как действительный, попробуй еще раз");
        }

        WorkCompleteEventHandler?.Invoke(_userId);

        return null;
    }

    public void Init(long userId, Vk vkApi, UserManager userManager)
    {
        _vkApi = vkApi ?? throw new ArgumentNullException(nameof(vkApi));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _userId = userId;
    }
}