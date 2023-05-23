using Application.Services;
using Domain.Commands;

namespace Application.CommandHandlers;

public class YoutubeDownloadCommandHandler
{
    private readonly YoutubeService _youtubeService;

    public YoutubeDownloadCommandHandler(YoutubeService youtubeService)
    {
        _youtubeService = youtubeService;
    }
    
    public async Task HandleAsync(YoutubeDownloadCommand command)
    {
        await _youtubeService.DownloadVideoAsync(command.Url, command.Stream, command.DestinationPath);
    }
}