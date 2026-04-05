using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using TelegramBot;

var token = Environment.GetEnvironmentVariable("grabber_token");
var dbFolder = Environment.GetEnvironmentVariable("grabber_db_path");
var logsDir = Environment.GetEnvironmentVariable("grabber_logs_dir");
var proxyName = Environment.GetEnvironmentVariable("grabber_proxy");

if (string.IsNullOrWhiteSpace(token))
    throw new ArgumentException(nameof(token));

if (string.IsNullOrWhiteSpace(dbFolder))
    throw new ArgumentException(nameof(dbFolder));

if (string.IsNullOrWhiteSpace(logsDir))
    throw new ArgumentException(nameof(logsDir));

HttpClient? proxyHttpClient = null;

if (!string.IsNullOrWhiteSpace(proxyName))
{
    var handler = new HttpClientHandler
    {
        Proxy = new WebProxy(new Uri(proxyName)),
        UseProxy = true
    };

    proxyHttpClient = new HttpClient(handler);
}

var bot = new Bot(token, dbFolder, logsDir, proxy: proxyHttpClient);

var cancellationTokenSource = new CancellationTokenSource();

await bot.Start(cancellationTokenSource.Token);

Console.CancelKeyPress += (_, _) =>
{
    cancellationTokenSource.Cancel();
    Environment.Exit(0);
};

Thread.Sleep(-1);