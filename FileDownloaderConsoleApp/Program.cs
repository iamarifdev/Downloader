using FileDownloaderLib;

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
    await youtubeDownloader.DownloadVideoAsync(url);
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