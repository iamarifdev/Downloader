using YoutubeExplode.Videos.Streams;

namespace Domain.Commands;

public record YoutubeDownloadCommand(string Url, MuxedStreamInfo? Stream, string? DestinationPath);