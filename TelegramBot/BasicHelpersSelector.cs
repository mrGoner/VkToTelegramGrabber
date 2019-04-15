using System;
using System.Collections.Generic;
using System.Linq;
using TelegramBot.UserHelpers;

namespace TelegramBot
{
    public class BasicHelpersSelector : IUserHelperSelector
    {
        private readonly List<IUserHelper> m_helpers;

        public BasicHelpersSelector(List<IUserHelper> _helpers, IDefaultHelper _defaultHelper)
        {
            m_helpers = _helpers ?? throw new ArgumentNullException(nameof(_helpers));
            DefaultHelper = _defaultHelper ?? throw new ArgumentNullException(nameof(_defaultHelper));
        }

        public IDefaultHelper DefaultHelper { get; }

        public bool TryGetCompatibleHelper(string _command, out IUserHelper _helper)
        {
            _helper = null;

            var type = m_helpers.FirstOrDefault(_h => _h.Command == _command)?.GetType();

            if (type == null)
                return false;

            _helper = (IUserHelper)Activator.CreateInstance(type);

            return true;
        }
    }
}
