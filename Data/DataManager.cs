﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace LMS_Project.Data
{
    public class DataManager : DbContext
    {
        public DbSet<Novel> Novels { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<WebSource> WebSourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=db_lms.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebSource>().HasKey(t => new { t.WebId });
            modelBuilder.Entity<Novel>().HasKey(t => new { t.NovelId });
            modelBuilder.Entity<Episode>().HasKey(t => new { t.EpisodeId });
            modelBuilder.Entity<Chapter>().HasKey(t => new { t.ChapterId });
        }

    }

}
