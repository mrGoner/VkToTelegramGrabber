using TelegramBot;

var token = Environment.GetEnvironmentVariable("grabber_token");
var dbFolder = Environment.GetEnvironmentVariable("grabber_db_path");
var logsDir = Environment.GetEnvironmentVariable("grabber_logs_dir");

if(string.IsNullOrWhiteSpace(token))
    throw new ArgumentException(nameof(token));

if(string.IsNullOrWhiteSpace(dbFolder))
    throw new ArgumentException(nameof(dbFolder));

if(string.IsNullOrWhiteSpace(logsDir))
    throw new ArgumentException(nameof(logsDir));

var bot = new Bot(token, dbFolder, logsDir);

var cancellationTokenSource = new CancellationTokenSource();

await bot.Start(cancellationTokenSource.Token);

Console.CancelKeyPress += (_, _) => 
{
    cancellationTokenSource.Cancel();
    Environment.Exit(0);
};

Thread.Sleep(-1);