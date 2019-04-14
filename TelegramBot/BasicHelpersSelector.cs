using System;
using System.Collections.Generic;
using System.Linq;

namespace TelegramBot
{
    public class BasicHelpersSelector : IUserHelperSelector
    {
        private readonly List<IUserHelper> m_helpers;
        private readonly IUserHelper m_defaultHelper;

        public BasicHelpersSelector(List<IUserHelper> _helpers)
        {
            m_helpers = _helpers ?? throw new ArgumentNullException(nameof(_helpers));
            m_defaultHelper = new UserInfoHelper();
        }

        public IUserHelper DefaultHelper => m_defaultHelper;

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
