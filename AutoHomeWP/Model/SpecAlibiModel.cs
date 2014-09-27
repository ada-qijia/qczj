using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    [Table]
    public class SpecAlibiModel
    {
        [Column]
        public string Average { get; set; }

        [Column]
        public int PeopleNum { get; set; }

        [Column]
        public string FuelConsumption { get; set; }

        [Column]
        public int OilPeoplenum { get; set; }

        [Column]
        public string LevelName { get; set; }

        [Column]
        public int PageIndex { get; set; }

        [Column]
        public int PageCount { get; set; }

        [Column]
        public int PageSize { get; set; }

        [Column]
        public int Total { get; set; }

        [Column]
        public IEnumerable<AlibiGradeModel> Grades { get; set; }

        [Column]
        public IEnumerable<KoubeiModel> Koubeis { get; set; }
    }

    [Table]
    public class AlibiGradeModel
    {
        [Column]
        public string Name { get; set; }

        [Column]
        public string Grade { get; set; }

        [Column]
        public string LevelGrade { get; set; }
    }

    [Table]
    public class KoubeiModel
    {
        [Column]
        public int ID { get; set; }

        [Column]
        public string SpecName { get; set; }

        [Column]
        public IEnumerable<KoubeiMedalModel> Medals { get; set; }

        [Column]
        public string UserName { get; set; }

        [Column]
        public int UserID { get; set; }

        [Column]
        public int IsAuth { get; set; }

        [Column]
        public string PostTime { get; set; }

        [Column]
        public string Content { get; set; }

        [Column]
        public string UserPic { get; set; }

        [Column]
        public string SeriesName { get; set; }
    }

    [Table]
    public class KoubeiMedalModel
    {
        [Column]
        public int Type { get; set; }

        [Column]
        public string Name { get; set; }
    }

}
