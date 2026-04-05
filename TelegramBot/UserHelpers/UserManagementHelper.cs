using System;
using VkGrabber;
using Telegram.Bot.Types.ReplyMarkups;
using VkApi;
using System.Text;
using Group = VkApi.ObjectModel.Group;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TelegramBot.Helpers;
using VkApi.ObjectModel;

namespace TelegramBot.UserHelpers;

public class UserManagementHelper(ILogger<UserManagementHelper> logger) : IUserHelper
{
    public static string Command => "/manage";

    public event WorkComplete? WorkCompleteEventHandler;

    private Vk? _vkApi;
    private UserManager? _userManager;
    private long _userId;
    private bool _waitingGroupNum;
    private Groups? _rawGroups;
    private Group? _selectedGroup;
    private bool _waitingGroupPeriod;
    private bool _waitingGroupRemove;
    private ILogger? _logger;
    private static readonly ReplyMarkup s_generalMarkup;
    private static readonly ReplyMarkup s_intervalMarkup;

    static UserManagementHelper()
    {
        s_intervalMarkup = KeyBoardBuilder.BuildMarkupKeyboard(["00:15:00", "00:30:00", "01:00:00", "01:30:00", "02:00:00"]);
        s_generalMarkup = KeyBoardBuilder.BuildMarkupKeyboard(["Добавить группу", "Удалить группу"]);
    }

    public void Init(long userId, Vk vkApi, UserManager userManager)
    {
        _vkApi = vkApi ?? throw new ArgumentNullException(nameof(vkApi));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _userId = userId;
        _logger = logger;
    }

    public async ValueTask<Response?> ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        try
        {
            if (_userManager is null)
                throw new InvalidOperationException("User manager is null, seems helper not initialized");

            if (_vkApi is null)
                throw new InvalidOperationException("Vk api is null, seems helper not initialized");
            
            var user = await _userManager.GetUserAsync(_userId.ToString(), cancellationToken);

            if (user == null) 
                return new Response("Ты еще не зарегистрирован!");

            if (message == Command)
                return new Response("Выбирай команду:", s_generalMarkup);

            if (message == "Добавить группу")
            {
                var builder = new StringBuilder();

                builder.AppendLine("Пришли мне ID группы для добавления из списка ниже");

                _rawGroups = await _vkApi.GetGroupsAsync(user.Token, 100, cancellationToken);

                foreach (var userGroup in user.Groups)
                {
                    var existed = _rawGroups.FirstOrDefault(rawGroup => rawGroup.Id == userGroup.GroupId);

                    if (existed != null)
                        _rawGroups.Remove(existed);
                }

                for (var i = 0; i < _rawGroups.Count; i++)
                {
                    var group = _rawGroups[i];
                    builder.AppendLine($"ID: {i}. Имя: {group.Name}");
                }

                _waitingGroupNum = true;
                _waitingGroupPeriod = false;
                _waitingGroupRemove = false;

                return new Response(builder.ToString());
            }

            if (message == "Удалить группу")
            {
                var builder = new StringBuilder();

                builder.AppendLine("Пришли мне ID группы для удаления из списка ниже");

                if (user.Groups.Any())
                {
                    for (var i = 0; i < user.Groups.Length; i++)
                    {
                        var group = user.Groups[i];
                        builder.AppendLine($"ID: {i}. Имя: {group.Name}");
                    }

                    _waitingGroupNum = false;
                    _waitingGroupPeriod = false;
                    _waitingGroupRemove = true;

                    return new Response(builder.ToString());
                }

                return new Response("Список групп пуст!", s_generalMarkup);
            }

            if (_waitingGroupRemove)
            {
                if (int.TryParse(message, out var removeIndex) && removeIndex >= 0 &&
                    user.Groups.Length - 1 >= removeIndex)
                {
                    _waitingGroupRemove = false;
                    _ = _userManager.RemoveGroupFromUser(user.Key, user.Groups[removeIndex], cancellationToken);

                    WorkCompleteEventHandler?.Invoke(_userId);

                    return new Response("Удалено!", s_generalMarkup);
                }

                return new Response("Некорректный ID", s_generalMarkup);
            }

            if (_waitingGroupNum)
            {
                if (_rawGroups == null)
                    return null;
                
                if (int.TryParse(message, out var groupNum) && groupNum >= 0 && _rawGroups.Count - 1 >= groupNum)
                {
                    _selectedGroup = _rawGroups[groupNum];

                    _waitingGroupNum = false;
                    _waitingGroupPeriod = true;

                    return new Response("Почти все! Выбери период обновления", s_intervalMarkup);
                }

                return new Response("Некорректный ID", s_generalMarkup);
            }

            if (_waitingGroupPeriod)
            {
                if (_selectedGroup == null)
                    return null;
                
                if (TimeSpan.TryParse(message, out var span) && span >= TimeSpan.FromMinutes(15))
                {
                    _waitingGroupPeriod = false;

                    await _userManager.AddGroupToUserAsync(user.Key,
                        new VkGrabber.Group(_selectedGroup.Id, span, _selectedGroup.Name), cancellationToken);

                    WorkCompleteEventHandler?.Invoke(_userId);

                    return new Response("Добавлено!", KeyBoardBuilder.EmptyKeyboard);
                }

                return new Response("Некорректный период обновления!", s_intervalMarkup);
            }

            return null;
        }
        catch(Exception ex)
        {
            WorkCompleteEventHandler?.Invoke(_userId);

            _logger?.LogError(ex, "Exception occured");

            return new Response("Что-то пошло не так");
        }
    }
}