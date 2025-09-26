namespace TrailBlog.Api.Entities
{
    public class UserCommunity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid CommunityId { get; set; }
        public Community Community { get; set; } = null!;
        public DateTime JoinedDate { get; set; }

    }
}
