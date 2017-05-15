using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS_Project.Data;

namespace LMS_Project.Model
{
    public class Valvrareteam : SourceAnalysis
    {
        public Valvrareteam() : base()
        {
            novels = new List<Novel>();
            webDetails = new List<WebDetail>();
            NavLinks = new Dictionary<int, string>();
            novelDetails = new List<NovelDetail>();
            episodes = new List<Episode>();
            chapters = new List<Chapter>();
            episodeDetails = new List<EpisodeDetail>();
        }

        private string MainAdress = @"http://valvrareteam.com/";

        public override void LoadNav()
        {
            bool isNav = false;
            Task.Run(async () => {
                await CheckConnection();
                await LoadHTLM(Sourse.Address);
            }).Wait();
            SourceAnalysis.m_HTML.ForEach(m =>
            {
                if (m.Contains("header"))
                    StringHtml = m;
            });
            SourceAnalysis.m_HTML.Clear();

            int begin = 0;
            int end = 0;
            StringHtml = StringHtml.Remove(0, StringHtml.IndexOf("/header") + 8);
            for (int i = 0; i < StringHtml.Length; i++)
            {

                if (StringHtml[i] == '<')
                {
                    begin = i;
                }

                if (StringHtml[i] == '>')
                {
                    end = i;
                    if (end > begin)
                    {
                        StringHtml = StringHtml.Insert(i + 1, "\n\n");
                        SourceAnalysis.m_HTML.Add(StringHtml.Substring(begin, end - begin + 1));
                    }
                }
            }
            foreach (var item in SourceAnalysis.m_HTML)
            {
                if (item.Contains("nav-links"))
                {
                    isNav = true;
                }

                if (item.Contains("/div"))
                {
                    isNav = false;
                }

                if (isNav)
                {
                    int index = 0;
                    int length = 0;
                    int key = 0;
                    string value = Sourse.Address;
                    if (item.Contains("prev page-numbers"))
                        continue;
                    if (item.Contains("<a") || item.Contains("<span"))
                    {
                        index = item.IndexOf("href") + 6;
                        length = item.Substring(index).IndexOf("\"");
                        if (length > 0)
                            value = item.Substring(index, length);
                        index = value.IndexOf("page") + 5;

                        key = NavLinks.Count + 1;

                        if (index < 5)
                        {
                            SourceAnalysis.CurrentPages = key;
                        }

                        if (!NavLinks.Keys.Contains(key) && !NavLinks.Values.Contains(value))
                        {
                            SourceAnalysis.NavLinks.Add(key, value);
                        }
                    }
                }

            }
        }

        public override void NextPage()
        {
            SourceAnalysis.CurrentPages++;
            Task.Run(async () => await LoadHTLM(SourceAnalysis.NavLinks[CurrentPages])).Wait();
            LoadNovel();

        }

