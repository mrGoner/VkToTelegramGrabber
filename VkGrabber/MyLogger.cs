using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace VkGrabber
{
    public static class MyLogger
    {
        static MyLogger()
        {
            var logRepo = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepo, 
                new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }

        public static ILog Log { get; } = LogManager.GetLogger(typeof(MyLogger));
    }
}
