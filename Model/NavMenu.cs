﻿using LMS_Project.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS_Project.Model
{
    public class NavMenu
    {
        public string BackgroundColor { get; set; }
        public List<NavItem> MenuItems { get; set; }
        public NavMenu()
        {
            BackgroundColor = "#FF2B2B2B";
            MenuItems = new List<NavItem>() {
                new NavItem(0, "\xE80F","Trang Chủ", typeof(HomePage)),
                new NavItem(1, "\xE8F1","Tất Cả", typeof(NovelPage)),
                new NavItem(2, "\xE81C", "Gần Đây", typeof(ViewNovelPage)),
                new NavItem(3, "\xE115", "Cài đặt", typeof(Setting))
            };
        }
        public int? GetIndex(Type Page)
        {
            NavItem item = null;
            MenuItems.ForEach(o => { if (o.Page == Page) item = o; });
            if (item != null)
                return item.Index;
            else
                return null;
        }
    }
}
