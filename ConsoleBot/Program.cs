using TelegramBot;

var token = Environment.GetEnvironmentVariable("grabber_token");
var dbFolder = Environment.GetEnvironmentVariable("grabber_db_path");

if(string.IsNullOrWhiteSpace(token))
    throw new ArgumentException(nameof(token));

if(string.IsNullOrWhiteSpace(dbFolder))
    throw new ArgumentException(nameof(dbFolder));

var bot = new Bot(token, dbFolder);

var cancellationTokenSource = new CancellationTokenSource();

await bot.Start(cancellationTokenSource.Token);

Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) => 
{
    cancellationTokenSource.Cancel();
    Environment.Exit(0);
};

Thread.Sleep(-1);