        public override void LoadEpisode(int NovelId)
        {
            using (var context = new DataManager())
            {
                int BeginEpisodeCount = 0;
                novels = context.Novels.ToList();
                chapters = context.Chapters.ToList();
                episodes = context.Episodes.ToList();
                novelDetails = context.NovelDetails.ToList();
                episodeDetails = context.EpisodeDetails.ToList();

                BeginEpisodeCount = episodes.Count;
                var NewChapterList = new List<Chapter>();
                var NewEpisodeList = new List<Episode>();
                var NewEpisodeDetailList = new List<EpisodeDetail>();
                var NewNovelDetailList = new List<NovelDetail>();

                var novel = context.Novels.Single(n => n.NovelId == NovelId);

                Task.Run(async ()=>await LoadHTLM(novel.Address)).Wait();
                SourceAnalysis.m_HTML.ForEach(m =>
                {
                    if (m.Contains("header"))
                        StringHtml = m;
                });
                SourceAnalysis.m_HTML.Clear();

                int index = 0;
                int index2 = 0;
                int length = 0;
                int length2 = 0;

                for (int i = 0; i < StringHtml.Length; i++)
                {
                    if (StringHtml[i] == '<')
                    {
                        index = i;
                        if (index2 > 0)
                            length2 = i - index2;
                        string item = StringHtml.Substring(index2, length2);

                        if (item.Trim() != "")
                            SourceAnalysis.m_HTML.Add(item);
                    }

                    if (StringHtml[i] == '>')
                    {
                        index2 = i + 1;
                        length = i - index + 1;
                        StringHtml = StringHtml.Insert(i + 1, "\n\n");
                        string item = StringHtml.Substring(index, length);
                        if (item.Trim() != "")
                            SourceAnalysis.m_HTML.Add(item);
                    }
                }

                index = -1;
                length = -1;
                int ChapterInEpisode = 0;
                bool SummanyWrite = false;
                bool chapterWrite = false;

                string TitleLine = "";
                string AddressLine = "";
                string pre_Item = null;
                Episode episode = new Episode();

                bool IsEpisode = false;
                string TypeChapter = "Light Novel";
                bool isGetType = false;

                foreach (var item in SourceAnalysis.m_HTML)
                {
                    if (item.ToLower().Contains("thông báo"))
                        isGetType = true;


                    if (item.ToLower().Remove(0, 2).Trim() == "web novel")
                    {
                        TypeChapter = "Web Novel";
                    }

                    if (item.ToLower().Remove(0, 2).Trim() == "light novel")
                    {
                        TypeChapter = "Light Novel";
                    }

                    if (item.ToLower().Remove(0, 2) == " manga")
                    {
                        TypeChapter = "Manga";
                    }

                    string nexxt_item = SourceAnalysis.m_HTML[SourceAnalysis.m_HTML.IndexOf(item) + 1];
                    if (item.ToLower().Contains("nội dung"))
                    {
                        SummanyWrite = true;
                        pre_Item = item;
                        continue;
                    }
                    if (item.ToLower().Contains("iii"))
                    {
                        SummanyWrite = false;
                    }
                    if (item.Contains("<strong"))
                        IsEpisode = true;
                    if (item.Contains("/strong"))
                        IsEpisode = false;

                    if (item.ToLower().Contains("danh sách") || item.Contains("Web Novel") || (item.Contains("/ Tác Phẩm")&& pre_Item.Contains("span")))
                    {
                        if (IsEpisode)
                        {
                            chapterWrite = true;
                            pre_Item = item;
                            IsEpisode = false;
                            continue;
                        }
                    }
                    if (SummanyWrite)
                    {
                        if (!item.Contains("["))
                        {
                            if (item.ToLower().Contains("tác giả"))
                            {
                                novel.Author = item.Substring(item.IndexOf(":") + 1);
                            }
                            if (!item.ToLower().Contains("THÔNG BÁO".ToLower()))
                            {

                                if (!item.Contains("<"))
                                {
                                    if (pre_Item.Contains("<"))
                                        novel.Summany += string.Format("\n\n{0}", item.Trim());
                                    else
                                        novel.Summany += item;
                                }
                            }
                        }

                    }
                    if (chapterWrite)
                    {

                        if (item.Contains("<strong"))
                        {
                            IsEpisode = true;
                        }

                        if (!item.Contains("<"))
                        {

                            if (item.ToLower().Contains("tập") || item.Contains("Quyển") || item.Contains("Web Novel") || item.ToLower().Contains("arc"))
                            {
                                if (IsEpisode || pre_Item.Contains("<span style=\"font-size: 24pt; color: #ff0000;\">"))
                                {
                                    episode = new Episode()
                                    {
                                        EpisodeId = episodes.Count + 1,
                                        Name = string.Format("{0}\n{1}", TypeChapter, item.Contains("\n\n") ? item.Remove(0, 2) : item)
                                    };
                                    if (context.Episodes.Where(c => c.Name == episode.Name).Count() == 0)
                                    {

                                        NewEpisodeList.Add(episode);
                                        episodes.Add(episode);
                                        var detail = new NovelDetail() { EpisodeId = episode.EpisodeId, NovelId = novel.NovelId };
                                        novelDetails.Add(detail);
                                        NewNovelDetailList.Add(detail);
                                    }

                                }
                            }
                        

                            if ((pre_Item != null && pre_Item.Contains("href")) || AddressLine  != "")
                            {
                                TitleLine = item;
                            }
                        }

                        if (item.Contains("/strong"))
                        {
                            IsEpisode = false;
                        }

                        if (item.Contains("href"))
                        {
                            AddressLine = item;
                        }

                        if (AddressLine != "")
                        {
                            var chapter = GetChapterFromHtmlLine(AddressLine, TitleLine);
                            if (chapter != null)
                            {
                                if (context.Chapters.Where(c => c.Name == chapter.Name).Count() == 0)
                                {
                                    ChapterInEpisode = episodeDetails.Where(w => w.EpisodeId == episode.EpisodeId).Count() + 1;
                                    if (episode.Name != null)
                                    {
                                        var episodeDetail = new EpisodeDetail()
                                        {
                                            EpisodeId = episode.EpisodeId,
                                            ChapterId = chapter.ChapterId
                                        };

                                        NewEpisodeDetailList.Add(episodeDetail);
                                        episodeDetails.Add(episodeDetail);
                                    }
                                    chapter.NumberInEpisode = ChapterInEpisode;
                                    chapters.Add(chapter);
                                    NewChapterList.Add(chapter);
                                }
                                AddressLine = "";
                                TitleLine = "";
                            }
                        }

                    }

                    pre_Item = item;
                }
                if (episodes.Count == BeginEpisodeCount)
                {
                    var newEpisode = new Episode() { EpisodeId = episodes.Count + 1, Name = novel.Title };
                    episodes.Add(newEpisode);
                    NewEpisodeList.Add(newEpisode);
                    var newNoveDetail = new NovelDetail() { EpisodeId = episodes[episodes.Count - 1].EpisodeId, NovelId = novel.NovelId };
                    novelDetails.Add(newNoveDetail);
                    NewNovelDetailList.Add(newNoveDetail);
                    NewChapterList.ForEach(c =>
                    {
                        var detail = new EpisodeDetail()
                        {
                            ChapterId = c.ChapterId,
                            EpisodeId = episodes[episodes.Count - 1].EpisodeId
                        };
                        episodeDetails.Add(detail);
                        NewEpisodeDetailList.Add(detail);
                    });
                }

                if (NewEpisodeList.Count > 0)
                {
                    context.NovelDetails.AddRange(NewNovelDetailList);
                    context.Episodes.AddRange(NewEpisodeList);
                    context.EpisodeDetails.AddRange(NewEpisodeDetailList);
                    context.Chapters.AddRange(NewChapterList);
                    context.SaveChanges();
                }

                episodes = context.Episodes.ToList();
                chapters = context.Chapters.ToList();
                episodeDetails = context.EpisodeDetails.ToList();
                novelDetails = context.NovelDetails.ToList();
            }
        }


