using FluentAssertions;
using MyPics.Api.Helpers;
using NUnit.Framework;

namespace MyPics.Api.Tests.Helpers
{
    [TestFixture]
    public class PaginationHeaderTests
    {
        [Test]
        public void TestAll_FromConstructor_ExpectedResult()
        {
            var result = new PaginationHeader(1, 2, 3, 4);

            result.Should().NotBeNull();
            result.CurrentPage.Should().Be(1);
            result.ItemsPerPage.Should().Be(2);
            result.TotalItems.Should().Be(3);
            result.TotalPages.Should().Be(4);
        }

        [Test]
        public void TestAll_FromSetters_ExpectedResult()
        {
            var result = new PaginationHeader(0, 0, 0, 0)
            {
                CurrentPage = 1, ItemsPerPage = 2, TotalItems = 3, TotalPages = 4
            };

            result.Should().NotBeNull();
            result.CurrentPage.Should().Be(1);
            result.ItemsPerPage.Should().Be(2);
            result.TotalItems.Should().Be(3);
            result.TotalPages.Should().Be(4);
        }
    }
}