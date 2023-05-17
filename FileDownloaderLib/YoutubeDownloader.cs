using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace FileDownloaderLib;

public class YoutubeDownloader
{
    public async Task DownloadVideoAsync(string url, string? destinationPath = null)
    {
        var youtube = new YoutubeClient();
        var videoId = url.Split("=")[1];
        var video = await youtube.Videos.GetAsync(videoId); // replace with your video id
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
        var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

        // Compose a file name, ensuring it doesn't contain illegal characters
        var fileName = $"{video.Title}.{streamInfo.Container}";
        fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '-'));

        var filePath = GetYoutubeVideoFileDownloadPath(fileName, destinationPath);
        // await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);

        using (var progress = new ConsoleProgress())
            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);

        Console.WriteLine("Done");
        Console.WriteLine($"Video saved to '{fileName}'");
    }

    private static string GetYoutubeVideoFileDownloadPath(string fileName, string? destinationPath = null)
    {
        if (!string.IsNullOrEmpty(destinationPath))
        {
            return destinationPath;
        }
        
        // Save to the output directory
        var downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads",
            "Youtube");
        if (!Directory.Exists(downloadFolder))
        {
            Directory.CreateDirectory(downloadFolder);
        }

        var filePath = Path.Combine(downloadFolder, fileName);
        return filePath;
    }
}