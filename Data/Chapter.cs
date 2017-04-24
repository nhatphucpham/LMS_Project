using System;

namespace LMS_Project.Data
{
    public class Chapter
    {
        public int ChapterId{get; set;}

        public string Name { get; set; }
        public int NumberInEpisode { get; set; }
        public string WebAddress { get; set; }

        public string Content { get; set; }
    }
}
