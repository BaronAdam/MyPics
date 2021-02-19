using System;
using System.ComponentModel.DataAnnotations;

namespace MyPics.Domain.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RecipientId { get; set; }
        public User User { get; set; }
        public Conversation Conversation { get; set; }
        [Encrypted] public string Content { get; set; }
        public bool IsPhoto { get; set; }
        [Encrypted] public string Url { get; set; }
        public DateTime DateSent { get; set; } = DateTime.UtcNow;
        public DateTime? DateRead { get; set; }
    }
}