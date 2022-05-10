using System.Collections.Generic;

namespace Core.Models
{
    public class Room : BaseEntity<Room>
    {
        public bool IsOpen { get; set; }

        public bool IsAlert { get; set; }

        public int UnreadCount { get; set; }

        public string Name { get; set; }

        public RoomType Type { get; set; }

        public List<Message> Messages { get; set; }
    }
}