using System.Collections.Generic;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType
{
    public class GetAnnouncementCountByTypeQueryResponse
    {
        public int Count { get; set; }
        public string Type { get; set; }
        public string ImagePath { get; set; }
    }
}
