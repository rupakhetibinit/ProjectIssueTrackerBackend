namespace ProjectIssueTracker.Extensions
{
    public class PaginatedResult<TEntity> 
    {
        public IEnumerable<TEntity> Items { get; set; }
        public int TotalCount { get; set; }
    }
    public static class PaginationHelper
    {
        public static PaginatedResult<TEntity> Paginate<TEntity>(
            this IQueryable<TEntity> query,
            int page,
            int pageSize)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than or equal to 1.");

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than or equal to 1.");

            int totalCount = query.Count();

            IEnumerable<TEntity> items = query.Skip((page - 1) * pageSize).Take(pageSize).AsEnumerable();

            return new PaginatedResult<TEntity>
            {
                Items = items,
                TotalCount = totalCount
            };
        }
    }
}
