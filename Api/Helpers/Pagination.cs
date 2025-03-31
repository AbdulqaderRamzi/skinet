namespace Api.Helpers;

public class Pagination<T>(int pageSize, int pageIndex, int count, IReadOnlyList<T> data)
{
    public int PageIndex { get; set; } = pageIndex;
    public int PageSize { get; set; } = pageSize;
    public int Count { get; set; } = count;
    public IReadOnlyList<T> Date { get; set; } = data;
}