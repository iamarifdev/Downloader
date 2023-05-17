using System.Net;
using System;

namespace FileDownloaderLib;

public class FileDownloader
{
    public async Task DownloadFileAsync(string url, string destinationPath, IProgress<double>? progress = null)
    {
        using HttpClient client = new();

        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        await using Stream contentStream = await response.Content.ReadAsStreamAsync(), 
            fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
        var bytesCopied = 0L;
        var buffer = new byte[8192];
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            bytesCopied += bytesRead;
            progress?.Report((double)bytesCopied / totalBytes);
        }
    }

}