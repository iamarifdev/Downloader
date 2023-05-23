using Library;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Application.Services;

public class YoutubeService
{
    private readonly YoutubeClient _youtubeClient;

    public YoutubeService(YoutubeClient youtubeClient)
    {
        _youtubeClient = youtubeClient;
    }

    public async Task<IEnumerable<MuxedStreamInfo>> GetStreamsAsync(string url)
    {
        var videoId = ParseVideoId(url);
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoId);
        return streamManifest.GetMuxedStreams();
    }

    public string ParseVideoId(string url)
    {
        var videoId = url.Split("=")[1];
        return videoId;
    }

    public async Task DownloadVideoAsync(string url, MuxedStreamInfo? stream = null, string? destinationPath = null)
    {
        var videoId = ParseVideoId(url);
        var video = await _youtubeClient.Videos.GetAsync(videoId);
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(video.Id);
        var streamInfo = stream ?? streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

        // Compose a file name, ensuring it doesn't contain illegal characters
        var fileName = $"{video.Title}.{streamInfo.Container}";
        fileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c, '-'));

        var filePath = GetYoutubeVideoFileDownloadPath(fileName, destinationPath);

        using (var progress = new ConsoleProgress())
            await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);

        Console.WriteLine("Done");
        Console.WriteLine($"Video saved to '{fileName}'");
    }


    private static string GetYoutubeVideoFileDownloadPath(string fileName, string? destinationPath = null)
    {
        if (!string.IsNullOrEmpty(destinationPath))
        {
            return destinationPath;
        }

        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var downloadFolder = Path.Combine(userProfilePath, "Downloads", "Youtube");
        
        if (!Directory.Exists(downloadFolder))
        {
            Directory.CreateDirectory(downloadFolder);
        }

        var filePath = Path.Combine(downloadFolder, fileName);
        return filePath;
    }
}