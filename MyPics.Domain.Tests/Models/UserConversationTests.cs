using System.Collections.Generic;
using FluentAssertions;
using MyPics.Domain.Models;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class UserConversationTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new UserConversation
            {
                UserId = 1,
                ConversationId = 1,
                Conversations = new List<Conversation>(),
                Users = new List<User>()
            };

            entity.Should().NotBeNull();
            entity.UserId.Should().Be(1);
            entity.ConversationId.Should().Be(1);
            entity.Users.Should().BeEmpty();
            entity.Conversations.Should().BeEmpty();
        }
    }
}