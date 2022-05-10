using System;
using System.Collections.Generic;

namespace Core.Models
{
    public class NewsFeed : BaseEntity<NewsFeed>
    {
        public int WeekNo { get; set; }
        public string Type { get; set; }
        public List<NewsSection> Sections { get; set; }
    }

    public class NewsSection
    {
        public Guid NewsFeedId { get; set; }
        public string Reference { get; set; }
        public string Title { get; set; }

        public List<NewsItem> Items { get; set; }
    }

    public class NewsItem
    {
        public Guid NewsFeedId { get; set; }
        public string NewsSectionReference { get; set; }
        public int SeqNo { get; set; }
        public string Text { get; set; }
    }
}
