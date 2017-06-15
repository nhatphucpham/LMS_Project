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

        /// <summary>
        /// HTML code by string List
        /// </summary>
        public static List<string> m_HTML;

        public string StringHtml { get; set; }

        /// <summary>
        /// Link Novels webpages
        /// </summary>
        public static Dictionary<int, string> NavLinks { get; set; }

        public static int CurrentPages { get; set; }

        protected List<Episode> episodes;
        protected List<Chapter> chapters;
        protected List<Novel> novels;
        
        public SourceAnalysis()
        {
            if (SourceAnalysis.m_HTML == null)
                SourceAnalysis.m_HTML = new List<string>();
            SourceAnalysis.CurrentPages = 0;
            SourceAnalysis.NavLinks = new Dictionary<int, string>();
        }

        /// <summary>
        /// Load html from Address
        /// </summary>
        /// <param name="Address">Web Address</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get Chapters List from Novel ID
        /// </summary>
        /// <param name="NovelId"></param>
        /// <returns></returns>
        public IEnumerable<Chapter> GetChaptersFromNovel(int NovelId)
        {
            using (var context = new DataManager())
            {
                try
                {
                    var Episodes = context.Episodes.Where(w => w.NovelId == NovelId);
                    if (Episodes != null)
                    {
                        foreach (var episode in Episodes)
                        {
                            var Chapters = context.Chapters.Where(w => w.EpisodeId == episode.EpisodeId).ToList();
                            foreach (var chapter in Chapters)
                            {
                                yield return chapter;
                            }
                        }
                    }
                }
                finally { }
            }

        }

        /// <summary>
        /// Get Novels List from Web Address
        /// </summary>
        /// <param name="webAddress"></param>
        /// <returns></returns>
        public IEnumerable<Novel> GetNovelFromWebSourse(string webAddress)
        {
            using (var context = new DataManager())
            {
                try
                {
                    int id = context.WebSourses.Single(w => w.Address == webAddress).WebId;
                    var novels = context.Novels.Where(w => w.WebId == id).ToList();
                    foreach (var novel in novels)
                    {
                        yield return novel;
                    }
                }
                finally { }
            }
        }

        /// <summary>
        /// Get Episode From Novel ID
        /// </summary>
        /// <param name="NovelId"></param>
        /// <returns></returns>
        public IEnumerable<Episode> GetEpisodesFromNovelId(int NovelId)
        {
            using (var context = new DataManager())
            {
                var Episodes = context.Episodes.Where(b => b.NovelId == NovelId).ToList();
                foreach (var episode in Episodes)
                {
                    try
                    {
                        yield return episode;
                    }
                    finally { }
                }
            }
        }

        /// <summary>
        /// Get Chapter List from Episode ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Chapter> GetChaptersFromEpisodeId(int id)
        {
            var chapters_List = new List<Chapter>();
            using (var context = new DataManager())
            {
                try
                {
                    var Chapters = context.Chapters.Where(b => b.EpisodeId == id).ToList();
                    foreach (var chapter in Chapters)
                    {
                        yield return chapter;
                    }
                }
                finally { }
            }
        }


        /// <summary>
        /// Get Chapter from ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Chapter GetChapter(int id)
        {
            var chapter = new Chapter();
            using (var context = new DataManager())
            {
                try
                {
                    chapter = context.Chapters.Single(b => b.ChapterId == id);
                }
                catch
                {
                    chapter = null;
                }
            }
            return chapter;
        }

        /// <summary>
        /// Get Episode from ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Episode GetEpisode(int id)
        {
            var episode = new Episode();
            using (var context = new DataManager())
            {
                try
                {
                    episode = context.Episodes.Single(b => b.EpisodeId == id);
                }
                catch
                {
                    episode = null;
                }
            }
            return episode;
        }

        /// <summary>
        /// Get Novel from ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Novel GetNovel(int id)
        {
            var novel = new Novel();
            using (var context = new DataManager())
            {
                novel = context.Novels.Single(b => b.NovelId == id);
            }
            return novel;
        }


        /// <summary>
        /// Get All Novel of Web Id
        /// </summary>
        /// <param name="WebId"></param>
        /// <returns></returns>
        public IEnumerable<Novel> GetNovels(int WebId)
        {
            using (var context = new DataManager())
            {
                try
                {
                    var Novels = context.Novels.Where(b => b.WebId == WebId).ToList();
                    foreach (var novel in Novels)
                    {
                        yield return novel;
                    }
                }
                finally { }
            }
        }

        bool one2Time = false;

        /// <summary>
        /// Check Internet conection
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get WebSource Of chapter
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WebSource GetWebSourceOfChapter(int id)
        {
            try
            {
                using (var context = new DataManager())
                {
                    int episodeId = context.Chapters.Single(s => s.ChapterId == id).EpisodeId;
                    int novelId = context.Episodes.Single(s => s.EpisodeId == episodeId).NovelId;
                    int webId = context.Novels.Single(s => s.NovelId == novelId).WebId;
                    return context.WebSourses.Single(s => s.WebId == webId);
                }
            }
            catch { return null; }
        }

        /// <summary>
        /// Load Chapter 
        /// </summary>
        /// <param name="AddressLine"></param>
        /// <param name="TitleLine"></param>
        /// <returns></returns>
        abstract protected Chapter GetChapterFromHtmlLine(string AddressLine, string TitleLine = "");

        /// <summary>
        /// Load Content of chapter
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        abstract public Task<List<string>> LoadContent(int id);

        /// <summary>
        /// Load Episode of novel
        /// </summary>
        /// <param name="NovelId"></param>
        abstract public void LoadEpisode(int NovelId = 1);

        /// <summary>
        /// Load Novels with main source
        /// </summary>
        abstract public void LoadNovel();


        /// <summary>
        /// Load all pages of website have novels
        /// </summary>
        abstract public void LoadNav();
    }
}
