using System.Text.Json.Serialization;

namespace TrailBlog.Api.Models
{
    public sealed class PagedResultDto<T>
    {
        public required IEnumerable<T> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
