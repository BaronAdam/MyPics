using System;
using FluentAssertions;
using FluentAssertions.Extensions;
using MyPics.Domain.Models;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class MessageTests
    {
        private readonly DateTime _testDate = new DateTime(2021, 02, 13);
        
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new Message
            {
                Id = 1,
                UserId = 1,
                RecipientId = 1,
                User = new User(),
                Conversation = new Conversation(),
                Content = "Test Content",
                IsPhoto = true,
                Url = "test.com/test",
                DateRead = _testDate,
                DateSent = _testDate
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.UserId.Should().Be(1);
            entity.RecipientId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new User());
            entity.Conversation.Should().BeEquivalentTo(new Conversation());
            entity.Content.Should().Be("Test Content");
            entity.IsPhoto.Should().BeTrue();
            entity.Url.Should().Be("test.com/test");
            entity.DateRead.Should().Be(_testDate);
            entity.DateSent.Should().Be(_testDate);
        }
        
        [Test]
        public void TestAll_NullDateRead_ExpectedResult()
        {
            var entity = new Message
            {
                Id = 1,
                UserId = 1,
                RecipientId = 1,
                User = new User(),
                Conversation = new Conversation(),
                Content = "Test Content",
                IsPhoto = true,
                Url = "test.com/test",
                DateSent = _testDate
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.UserId.Should().Be(1);
            entity.RecipientId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new User());
            entity.Conversation.Should().BeEquivalentTo(new Conversation());
            entity.Content.Should().Be("Test Content");
            entity.IsPhoto.Should().BeTrue();
            entity.Url.Should().Be("test.com/test");
            entity.DateRead.Should().BeNull();
            entity.DateSent.Should().Be(_testDate);
        }
        
        [Test]
        public void TestAll_DefaultDateSent_ExpectedResult()
        {
            var entity = new Message
            {
                Id = 1,
                UserId = 1,
                RecipientId = 1,
                User = new User(),
                Conversation = new Conversation(),
                Content = "Test Content",
                IsPhoto = true,
                Url = "test.com/test",
                DateRead = _testDate
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.UserId.Should().Be(1);
            entity.RecipientId.Should().Be(1);
            entity.User.Should().BeEquivalentTo(new User());
            entity.Conversation.Should().BeEquivalentTo(new Conversation());
            entity.Content.Should().Be("Test Content");
            entity.IsPhoto.Should().BeTrue();
            entity.Url.Should().Be("test.com/test");
            entity.DateRead.Should().Be(_testDate);
            entity.DateSent.Should().BeCloseTo(DateTime.UtcNow, 0.1.Seconds());
        }
    }
}