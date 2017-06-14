using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using LMS_Project.Data;

namespace LMS_Project.Migrations
{
    [DbContext(typeof(DataManager))]
    partial class DataManagerModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("LMS_Project.Data.Chapter", b =>
                {
                    b.Property<int>("ChapterId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EpisodeId");

                    b.Property<string>("Content");

                    b.Property<string>("Name");

                    b.Property<int>("NumberInEpisode");

                    b.Property<string>("WebAddress");

                    b.Property<int>("WebId");

                    b.HasKey("ChapterId");

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("LMS_Project.Data.Episode", b =>
                {
                    b.Property<int>("EpisodeId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("NovelId");

                    b.Property<string>("Name");

                    b.Property<int>("WebId");

                    b.Property<string>("TypeOfNovel");

                    b.HasKey("EpisodeId");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("LMS_Project.Data.Novel", b =>
                {
                    b.Property<int>("NovelId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("WebId");

                    b.Property<string>("Address");

                    b.Property<string>("Author");

                    b.Property<string>("Summany");

                    b.Property<string>("Title");

                    b.HasKey("NovelId");

                    b.ToTable("Novels");
                });

            modelBuilder.Entity("LMS_Project.Data.WebSourse", b =>
                {
                    b.Property<int>("WebId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Name");

                    b.HasKey("WebId");

                    b.ToTable("WebSourses");
                });
        }
    }
}
