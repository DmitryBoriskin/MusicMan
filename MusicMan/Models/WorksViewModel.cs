using cms.dbModel.entity;
using System;

namespace MusicMan.Models
{
    public class WorksViewModel : CoreViewModel
    {
        public WorkModel Item { get; set; }
        public WorkList List { get; set; }
    }

    public class LikeModel
    {
        public Guid UserId { get; set; }
        public Guid WorkId { get; set; }

    }
}

namespace MusicMan.Models.Vimeo
{
    public class VimeoVideo
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string upload_date { get; set; }
        public string thumbnail_small { get; set; }
        public string thumbnail_medium { get; set; }
        public string thumbnail_large { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_url { get; set; }
        public string user_portrait_small { get; set; }
        public string user_portrait_medium { get; set; }
        public string user_portrait_large { get; set; }
        public string user_portrait_huge { get; set; }
        public int stats_number_of_likes { get; set; }
        public int stats_number_of_plays { get; set; }
        public int stats_number_of_comments { get; set; }
        public int duration { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string tags { get; set; }
        public string embed_privacy { get; set; }
    }

}
namespace MusicMan.Models.Rutube
{
    public class RutubeVideo
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string short_description { get; set; }
        public string thumbnail_url { get; set; }
        public DateTime created_ts { get; set; }
        public string video_url { get; set; }
        public int track_id { get; set; }
        public int hits { get; set; }
        public int duration { get; set; }
        public string picture_url { get; set; }
        public Author author { get; set; }
        public bool is_adult { get; set; }
        public DateTime publication_ts { get; set; }
        public object[] hashtags { get; set; }
        public bool is_livestream { get; set; }
        public int comments_count { get; set; }
        public DateTime last_update_ts { get; set; }
        public bool is_hidden { get; set; }
        public string html { get; set; }
        public bool for_registered { get; set; }
        public bool for_linked { get; set; }
        public bool has_high_quality { get; set; }
        public bool is_deleted { get; set; }
        public string embed_url { get; set; }
        public string source_url { get; set; }
        public bool is_external { get; set; }
        public Category category { get; set; }
        public object rutube_poster { get; set; }
        public bool is_official { get; set; }
        public int action_reason { get; set; }
        public object pepper { get; set; }
        public string comment_editors { get; set; }
        public string show { get; set; }
        public string persons { get; set; }
        public string genres { get; set; }
        public object music { get; set; }
        public object[] all_tags { get; set; }
        public Restrictions restrictions { get; set; }
        public string feed_url { get; set; }
        public string feed_name { get; set; }
        public string feed_subscription_url { get; set; }
        public int feed_subscribers_count { get; set; }
    }

    public class Author
    {
        public int id { get; set; }
        public string name { get; set; }
        public string avatar_url { get; set; }
        public string site_url { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public string category_url { get; set; }
        public string name { get; set; }
    }

    public class Restrictions
    {
        public Country country { get; set; }
    }

    public class Country
    {
        public string[] restricted { get; set; }
        public string[] allowed { get; set; }
    }

}