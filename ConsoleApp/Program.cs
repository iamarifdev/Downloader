using Application.CommandHandlers;
using Application.QueryHandlers;
using Application.Services;
using Domain.Commands;
using Domain.Queries;
using Library;
using Spectre.Console;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

static (string url, string? filename) ProcessArgs(string[] args)
{
    if (args.Length < 1 || !args[0].StartsWith("--url="))
    {
        throw new Exception($"The download URL should be passed as the first argument. i.e --url=\"<url>\"");
    }

    var url = args[0].Split("--url=")[1];
    if (string.IsNullOrEmpty(url)) throw new Exception("The download URL should not be empty. i.e --url=\"<url>\"");

    string? filename = null;
    if (!url.Contains("youtube.com"))
    {
        if (!args[1].StartsWith("--filename="))
        {
            throw new Exception("The download URL should be passed as the 2nd argument. i.e --filename=\"<filename>\"");
        }

        filename = args[1].Split("=")[1];
    }

    return (url, filename);
}

var (url, filename) = ProcessArgs(args);

var downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

// handle youtube download
if (url.Contains("youtube.com"))
{
    var youtubeService = new YoutubeService(new YoutubeClient());
    var queryHandler = new YoutubeStreamsQueryHandler(youtubeService);
    var muxedStreamInfos = await queryHandler.HandleAsync(new YoutubeStreamsQuery(url));

    var streamInfos = muxedStreamInfos as MuxedStreamInfo[] ?? muxedStreamInfos.ToArray();
    var prompt = new SelectionPrompt<string>()
        .Title("Select an option:")
        .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
        .AddChoices(streamInfos.Select(s => $"{s.VideoQuality.Label} - {s.Container}"));

    var result = AnsiConsole.Prompt(prompt);
    var stream = streamInfos.FirstOrDefault(s => $"{s.VideoQuality.Label} - {s.Container}" == result);
    
    if (stream is null) throw new Exception("No stream selected");

    var destinationPath = Path.Combine(downloadFolder, "Youtube", $"{stream.VideoQuality.Label} - {stream.Container}");
    var commandHandler = new YoutubeDownloadCommandHandler(youtubeService);
    await commandHandler.HandleAsync(new YoutubeDownloadCommand(url, stream, destinationPath));
    return;
}

// handle file download
if (filename is not null)
{
    var progress = new Progress<double>();
    progress.ProgressChanged += (sender, value) => {
        Console.Write($"\rDownload progress: {value:P2}".PadRight(20));
    };
    
    Console.WriteLine($"URL to download: {url}");
    Console.WriteLine($"Save file as: {filename}");

    var dirPath = Path.Combine(downloadFolder, "Others");
    var fileDownloadPath = Path.Combine(dirPath, filename);
    if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
    
    var fileDownloadCommandHandler = new FileDownloadCommandHandler(new HttpClient());

    Console.WriteLine("File download started. Continue with other work...");
    await fileDownloadCommandHandler.HandleAsync(new FileDownloadCommand(url, fileDownloadPath, progress));
}