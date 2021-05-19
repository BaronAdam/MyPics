namespace MyPics.Infrastructure.Helpers.PaginationParameters
{
    public class LikeParameters
    {
        private const int MaxPageSize = 70;
        private readonly int _pageNumber = 1;
        private readonly int _pageSize = 50;

        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = value <= 0 ? 20 : value > MaxPageSize ? MaxPageSize : value;
        }

        public int PageNumber
        {
            get => _pageNumber;
            init => _pageNumber = value <= 0 ? 1 : value;
        }
    }
}