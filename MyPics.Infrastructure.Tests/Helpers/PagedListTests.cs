using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MyPics.Infrastructure.Helpers;
using NUnit.Framework;
using MockQueryable.FakeItEasy;
using MyPics.Domain.Models;

namespace MyPics.Infrastructure.Tests.Helpers
{
    [TestFixture]
    public class PagedListTests
    {
        [Test]
        public void TestAll_FromConstructor_ExpectedResult()
        {
            var list = new List<string>();
            for (var i = 0; i < 50; i++)
            {
                list.Add("item");
            }
            
            var result = new PagedList<string>(list, 1, 50, 10);
            
            result.CurrentPage.Should().Be(1);
            result.TotalPages.Should().Be(5);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(50);
            result.Count.Should().Be(50);
        }
        
        [Test]
        public void TestAll_FromProperties_ExpectedResult()
        {
            var list = new List<string>();
            for (var i = 0; i < 50; i++)
            {
                list.Add("item");
            }

            var result = new PagedList<string>(list, 0, 0, 0)
            {
                CurrentPage = 1,
                TotalPages = 5,
                PageSize = 10,
                TotalCount = 50,
            };
            
            result.CurrentPage.Should().Be(1);
            result.TotalPages.Should().Be(5);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(50);
            result.Count.Should().Be(50);
        }

        [Test]
        public async Task CreateAsync_Successful_ExpectedResult()
        {
            var list = new List<User>();
            for (var i = 0; i < 50; i++)
            {
                list.Add(new User {Id = i});
            }

            var queryable = list.AsQueryable().BuildMock();

            var result = await PagedList<User>.CreateAsync(queryable, 3, 10);

            result.Should().NotBeEmpty();
            result.CurrentPage.Should().Be(3);
            result.TotalCount.Should().Be(50);
            result.TotalPages.Should().Be(5);
            result.PageSize.Should().Be(10);
            result[0].Id.Should().Be(20);
        }
    }
}