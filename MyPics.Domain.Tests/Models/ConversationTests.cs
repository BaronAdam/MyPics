using System.Collections.Generic;
using FluentAssertions;
using MyPics.Domain.Models;
using NUnit.Framework;

namespace MyPics.Domain.Tests.Models
{
    [TestFixture]
    public class ConversationTests
    {
        [Test]
        public void TestAll_ExpectedResult()
        {
            var entity = new Conversation
            {
                Id = 1,
                Messages = new List<Message>()
            };

            entity.Should().NotBeNull();
            entity.Id.Should().Be(1);
            entity.Messages.Should().BeEmpty();
        }
    }
}