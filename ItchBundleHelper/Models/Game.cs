namespace ItchBundleHelper.Models
{
    public class Game
    {
        public int id { get; set; }
        public Bundle_Game bundle_game { get; set; }
        public string url { get; set; }
        public string[] platforms { get; set; }
        public User user { get; set; }
        public string title { get; set; }
        public string cover { get; set; }
        public string cover_color { get; set; }
        public string classification { get; set; }
        public string short_text { get; set; }
        public string price { get; set; }
        public string gif_cover { get; set; }
    }
}