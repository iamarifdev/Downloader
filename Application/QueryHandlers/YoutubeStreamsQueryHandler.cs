using Application.Services;
using Domain.Queries;
using YoutubeExplode.Videos.Streams;

namespace Application.QueryHandlers;

public class YoutubeStreamsQueryHandler
{
    private readonly YoutubeService _youtubeService;

    public YoutubeStreamsQueryHandler(YoutubeService youtubeService)
    {
        _youtubeService = youtubeService;
    }
    
    public async Task<IEnumerable<MuxedStreamInfo>> HandleAsync(YoutubeStreamsQuery query)
    {
        return await _youtubeService.GetStreamsAsync(query.Url);
    }
}