        public override async Task<List<string>> LoadContent(int id)
        {
            using (var context = new DataManager())
            {
                var chapter = context.Chapters.Single(s => s.ChapterId == id);
                await CheckConnection();
                await LoadHTLM(chapter.WebAddress);
                SourceAnalysis.m_HTML.ForEach(m =>
                {
                    if (m.Contains("header"))
                        StringHtml = m;
                });
                SourceAnalysis.m_HTML.Clear();
                int index = 0;
                int index2 = 0;
                int length = 0;
                int length2 = 0;

                for (int i = 0; i < StringHtml.Length; i++)
                {
                    if (StringHtml[i] == '<')
                    {
                        index = i;
                        if (index2 > 0)
                            length2 = i - index2;
                        string item = StringHtml.Substring(index2, length2);

                        if (item.Trim() != "")
                            SourceAnalysis.m_HTML.Add(item);
                    }

                    if (StringHtml[i] == '>')
                    {
                        index2 = i + 1;
                        length = i - index + 1;
                        string item = StringHtml.Substring(index, length);
                        if (item.Trim() != "")
                            SourceAnalysis.m_HTML.Add(item);
                    }
                }
                StringHtml = "";
                bool isContent = false;
                string pre_item = "";
                for(int i = 0; i < SourceAnalysis.m_HTML.Count; i++)
                {
                    if (SourceAnalysis.m_HTML[0].Contains("/"))
                    {
                        SourceAnalysis.m_HTML.RemoveAt(0);
                        i--;

                    }
                    if (i >= 0)
                    {
                        var item = SourceAnalysis.m_HTML[i];
                        if (item.Contains("THÔNG BÁO") && pre_item.Contains("span"))
                        {
                            isContent = true;
                            pre_item = item;
                            SourceAnalysis.m_HTML.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (item.Contains("function"))
                        {
                            isContent = false;
                        }
                        if (isContent)
                        {
                            if (!item.Contains("scrip") && !item.Contains("google"))
                                StringHtml += item;
                        }
                        else
                        {
                            SourceAnalysis.m_HTML.RemoveAt(i);
                            i--;
                        }
                        pre_item = item;
                    }
                }

            }

            StringHtml = StringHtml.Insert(0, "<html onselectstart = \"return false;\" style = \"-ms-user-select: none;\" >");
            StringHtml = StringHtml.Insert(StringHtml.Count(), "</html>");

            return SourceAnalysis.m_HTML;
        }

        protected override Chapter GetChapterFromHtmlLine(string AddressLine, string TitleLine)
        {
            if (TitleLine.Contains("\n\n"))
                TitleLine = TitleLine.Remove(TitleLine.IndexOf("\n\n"), 2);
            var chapter = new Chapter()
            {

                ChapterId = chapters.Count + 1,
                WebId = Sourse.WebId,
            };
            if (TitleLine != "")
            {
                chapter.Name = TitleLine;
                int index = AddressLine.IndexOf("href") + 6;
                int length = AddressLine.Substring(index).IndexOf("\"");
                if (index >= 0 && length > 0)
                {
                    chapter.WebAddress = AddressLine.Substring(index, length);
                }
            }
            else
            {
                int index = AddressLine.IndexOf("href") + 6;
                int length = AddressLine.Substring(index).IndexOf("\"");
                if (index >= 0 && length > 0)
                {
                    chapter.WebAddress = AddressLine.Substring(index, length);
                }

                string name;
                index = AddressLine.IndexOf(">") + 1;
                length = AddressLine.Substring(index).IndexOf("<");
                if (length == -1)
                    name = AddressLine.Substring(index);
                else
                    name = AddressLine.Substring(index, length);
                if (name != "")
                    chapter.Name = name;
                else
                    return null;
            }
            return chapter;
        }

        public override void LoadNovel()
        {
            SourceAnalysis.m_HTML.ForEach(m =>
            {
                if (m.Contains("header"))
                    StringHtml = m;
            });
            SourceAnalysis.m_HTML.Clear();

            using (var context = new DataManager())
            {
                novels = context.Novels.ToList();
                webDetails = context.WebDetails.ToList();
                var NewNovelList = new List<Novel>();
                var NewWebDetailList = new List<WebDetail>();
                int begin = 0;
                int end = 0;
                StringHtml = StringHtml.Remove(0, StringHtml.IndexOf("/header") + 8);
                for (int i = 0; i < StringHtml.Length; i++)
                {

                    if (StringHtml[i] == '<')
                    {
                        begin = i;
                    }

                    if (StringHtml[i] == '>')
                    {
                        end = i;
                        if (end > begin)
                        {
                            StringHtml = StringHtml.Insert(i + 1, "\n\n");
                            SourceAnalysis.m_HTML.Add(StringHtml.Substring(begin, end - begin + 1));
                        }
                    }
                }

                bool isNovel = false;

                List<string> strList = new List<string>();

                var novel = new Novel();
                foreach (var item in SourceAnalysis.m_HTML)
                {
                    if (item.Contains("<h2"))
                        isNovel = true;
                    if (item.Contains("</h2"))
                        isNovel = false;

                    if (isNovel)
                    {
                        if (item.Contains("<a"))
                        {
                            int index = item.IndexOf("href") + 6;
                            int length = item.Substring(index).IndexOf("\"");
                            string address = item.Substring(index, length);
                            index = item.IndexOf("title") + 7;
                            length = item.Substring(index).IndexOf("\"");
                            string title = item.Substring(index, length);
                            if (title.Count() > 20)
                            {
                                title.Insert(20, "\r\n");
                            }
                            novel.NovelId = novels.Count + 1;
                            novel.Title = title;
                            novel.Address = address;
                            if (novels.Where(w=>w.Title == novel.Title).Count() == 0)
                            {
                                novels.Add(novel);
                                NewNovelList.Add(novel);

                                var detail = new WebDetail() { WebId = context.WebSourses.Single(s=>s.Address == MainAdress).WebId, NovelId = novel.NovelId };
                                if (!webDetails.Contains(detail))
                                {
                                    webDetails.Add(detail);
                                    NewWebDetailList.Add(detail);
                                }
                                novel = new Novel();
                            }
                        }
                    }

                    if (item.Contains("img"))
                    {
                        int index = item.IndexOf("src") + 5;
                        int length = item.Substring(index).IndexOf("\"");
                        novel.ImageUrl = item.Substring(index, length);
                        if (novel.ImageUrl.Contains("/no-avatar.png"))
                        {
                            novel.ImageUrl = "ms-appx:///Assets/noimagefound.jpg";
                        }
                    }
                }

                context.Novels.AddRange(NewNovelList);
                context.WebDetails.AddRange(NewWebDetailList);
                context.SaveChanges();
            }
        }

        public override void PreviousPage()
        {
            SourceAnalysis.CurrentPages--;
            Sourse.Address = SourceAnalysis.NavLinks[CurrentPages];

        }
    }
}
