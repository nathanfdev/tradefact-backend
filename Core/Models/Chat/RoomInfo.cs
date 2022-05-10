using System;

namespace Core.Models
{
    public class RoomInfo
    {
        public DateTime? Timestamp { get; set; }

        public RoomType Type { get; set; }

        public string Name { get; set; }

        public DateTime? LastMessage { get; set; }

        public int? MessageCount { get; set; }

    }
}