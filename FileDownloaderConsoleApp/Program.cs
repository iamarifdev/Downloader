using FileDownloaderLib;
using Spectre.Console;
using YoutubeExplode.Videos.Streams;

if (args.Length < 1 || !args[0].StartsWith("--url="))
{
    throw new Exception($"The download URL should be passed as the first argument. i.e --url=\"<url>\"");
}


var progress = new Progress<double>();
progress.ProgressChanged += (sender, value) => {
    Console.Write($"\rDownload progress: {value:P2}".PadRight(20));
};

var url = args[0].Split("--url=")[1];
if (url.Contains("youtube.com"))
{
    var youtubeDownloader = new YoutubeDownloader();
    var streams = await youtubeDownloader.GetStreamsAsync(url);
    var muxedStreamInfos = streams as MuxedStreamInfo[] ?? streams.ToArray();
    
    var prompt = new SelectionPrompt<string>()
        .Title("Select an option:")
        .MoreChoicesText("[grey](Move up and down to reveal more choices)[/]")
        .AddChoices(muxedStreamInfos.Select(s => $"{s.VideoQuality.Label} - {s.Container}"));

    var result = AnsiConsole.Prompt(prompt);
    var stream = muxedStreamInfos.FirstOrDefault(s => $"{s.VideoQuality.Label} - {s.Container}" == result);
    
    await youtubeDownloader.DownloadVideoAsync(url, stream: stream);
    return;
}

if (!args[1].StartsWith("--filename="))
{
    throw new Exception("The download URL should be passed as the 2nd argument. i.e --filename=\"<filename>\"");
}


var filename = args[1].Split("=")[1];
Console.WriteLine($"URL to download: {url}");
Console.WriteLine($"Save file as: {filename}");

var downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
var destinationPath = Path.Combine(downloadFolder, filename);

FileDownloader fileDownloader = new();
await fileDownloader.DownloadFileAsync(url, destinationPath, progress);
Console.WriteLine("File download started. Continue with other work...");