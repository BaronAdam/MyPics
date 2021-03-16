namespace MyPics.Infrastructure.Helpers.PaginationParameters
{
    public class UserParameters
    {
        private const int MaxPageSize = 50;
        private readonly int _pageNumber = 1;
        private readonly int _pageSize = 20;

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