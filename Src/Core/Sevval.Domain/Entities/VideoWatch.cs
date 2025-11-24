using System;

namespace Sevval.Domain.Entities
{
    public class VideoWatch
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public string UserId { get; set; }
        public DateTime WatchedAtUtc { get; set; }
    }
}


