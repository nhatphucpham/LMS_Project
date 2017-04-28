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
    public class Sublightnovel : SourseAnalysis
    {
        public Sublightnovel()
        {
            if(m_HTML == null)
                m_HTML = new List<string>();
            if (episodes == null)
            {
                episodes = new List<Episode>();
                chapters = new List<Chapter>();
                details = new List<EpisodeDetail>();
            }
        }

        public override async Task<List<string>> SetContent(int id)
        {
            using (var context = new SourseManager())
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

        protected override Chapter GetChapterFromHtmlLine(string Line)
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
                    Name = title,
                    Content = null,
                    WebAddress = address
                };
                if (iEp > 0)
                {
                    if (details.Where(b => b.ChapterId == chapter.ChapterId && b.EpisodeId == iEp).Count() == 0)
                    {
                        details.Add(new EpisodeDetail() { EpisodeId = iEp, ChapterId = chapter.ChapterId });
                    }
                }
                return chapter;
            }

            return null;
        }

        ///Find and detect and set the chapters and the episodes
        public override void LoadData()
        {
            using (var context = new SourseManager())
            {
                string strEpi = "";
                int iEpi = -1;
                bool accept = false;
                foreach (string s in m_HTML)
                {
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
                            if (chapter != null)
                            {
                                if (!chapters.Contains(chapter))
                                {
                                    if (episodes.Where(e=>e.EpisodeId == iEpi).Count() == 0)
                                    {
                                        episodes.Add(new Episode()
                                        {
                                            EpisodeId = iEpi,
                                            Name = string.Format("Tập {0}", iEpi),
                                            Image = ""
                                        });
                                    }
                                    chapters.Add(chapter);

                                    if (details.Where(d=>d.ChapterId == chapter.ChapterId && d.EpisodeId == iEpi).Count() == 0)
                                        details.Add(new EpisodeDetail()
                                        {
                                            EpisodeId = iEpi,
                                            ChapterId = chapter.ChapterId
                                        });
                                }
                            }
                        }
                    }
                }

                if (episodes.Count > 0)
                {
                    context.Episodes.AddRange(episodes);
                    context.Chapters.AddRange(chapters);
                    context.EpisodeDetails.AddRange(details);
                    context.SaveChanges();
                }
            }
        }
    }
}
