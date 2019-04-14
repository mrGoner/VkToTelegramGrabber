using System;
using VkGrabber;
using Telegram.Bot.Types.ReplyMarkups;
using VkApi;
using System.Text;
using VkTools.ObjectModel;
using Group = VkTools.ObjectModel.Group;
using System.Linq;

namespace TelegramBot
{
    public partial class Bot
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
            private IReplyMarkup m_generalMarkup;

            public void Init(long _userId, Vk _vkApi, UserManager _userManager)
            {
                m_vkApi = _vkApi ?? throw new ArgumentNullException(nameof(_vkApi));
                m_userManager = _userManager ?? throw new ArgumentNullException(nameof(_userManager));
                m_userId = _userId;
                m_generalMarkup = GetInlineKeyboard(new string[] { "Добавить группу", "Удалить группу" });
            }

            public Response OnCallBackUpdate()
            {
                throw new NotImplementedException();
            }

            public Response OnMessage(string _message)
            {
                var user = m_userManager.GetUser(m_userId.ToString());

                if (user == null)
                    return new Response("Ты еще не зарегистрирован!");

                if (_message == Command)
                    return new Response("Выбирай команду:", m_generalMarkup);

                if(_message == "Добавить группу")
                {
                    var builder = new StringBuilder();

                    builder.AppendLine("Пришли мне ID группы для добавления из списка ниже");

                    m_rawGroups = m_vkApi.GetGroups(user.Token, 100);

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

                if(_message == "Удалить группу")
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
                    m_waitingGroupRemove = false;

                    if(int.TryParse(_message, out var removeIndex) && user.Groups.Length >= removeIndex)
                    {
                        m_userManager.RemoveGroupFromUser(user.Key, user.Groups[removeIndex]);

                        return new Response("Удалено!");
                    }

                    return new Response("Некорректный индекс!", m_generalMarkup);
                }

                if (m_waitingGroupNum)
                {
                    m_waitingGroupNum = false;

                    if (int.TryParse(_message, out var groupNum) && m_rawGroups.Count >= groupNum)
                    {
                        m_selectedGroup = m_rawGroups[groupNum];

                        m_waitingGroupPeriod = true;

                        return new Response("Почти все! Выбери период обновления",
                            GetInlineKeyboard(new string[] { "00:15:00", "00:30:00", "01:00:00", "01:30:00", "02:00:00" }));
                    }

                    return new Response("Некорректный ID группы", m_generalMarkup);
                }

                if (m_waitingGroupPeriod)
                {
                    m_waitingGroupPeriod = false;

                    if (TimeSpan.TryParse(_message, out var span) && 
                        span >= TimeSpan.FromMinutes(15))
                    {
                        m_userManager.AddGroupToUser(user.Key,
                            new VkGrabber.Group(m_selectedGroup.Id, span, m_selectedGroup.Name));

                        return new Response("Добавлено!");
                    }

                    return new Response("Некорректный период обновления!", m_generalMarkup);
                }

                return null;
            }

            internal IReplyMarkup GetInlineKeyboard(string[] stringArray)
            {
                var keyboardButtons = new KeyboardButton[stringArray.Length];
                for (var i = 0; i < stringArray.Length; i++)
                {
                    keyboardButtons[i] = new KeyboardButton
                    {
                        Text = stringArray[i]
                    };
                }

                var keyboardInline = new ReplyKeyboardMarkup(keyboardButtons, true);

                return keyboardInline;
            }
        }
    }
}
