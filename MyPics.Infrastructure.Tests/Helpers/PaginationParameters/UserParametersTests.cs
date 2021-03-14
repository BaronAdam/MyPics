using FluentAssertions;
using MyPics.Infrastructure.Helpers.PaginationParameters;
using NUnit.Framework;

namespace MyPics.Infrastructure.Tests.Helpers.PaginationParameters
{
    [TestFixture]
    public class UserParametersTests
    {
        [TestCase(10)]
        [TestCase(30)]
        [TestCase(60)]
        public void TestAll_ExpectedResult(int pageSize)
        {
            var result = new UserParameters
            {
                PageNumber = 1,
                PageSize = pageSize
            };

            result.PageNumber.Should().Be(1);
            result.PageSize.Should().BePositive();
            result.PageNumber.Should().BeLessOrEqualTo(50);
        }
    }
}