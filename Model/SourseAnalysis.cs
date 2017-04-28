using LMS_Project.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Project.Model
{
    public abstract class SourseAnalysis
    {
        public WebSourse Sourse { get; set; }

        public static List<string> m_HTML;

        public string StringHtml { get; set; }

        protected List<Episode> episodes;
        protected List<Chapter> chapters;
        protected List<EpisodeDetail> details;

        public SourseAnalysis()
        {
            if (m_HTML == null)
                m_HTML = new List<string>();
            if (episodes == null)
            {
                episodes = new List<Episode>();
                chapters = new List<Chapter>();
                details = new List<EpisodeDetail>();
            }
        }

        public async Task LoadHTLM()
        {
            List<string> newcode = new List<string>();

            // Thread send request and receive html code to List String
            var httpClient = new HttpClient();

            try
            {
                var result = await httpClient.GetStreamAsync(Sourse.Address);
                StreamReader read = new StreamReader(result);
                httpClient.Dispose();

                while (read.Peek() >= 0)
                {
                    newcode.Add(WebUtility.HtmlDecode(string.Format("{0}", read.ReadLine())));
                }
                if (m_HTML.Equals(newcode) && m_HTML.Count != 0)
                    return;
                m_HTML = newcode;
            }
            catch
            {
                httpClient.Dispose();
            }
        }

        public List<Chapter> GetChaptersList()
        {
            var chapters_List = new List<Chapter>();
            using (var context = new SourseManager())
            {
                chapters_List = context.Chapters.ToList();
            }
            return chapters_List;
        }

        public List<Episode> GetEpisodesList()
        {
            var episodes_List = new List<Episode>();
            using (var context = new SourseManager())
            {
                episodes_List = context.Episodes.ToList();
            }
            return episodes_List;
        }

        public List<Chapter> GetChaptersFromEpisodeId(int id)
        {
            var chapters_List = new List<Chapter>();
            using (var context = new SourseManager())
            {
                var chapters = context.EpisodeDetails.Where(b => b.EpisodeId == id).ToList();
                foreach (var chapter in chapters)
                {
                    chapters_List.Add(context.Chapters.Single(b => b.ChapterId == chapter.ChapterId));
                }
            }
            return chapters_List;
        }

        public Chapter GetChapterFromChapterId(int id)
        {
            var chapter = new Chapter();
            using (var context = new SourseManager())
            {
                chapter = context.Chapters.Single(b => b.ChapterId == id);
            }
            return chapter;
        }
        public Episode GetEpisodeFromEpisodeId(int id)
        {
            var episode = new Episode();
            using (var context = new SourseManager())
            {
                episode = context.Episodes.Single(b => b.EpisodeId == id);
            }
            return episode;
        }
        abstract protected Chapter GetChapterFromHtmlLine(string Line);

        abstract public Task<List<string>> SetContent(int id);

        abstract public void LoadData();
    }
}
