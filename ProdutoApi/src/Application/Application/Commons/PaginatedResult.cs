namespace Application.Commons;

public class PaginatedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public PaginatedResult()
    {
        Items = new List<T>();
    }
}