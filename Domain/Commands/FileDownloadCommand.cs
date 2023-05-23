namespace Domain.Commands;

public record FileDownloadCommand(string Url, string DestinationPath, IProgress<double>? Progress = null);