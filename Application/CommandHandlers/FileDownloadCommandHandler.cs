using Domain.Commands;

namespace Application.CommandHandlers;

public class FileDownloadCommandHandler
{
    private readonly HttpClient _client;

    public FileDownloadCommandHandler(HttpClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(FileDownloadCommand command, CancellationToken cancellationToken = default)
    {
        using var response = await _client.GetAsync(command.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await using Stream contentStream = await response.Content.ReadAsStreamAsync(),
            fileStream = new FileStream(command.DestinationPath, FileMode.Create, FileAccess.Write, FileShare.None,
                8192, true);

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
        var bytesCopied = 0L;
        var buffer = new byte[8192];
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            bytesCopied += bytesRead;
            
            command.Progress?.Report((double)bytesCopied / totalBytes);
        }
    }
}