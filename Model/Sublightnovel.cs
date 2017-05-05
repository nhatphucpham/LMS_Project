using System.Collections.Generic;
using LMS_Project.Data;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Linq;
using System.Net.Http;
using System;

namespace LMS_Project.Model
{
    public class Sublightnovel : SourceAnalysis
    {
        private Novel thisNovel;

        public Sublightnovel():base()
        {
            if (episodes == null)
            {
                novelDetails = new List<NovelDetail>();
                thisNovel = new Novel();
                episodes = new List<Episode>();
                chapters = new List<Chapter>();
                episodeDetails = new List<EpisodeDetail>();
            }
        }

        public override async Task<List<string>> SetContent(int id)
        {
            using (var context = new DataManager())
            {
                var chapter = context.Chapters.Single(b => b.ChapterId == id);
                List<string> content_HTML = new List<string>();

                var httpClient = new HttpClient();
                try
                {
                    var result = await httpClient.GetStreamAsync(chapter.WebAddress);
                    StreamReader read = new StreamReader(result);
                    bool Nope1 = false;
                    bool Nope2 = false;
                    StringHtml = "";
                    while (read.Peek() >= 0)
                    {
                        string str = await read.ReadLineAsync();
                        if (str.Contains("</style>"))
                            Nope1 = true;
                        if (str.Contains("reaction-buttons"))
                            Nope1 = false;
                        if (Nope1)
                        {
                            if (str.Contains("h3"))
                                Nope2 = true;
                            if (Nope2)
                            {
                                StringHtml += str + "\n\r";
                                content_HTML.Add(WebUtility.HtmlDecode(str));
                            }
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
                            Nope1 = true;
                            continue;
                        }
                        if (item.Contains("star-ratings"))
                            Nope1 = false;
                        if (Nope1)
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

        protected override Chapter GetChapterFromHtmlLine(string Line, string TitleLine = "")
        {
            string[] spl = System.Text.RegularExpressions.Regex.Split(Line, @"<\W?\w{1,4}\W?\w?\d?(\s\w{4}\W""http://www.sublightnovel.com/\d{4,}/\d{2,}([\w-\W]*).html""( target=""_blank"")?)?\s?\W?>");
            string chap = "", title = "", address = "", ep = "";
            foreach (string s in spl)
            {
                if (s.Contains("Tập"))
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
            int iEp = (ep != "") ? int.Parse(ep.Substring(ep.Length - 1)) : 0;
            if (chapters.Where(b => b.Name == title).Count() == 0)
            {
                Chapter chapter = new Chapter()
                {
                    ChapterId = chapters.Count() + 1,
                    WebId = Sourse.WebId,
                    Name = title,
                    Content = null,
                    WebAddress = address
                };
                if (iEp > 0)
                {
                    if (episodeDetails.Where(b => b.ChapterId == chapter.ChapterId && b.EpisodeId == iEp).Count() == 0)
                    {
                        episodeDetails.Add(new EpisodeDetail() { EpisodeId = iEp, ChapterId = chapter.ChapterId });
                    }
                }
                
                return chapter;
            }

            return null;
        }

        ///Find and detect and set the chapters and the episodes
        public override void LoadEpisode(int NovelId)
        {
            using (var context = new DataManager())
            {
                chapters = context.Chapters.ToList();
                episodes = context.Episodes.ToList();
                episodeDetails = context.EpisodeDetails.ToList();
                var novel = context.Novels.Single(s => s.NovelId == NovelId);
                novelDetails = context.NovelDetails.ToList();
                List<Chapter> NewChapterList = new List<Chapter>();
                List<Episode> NewEpisodeList = new List<Episode>();
                List<EpisodeDetail> NewEpisodeDetailList = new List<EpisodeDetail>();
                List<NovelDetail> NewNovelDetailList = new List<NovelDetail>();

                string strEpi = "";
                int iEpi = -1;
                bool accept = false;
                foreach (string s in m_HTML)
                {
                    // Toàn bộ nôi dung có ở tab <h4>
                    if (!accept && s.Contains("<h4>")) accept = true;
                    if (s.Contains("/h4")) break;
                    if (accept)
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
                            if (chapter != null && chapters.Where(w=>w.WebAddress == chapter.WebAddress).Count() == 0)
                            {
                                if (chapters.Where(w=>w.Name == chapter.Name).Count() == 0)
                                {
                                    var episode = new Episode()
                                    {
                                        EpisodeId = episodes.Count + 1,
                                        Name = string.Format("{0}: Tập {1}", novel.Title, iEpi),
                                        Image = ""
                                    };

                                    if (episodes.Where(w => w.Name == episode.Name).Count() == 0)
                                    {
                                        episodes.Add(episode);
                                        NewEpisodeList.Add(episode);
                                        var noDetail = new NovelDetail() { EpisodeId = episodes[episodes.Count - 1].EpisodeId, NovelId = novel.NovelId };
                                        novelDetails.Add(noDetail);
                                        NewNovelDetailList.Add(noDetail);
                                    }

                                    var epDetail = new EpisodeDetail()
                                    {
                                        EpisodeId = iEpi,
                                        ChapterId = chapter.ChapterId
                                    };

                                    if (episodeDetails.Where(w => w.EpisodeId == episode.EpisodeId && w.ChapterId == epDetail.ChapterId).Count() == 0)
                                    {
                                        episodeDetails.Add(epDetail);
                                        NewEpisodeDetailList.Add(epDetail);
                                    }
                                    chapter.NumberInEpisode = episodeDetails.Where(e => e.EpisodeId == episodes[episodes.Count - 1].EpisodeId).Count();
                                    chapters.Add(chapter);
                                    NewChapterList.Add(chapter);
                                }
                            }
                        }
                    }
                }


                if (NewEpisodeList.Count > 0)
                {
                    context.NovelDetails.AddRange(NewNovelDetailList);
                    context.Episodes.AddRange(NewEpisodeList);
                    context.Chapters.AddRange(NewChapterList);
                    context.EpisodeDetails.AddRange(NewEpisodeDetailList);
                    context.SaveChanges();
                }

                episodes = context.Episodes.ToList();
                chapters = context.Chapters.ToList();
                episodeDetails = context.EpisodeDetails.ToList();
                novelDetails = context.NovelDetails.ToList();
            }
        }

        public override void LoadNovel()
        {
            thisNovel.Address = Sourse.Address;
            thisNovel.NovelId = (new DataManager()).Novels.Count() + 1;
            using (var context = new DataManager())
            {
                bool tilte = false;
                bool sumany = false;
                foreach (string s in m_HTML)
                {
                    if (s.Contains("h1 class='title'"))
                    {
                        tilte = true;
                        continue;
                    }
                    if (tilte)
                    {
                        int index = s.IndexOf(">") + 1;
                        int length = s.Substring(index).IndexOf("<");
                        thisNovel.Title = s.Substring(index, length);
                        tilte = false;
                    }

                    if (s.Contains("img border=\"0\" src"))
                    {
                        int index = s.IndexOf("\"") + 1;
                        int length = s.Substring(index).IndexOf("\"");
                        thisNovel.ImageUrl = s.Substring(index, length);
                    }

                    if (s.Contains(">Tác giả:"))
                    {
                        int index = s.IndexOf(">") + 10;
                        int length = s.Substring(index).IndexOf("<");
                        thisNovel.Author = s.Substring(index, length);
                    }

                    if (s.Contains("text-align: justify;"))
                    {
                        sumany = true;
                        continue;
                    }
                    if (sumany)
                    {
                        if (s.Contains("/span"))
                        {
                            int length = s.IndexOf("<");
                            thisNovel.Summany += s.Substring(0, length);
                            sumany = false;
                        }
                        else
                        {
                            if (s.Contains("<span"))
                            {
                                int index = s.IndexOf(">") + 1;
                                thisNovel.Summany += string.Format("{0} ", s.Substring(index));
                            }
                            else thisNovel.Summany += string.Format("{0} ", s);
                        }
                    }
                }

                context.WebDetails.Add(new WebDetail() { WebId = Sourse.WebId, NovelId = thisNovel.NovelId });
                context.Novels.Add(thisNovel);
                context.SaveChanges();
            }
        }

        public override void LoadNav()
        {
            SourceAnalysis.CurrentPages = 0;
            SourceAnalysis.NavLinks = new Dictionary<int, string>();
        }

        public override void NextPage()
        {
            
        }

        public override void PreviousPage()
        {

        }
    }
}
