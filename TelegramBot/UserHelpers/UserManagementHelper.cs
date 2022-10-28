using System;
using VkGrabber;
using Telegram.Bot.Types.ReplyMarkups;
using VkApi;
using System.Text;
using Group = VkApi.ObjectModel.Group;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelegramBot.Helpers;
using VkApi.ObjectModel;

namespace TelegramBot.UserHelpers
{
    public class UserManagementHelper : IUserHelper
    {
        public string Command => "/manage";

        public event WorkComplete WorkCompleteEventHandler;

        private Vk m_vkApi;
        private UserManager m_userManager;
        private long m_userId;
        private bool m_waitingGroupNum;
        private Groups m_rawGroups;
        private Group m_selectedGroup;
        private bool m_waitingGroupPeriod;
        private bool m_waitingGroupRemove;
        private static IReplyMarkup m_generalMarkup;
        private static IReplyMarkup m_intervalMarkup;

        static UserManagementHelper()
        {
            m_intervalMarkup = KeyBoardBuilder.BuildMarkupKeyboard(new[] { "00:15:00", "00:30:00", "01:00:00", "01:30:00", "02:00:00" });
            m_generalMarkup = KeyBoardBuilder.BuildMarkupKeyboard(new[] { "Добавить группу", "Удалить группу" });
        }

        public void Init(long _userId, Vk _vkApi, UserManager _userManager)
        {
            m_vkApi = _vkApi ?? throw new ArgumentNullException(nameof(_vkApi));
            m_userManager = _userManager ?? throw new ArgumentNullException(nameof(_userManager));
            m_userId = _userId;
        }

        public async ValueTask<Response> ProcessMessageAsync(string _message, CancellationToken _cancellationToken)
        {
            try
            {
                var user = await m_userManager.GetUserAsync(m_userId.ToString(), _cancellationToken);

                if (user == null)
                {
                    return new Response("Ты еще не зарегистрирован!");
                }

                if (_message == Command)
                    return new Response("Выбирай команду:", m_generalMarkup);

                if (_message == "Добавить группу")
                {
                    var builder = new StringBuilder();

                    builder.AppendLine("Пришли мне ID группы для добавления из списка ниже");

                    m_rawGroups = await m_vkApi.GetGroupsAsync(user.Token, 100, _cancellationToken);

                    foreach (var userGroup in user.Groups)
                    {
                        var existed = m_rawGroups.FirstOrDefault(_rawGroup => _rawGroup.Id == userGroup.GroupId);

                        if (existed != null)
                            m_rawGroups.Remove(existed);
                    }

                    for (int i = 0; i < m_rawGroups.Count; i++)
                    {
                        var group = m_rawGroups[i];
                        builder.AppendLine($"ID: {i}. Имя: {group.Name}");
                    }

                    m_waitingGroupNum = true;
                    m_waitingGroupPeriod = false;
                    m_waitingGroupRemove = false;
                    
                    return new Response(builder.ToString());
                }

                if (_message == "Удалить группу")
                {
                    var builder = new StringBuilder();

                    builder.AppendLine("Пришли мне ID группы для удаления из списка ниже");

                    if (user.Groups.Any())
                    {
                        for (int i = 0; i < user.Groups.Length; i++)
                        {
                            var group = user.Groups[i];
                            builder.AppendLine($"ID: {i}. Имя: {group.Name}");
                        }

                        m_waitingGroupNum = false;
                        m_waitingGroupPeriod = false;
                        m_waitingGroupRemove = true;

                        return new Response(builder.ToString());
                    }

                    return new Response("Список групп пуст!", m_generalMarkup);
                }

                if (m_waitingGroupRemove)
                {
                    if (int.TryParse(_message, out var removeIndex) && removeIndex >= 0 && user.Groups.Length - 1 >= removeIndex)
                    {
                        m_waitingGroupRemove = false;
                        _ = m_userManager.RemoveGroupFromUser(user.Key, user.Groups[removeIndex], _cancellationToken);

                        WorkCompleteEventHandler?.Invoke(m_userId);
                        
                        return new Response("Удалено!", m_generalMarkup);
                    }

                    return new Response("Некорректный ID", m_generalMarkup);
                }

                if (m_waitingGroupNum)
                {
                    if (int.TryParse(_message, out var groupNum) && groupNum >= 0 && m_rawGroups.Count - 1 >= groupNum)
                    {
                        m_selectedGroup = m_rawGroups[groupNum];

                        m_waitingGroupNum = false;
                        m_waitingGroupPeriod = true;

                        return new Response("Почти все! Выбери период обновления", m_intervalMarkup);
                    }

                    return new Response("Некорректный ID", m_generalMarkup);
                }

                if (m_waitingGroupPeriod)
                {

                    if (TimeSpan.TryParse(_message, out var span) &&
                        span >= TimeSpan.FromMinutes(15))
                    {
                        m_waitingGroupPeriod = false;

                        await m_userManager.AddGroupToUserAsync(user.Key,
                            new VkGrabber.Group(m_selectedGroup.Id, span, m_selectedGroup.Name), _cancellationToken);

                        WorkCompleteEventHandler?.Invoke(m_userId);

                        return new Response("Добавлено!", KeyBoardBuilder.EmptyKeyboard);
                    }

                    return new Response("Некорректный период обновления!", m_intervalMarkup);
                }

                return null;
            }
            catch
            {
                WorkCompleteEventHandler?.Invoke(m_userId);

                return new Response("Что-то пошло не так");
            }
        }
    }
}