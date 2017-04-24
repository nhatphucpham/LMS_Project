using System.Collections.Generic;
using LMS_Project.Data;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace LMS_Project.Model
{
    public class Sublightnovel
    {
        public static List<string> m_HTML;

        public Sublightnovel()
        {
            if(m_HTML == null)
                m_HTML = new List<string>();
        }

        public void LoadHTLM()
        {
            List<string> newcode = new List<string>();

            // Thread send request and receive html code to List String
            Task.Run(async () =>
            {
                HttpWebRequest request = WebRequest.Create(@"http://www.sublightnovel.com/p/home.html") as HttpWebRequest;
                HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

                Stream receive = response.GetResponseStream();

                StreamReader read = new StreamReader(receive);

                while (read.Peek() >= 0)
                {
                    newcode.Add(WebUtility.HtmlDecode(string.Format("{0}", read.ReadLine())));
                }
            }).Wait();

            if (m_HTML.Equals(newcode) && m_HTML.Count != 0)
                return;
            m_HTML = newcode;
        }

        public void UpdateChapter(int id, Chapter chapter)
        {
            using (var context = new ChapterManager())
            {
                Chapter member = context.Chapters.Single(b => b.ChapterId == id);
                member.Name = chapter.Name;
                member.Content = chapter.Content;
                member.WebAddress = chapter.WebAddress;
            }
        }
        public List<Chapter> GetChaptersList()
        {
            var chapters_List = new List<Chapter>();
            using (var context = new ChapterManager())
            {
                chapters_List = context.Chapters.ToList();
            }
            return chapters_List;
        }

        public List<Episode> GetEpisodesList()
        {
            var episodes_List = new List<Episode>();
            using (var context = new ChapterManager())
            {
                episodes_List = context.Episodes.ToList();
            }
            return episodes_List;
        }

        public List<Chapter> GetChaptersFromEpisodeId(int id)
        {
            var chapters_List = new List<Chapter>();
            using (var context = new ChapterManager())
            {
                var chapters = context.EpisodeDetails.Where(b=>b.EpisodeId == id).ToList();
                foreach (var chapter in chapters)
                {
                    chapters_List.Add(context.Chapters.Single(b=>b.ChapterId==chapter.ChapterId));
                }
            }
            return chapters_List;
        }

        public Chapter GetChapterFromChapterId(int id)
        {
            var chapter = new Chapter();
            using (var context = new ChapterManager())
            {
                chapter = context.Chapters.Single(b => b.ChapterId == id);
            }
            return chapter;
        }

        public Episode GetEpisodeFromEpisodeId(int id)
        {
            var episode = new Episode();
            using (var context = new ChapterManager())
            {
                episode = context.Episodes.Single(b => b.EpisodeId == id);
            }
            return episode;
        }

        public async Task<List<string>> SetupContent(int id)
        {
            using (var context = new ChapterManager())
            {
                var chapter = context.Chapters.Single(b => b.ChapterId == id);
                List<string> content_HTML = new List<string>();
                //HttpWebRequest request = WebRequest.Create(chapter.WebAddress) as HttpWebRequest;
                //HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

                var httpClient = new HttpClient();
                try
                {
                    var result = await httpClient.GetStreamAsync(chapter.WebAddress);
                    StreamReader read = new StreamReader(result);
                    bool write = false;
                    while (read.Peek() >= 0)
                    {
                        string str = await read.ReadLineAsync();
                        if (str.Contains("</style>"))
                            write = true;
                        if (str.Contains("reaction-buttons"))
                            write = false;
                        if (write)
                        {
                            content_HTML.Add(WebUtility.HtmlDecode(str));
                        }
                    }
                    httpClient.Dispose();
                    for (int i = 0; i < content_HTML.Count; i++)
                    {
                        string item = content_HTML[i];
                        if (item.Contains(chapter.WebAddress))
                        {
                            content_HTML.RemoveAt(i);
                            i--;
                            write = true;
                            continue;
                        }
                        if (item.Contains("star-ratings"))
                            write = false;
                        if (write)
                        {
                            while (item.Contains("<"))
                            {
                                if (item.Contains(">") && item.IndexOf("<") < item.IndexOf(">"))
                                    item = item.Remove(item.IndexOf("<"), item.IndexOf(">") - item.IndexOf("<") + 1);
                                else
                                {
                                    item = item.Remove(item.IndexOf("<"), 4);
                                }
                            }

                            content_HTML[i] = item;

                            if (item == "")
                            {
                                if (i > 0 && content_HTML[i - 1] == "")
                                {
                                    content_HTML.RemoveAt(i);
                                    i--;
                                }
                            }
                            else
                            {
                                if (i > 0 && content_HTML[i - 1] != "")
                                {
                                    content_HTML[i - 1] += string.Format(" {0}", item);
                                    content_HTML.RemoveAt(i);
                                    i--;
                                    continue;
                                }
                                context.Chapters.Single(c => c.ChapterId == id).Content += item;
                            }
                        }
                        else
                        {
                            content_HTML.RemoveAt(i);
                            i--;
                        }
                    }
                    return content_HTML;
                }
                catch
                {
                    httpClient.Dispose();
                    return null;
                }
                
            }
        }
        
        private Chapter GetChapterFromHtmlLine(string Line)
        {
            string[] spl = System.Text.RegularExpressions.Regex.Split(Line, @"<\W?\w{1,4}\W?\w?\d?(\s\w{4}\W""http://www.sublightnovel.com/\d{4,}/\d{2,}([\w-\W]*).html""( target=""_blank"")?)?\s?\W?>");
            string chap = "", title = "", address = "", ep = "";
            foreach (string s in spl)
            {
                if (s.Contains("Tập") )
                {
                    ep = s.Substring(s.IndexOf("T"), s.IndexOf(",") - (s.IndexOf("T")));
                }
                else if (s.Contains(":") && char.IsNumber(s[s.IndexOf(":") - 1]))
                {
                    if (s.Contains("chư") || s.Contains("Chư"))
                        chap = s.Remove(s.IndexOf(":"));
                    else
                        chap = string.Format("Chương {0}", s.Remove(s.IndexOf(":")));
                }
                else if (s.Contains("href"))
                {
                    int i = s.IndexOf("\"") + 1;
                    int size = s.IndexOf("\"", i) - i;
                    address = s.Substring(i, size);
                }
                else if (!s.Contains("/") && !s.Equals("") && !s.Contains("\"_blank\""))
                {
                    title = s;
                    break;
                }
            }

            if ((chap == "") || (title == "") || (address == ""))
            {
                return null;
            }
            using (var context = new ChapterManager())
            {
                int iEp = (ep != "") ? int.Parse(ep.Substring(ep.Length - 1)) : 0;
                if (context.Chapters.Where(b => b.Name == title).Count() == 0)
                {
                    Chapter chapter = new Chapter()
                    {
                        ChapterId = context.Chapters.Count() + 1,
                        Name = title,
                        Content = null,
                        WebAddress = address
                    };
                    if (iEp > 0)
                    {
                        if (context.EpisodeDetails.Where(b => b.ChapterId == chapter.ChapterId && b.EpisodeId == iEp).Count() == 0)
                        {
                            context.EpisodeDetails.Add(new EpisodeDetail() { EpisodeId = iEp, ChapterId = chapter.ChapterId });
                        }
                    }
                    return chapter;
                }
            }
            return null;
        }

        //Find and detect and set the chapters and the episodes
        public void LoadData()
        {
            bool Begin = false;
            string strEpi = "";
            int iEpi = -1;
            foreach (string s in m_HTML)
            {
                if (s.Contains("<h4>")) Begin = true;
                if (s.Contains("</h4>")) break;
                if (Begin)
                {
                    if (s.Contains("Tập") || s.Contains(WebUtility.HtmlDecode("Tâ&#803;p")))
                    {
                        if (s.Contains(":"))
                            strEpi = s.Substring(s.IndexOf("Tập"), s.IndexOf(":", s.IndexOf("Tập")) - (s.IndexOf("Tập")));
                        else
                            strEpi = s.Substring(s.IndexOf("Tập"), s.IndexOf("<", s.IndexOf("Tập")) - (s.IndexOf("Tập")));
                        iEpi = int.Parse(strEpi.Substring(strEpi.IndexOf(" ") + 1));
                    }
                    if (char.IsNumber(s[0]) || s[0].Equals('C'))
                    {
                        Chapter chapter = GetChapterFromHtmlLine(s);
                        if (chapter != null)
                        {
                            using (var context = new ChapterManager())
                            {
                                if (context.Chapters.Where(b => b.ChapterId == chapter.ChapterId).Count() == 0)
                                {
                                    if (context.Episodes.Where(b => b.EpisodeId == iEpi).Count() == 0)
                                    {
                                        var newEpisode = new Episode()
                                        {
                                            EpisodeId = iEpi,
                                            Name = string.Format("Tập {0}", iEpi),
                                            Image = ""
                                        };
                                        context.Episodes.Add(newEpisode);
                                        context.SaveChanges();
                                    }
                                    context.Chapters.Add(chapter);
                                    if (context.EpisodeDetails.Where(b => b.ChapterId == chapter.ChapterId && b.EpisodeId == iEpi).Count() == 0)
                                    {
                                        context.EpisodeDetails.Add(new EpisodeDetail() { EpisodeId = iEpi, ChapterId = chapter.ChapterId });
                                    }
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
