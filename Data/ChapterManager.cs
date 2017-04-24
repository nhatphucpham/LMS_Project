using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project.Data
{
    public class ChapterManager : DbContext
    {
        public string TitleE {
            get
            {
                return "Legendary Moonlight Sculptor"; 
            }
        }
        public string TitleV
        {
            get
            {
                return "Nhà điêu khắc ánh trăng huyền thoại";
            }
        }
        public string Author
        {
            get
            {
                return "Nam Hi-sung";
            }
        }
        public string Summany
        {
            get
            {
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} {13}", "Legendary Moonlight Sculptor",
                                    "– Con đường Đế Vương là câu chuyện về kẻ bị thế giới ruồng bỏ, kẻ là nô lệ cho",
                                    "đồng tiền, và đồng thời cũng là Thần Chiến Tranh huyền thoại của trò chơi MMORPG",
                                    "nổi tiếng Lục Địa Phép Thuật.Trước khi quyết định nghỉ game để đi làm, chút nỗ",
                                    "lực bán acc để kiếm chút tiền còn lại tạo ra một điều không tưởng với cậu.Qua",
                                    "một chuỗi các sự kiện tình cờ, nhân vật huyền thoại của cậu được đấu lên tới",
                                    "3,1 tỉ won(2,7 triệu $). Vui mừng chưa hết thì cậu đã sớm bị lũ chủ nợ cho vay",
                                    "nặng lãi tới xâu xé.Nhận ra được khả năng kiếm tiền từ game, cậu vực dậy và dấn",
                                    "thân vào một thời đại mới mở của game, dẫn đầu bởi trò chơi thực tế ảo Royal",
                                    "Road – Con đường đế vương. Legendary Moonlight Sculptor là huyền thoại về Lee",
                                    "Hyun trên con đường trở thành vị hoàng đế vĩ đại, tất cả chỉ nhờ vào tình yêu",
                                    "thương gia đình, sự khao khát giàu sang, một tinh thần gan góc và thân thể tôi",
                                    "luyện không ngừng.Mộ bộ truyện cực kỳ cuốn hút từ đầu chí cuối, kịch tính, nảy",
                                    "lửa và sục sôi.");
            }
        }

        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Episode> Episodes { get; set; }

        public DbSet<EpisodeDetail> EpisodeDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=db_lms.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpisodeDetail>().HasKey(t => new { t.ChapterId, t.EpisodeId });
        }

    }

}
