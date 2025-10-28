﻿namespace TrailBlog.Api.Models
{
    public class RecentViewedPostDto
    {
        public Guid PostId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string CommunityName { get; set; } = string.Empty;

        public int TotalComment { get; set; }

        public int TotalReactions { get; set; }

    }
}
