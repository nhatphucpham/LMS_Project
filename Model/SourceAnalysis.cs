using LMS_Project.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace LMS_Project.Model
{
    public abstract class SourceAnalysis
    {
        public WebSource Sourse { get; set; }

        public static List<string> m_HTML;

        public string StringHtml { get; set; }

        public static Dictionary<int, string> NavLinks { get; set; }
        public static int CurrentPages { get; set; }

        protected List<NovelDetail> novelDetails;
        protected List<Episode> episodes;
        protected List<Chapter> chapters;
        protected List<EpisodeDetail> episodeDetails;

        protected List<Novel> novels;
        protected List<WebDetail> webDetails;


        public SourceAnalysis()
        {
            if (SourceAnalysis.m_HTML == null)
                SourceAnalysis.m_HTML = new List<string>();
            SourceAnalysis.CurrentPages = 0;
            SourceAnalysis.NavLinks = new Dictionary<int, string>();
        }

        public async Task<bool> LoadHTLM(string Address)
        {
            List<string> newcode = new List<string>();

            // Thread send request and receive html code to List String
            var httpClient = new HttpClient();

            try
            {
                var result = await httpClient.GetStreamAsync(Address);
                StreamReader read = new StreamReader(result);
                httpClient.Dispose();

                while (read.Peek() >= 0)
                {
                    newcode.Add(WebUtility.HtmlDecode(string.Format("{0}", read.ReadLine())));
                }
                if (!SourceAnalysis.m_HTML.Equals(newcode))
                    SourceAnalysis.m_HTML = newcode;
                return true;
            }
            catch
            {
                httpClient.Dispose();
            }
            return false;
        }

        public void RemoveEpisode()
        {
            using (var context = new DataManager())
            {
                if (context.Episodes.Count() > 0)
                {
                    var episodes = context.Episodes;
                    episodes.RemoveRange(episodes);
                    var chapters = context.Chapters;
                    chapters.RemoveRange(chapters);
                    var novelDetails = context.NovelDetails;
                    novelDetails.RemoveRange(novelDetails);
                    var episodeDetails = context.EpisodeDetails;
                    episodeDetails.RemoveRange(episodeDetails);
                    context.SaveChanges();
                }
            }
        }

        public List<Chapter> GetChaptersFromNovel(int NovelId)
        {
            var ChapterList = new List<Chapter>();
            using (var context = new DataManager())
            {
                var episodes = context.NovelDetails.Where(b => b.NovelId == NovelId).ToList();
                foreach (var episode in episodes)
                {
                    var chapters = context.EpisodeDetails.Where(c => c.EpisodeId == episode.EpisodeId).ToList();
                    foreach (var chapter in chapters)
                    {
                        ChapterList.AddRange(context.Chapters.Where(c => c.ChapterId == chapter.ChapterId));
                    }
                }
            }
            return ChapterList;
        }

        public List<Novel> GetNovelFromWebSourse(string webAddress)
        {
            var NovelList = new List<Novel>();
            using (var context = new DataManager())
            {
                try
                {
                    int id = context.WebSourses.Single(w => w.Address == webAddress).WebId;
                    var novels = context.WebDetails.Where(w => w.WebId == id).ToList();
                    foreach (var novel in novels)
                    {
                        NovelList.Add(context.Novels.Single(n => n.NovelId == novel.NovelId));
                    }
                }
                catch
                {
                    return null;
                }
            }
            return NovelList;
        }

        public List<Episode> GetEpisodesFromNovelId(int NovelId)
        {
            var EpisodeList = new List<Episode>();
            using (var context = new DataManager())
            {
                var episodes = context.NovelDetails.Where(b => b.NovelId == NovelId).ToList();
                foreach (var episode in episodes)
                {
                    EpisodeList.Add(context.Episodes.Single(b => b.EpisodeId == episode.EpisodeId));
                }
            }
            return EpisodeList;
        }

        public List<Chapter> GetChaptersFromEpisodeId(int id)
        {
            var chapters_List = new List<Chapter>();
            using (var context = new DataManager())
            {
                var chapters = context.EpisodeDetails.Where(b => b.EpisodeId == id).ToList();
                foreach (var chapter in chapters)
                {
                    chapters_List.Add(context.Chapters.Single(b => b.ChapterId == chapter.ChapterId));
                }
            }
            return chapters_List;
        }

        public Chapter GetChapter(int id)
        {
            var chapter = new Chapter();
            using (var context = new DataManager())
            {
                chapter = context.Chapters.Single(b => b.ChapterId == id);
            }
            return chapter;
        }
        public Episode GetEpisode(int id)
        {
            var episode = new Episode();
            using (var context = new DataManager())
            {
                episode = context.Episodes.Single(b => b.EpisodeId == id);
            }
            return episode;
        }

        public Novel GetNovel(int id)
        {
            var novel = new Novel();
            using (var context = new DataManager())
            {
                novel = context.Novels.Single(b => b.NovelId == id);
            }
            return novel;
        }

        public List<Novel> GetNovels(int WebId)
        {
            var NovelList = new List<Novel>();
            using (var context = new DataManager())
            {
                var Novels = context.WebDetails.Where(b => b.WebId == WebId).ToList();
                foreach (var novel in Novels)
                {
                    NovelList.Add(context.Novels.Single(b => b.NovelId == novel.NovelId));
                }
            }
            return NovelList;
        }

        bool one2Time = false;

        public async Task CheckConnection()
        {
            var httpClient = new HttpClient();

            try
            {
                var result = await httpClient.GetStreamAsync(@"http://www.sublightnovel.com/p/home.html");
                httpClient.Dispose();
            }
            catch
            {
                httpClient.Dispose();
                if (!one2Time)
                {
                    var dialog = new MessageDialog("Cannot access to internet!\r\nPlease check your internet conection", "No Internet");
                    dialog.Commands.Add(new UICommand("Try Again", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                    dialog.Commands.Add(new UICommand("Exit", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                    await dialog.ShowAsync();
                }
                else
                {
                    var dialog = new MessageDialog("Cannot access to internet!\r\nPlease check your internet conection", "No Internet");
                    dialog.Commands.Add(new UICommand("Exit", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                    await dialog.ShowAsync();
                }
            }
        }

        private async void CommandInvokedHandler(IUICommand command)
        {
            switch (command.Label)
            {
                case "Try Again":
                    one2Time = true;
                    await CheckConnection();
                    break;
                case "Exit":
                    Windows.UI.Xaml.Application.Current.Exit();
                    break;
            }
        }

        public static WebSource GetWebSourceOfChapter(int id)
        {
            using (var context = new DataManager())
            {
                var eDetail = context.EpisodeDetails.Single(s => s.ChapterId == id);
                var nDetai = context.NovelDetails.Single(s => s.EpisodeId == eDetail.EpisodeId);
                var wDetail = context.WebDetails.Single(s => s.NovelId == nDetai.NovelId);
                return context.WebSourses.Single(s => s.WebId == wDetail.WebId);
            }
        }

        abstract protected Chapter GetChapterFromHtmlLine(string AddressLine, string TitleLine = "");

        abstract public Task<List<string>> SetContent(int id);

        abstract public void LoadEpisode(int NovelId = 1);

        abstract public void LoadNovel();

        abstract public void LoadNav();

        abstract public void NextPage();

        abstract public void PreviousPage();
    }
}
