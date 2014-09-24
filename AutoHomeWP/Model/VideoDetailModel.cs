using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    [Table]
    public class VideoDetailModel
    {
        [Column]
        public int ID { get; set; }

        [Column]
        public string Title { get; set; }
        
        [Column]
        public string ShortTitle { get; set; }

        [Column]
        public string PicUrl { get; set; }

        [Column]
        public string Duration { get; set; }

        [Column]
        public int PlayTimes { get; set; }

        [Column]
        public int CommentNum { get; set; }

        [Column]
        public string InputTime { get; set; }

        [Column]
        public int CategoryID { get; set; }

        [Column]
        public string CategoryName { get; set; }

        [Column]
        public string Link { get; set; }

        [Column]
        public string Description { get; set; }

        [Column]
        public string Pic_400 { get; set; }

        [Column]
        public string NickName { get; set; }

        [Column]
        public string VideoAddress { get; set; }

        [Column]
        public string YoukuVideoKey { get; set; }

        [Column]
        public IEnumerable<RelationVideoModel> RelationVideoList { get; set; }

    }

    [Table]
    public class RelationVideoModel
    {
        [Column]
        public int ID { get; set; }

        [Column]
        public string Title { get; set; }

        [Column]
        public string PicUrl { get; set; }

        [Column]
        public string InputTime { get; set; }

        [Column]
        public int PlayTimes { get; set; }
    }
}
