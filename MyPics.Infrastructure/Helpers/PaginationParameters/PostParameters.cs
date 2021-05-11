namespace MyPics.Infrastructure.Helpers.PaginationParameters
{
    public class PostParameters
    {
        private const int MaxPageSize = 30;
        private readonly int _pageNumber = 1;
        private readonly int _pageSize = 21;

        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = value <= 0 ? 21 : value > MaxPageSize ? MaxPageSize : value;
        }

        public int PageNumber
        {
            get => _pageNumber;
            init => _pageNumber = value <= 0 ? 1 : value;
        }
    }
}