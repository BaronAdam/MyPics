using System.Collections.Generic;

namespace MyPics.Domain.Models
{
    public class UserConversation
    {
        public int UserId { get; set; }
        public int ConversationId { get; set; }

        public virtual ICollection<Conversation> Conversations { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}