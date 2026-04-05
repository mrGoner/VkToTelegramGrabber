using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using TelegramBot.UserHelpers;

namespace TelegramBot;

public class BasicHelpersSelector(
    IReadOnlyDictionary<string, Type> helpersMap,
    IDefaultHelper defaultHelper,
    ILoggerFactory loggerFactory)
    : IUserHelperSelector
{
    public IDefaultHelper DefaultHelper { get; } = defaultHelper ?? throw new ArgumentNullException(nameof(defaultHelper));

    public bool TryGetCompatibleHelper(string command, out IUserHelper? helper)
    {
        helper = null;

        var type = helpersMap.TryGetValue(command, out var helperType) ? helperType : null;

        if (type == null)
            return false;

        helper = Activator.CreateInstance(type, BindingFlags.CreateInstance, null, loggerFactory.CreateLogger(type)) as IUserHelper;

        return helper is not null;
    }
}