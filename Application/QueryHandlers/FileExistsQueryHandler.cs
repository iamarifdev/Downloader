using Domain.Queries;

namespace Application.QueryHandlers;

public class FileExistsQueryHandler
{
    public bool Handle(FileExistsQuery query)
    {
        return File.Exists(query.FilePath);
    }
}