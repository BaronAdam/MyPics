using System.Collections.Generic;

namespace MyPics.Domain.Models
{
    public class Conversation
    {
        public int Id { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}