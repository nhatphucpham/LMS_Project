using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using LMS_Project.Data;

namespace LMS_Project.Migrations
{
    [DbContext(typeof(ChapterManager))]
    [Migration("20170422103712_LMSMigration")]
    partial class LMSMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("LMS_Project.Data.Chapter", b =>
                {
                    b.Property<int>("ChapterId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<string>("Name");

                    b.Property<int>("NumberInEpisode");

                    b.Property<string>("WebAddress");

                    b.HasKey("ChapterId");

                    b.ToTable("Chapters");
                });

            modelBuilder.Entity("LMS_Project.Data.Episode", b =>
                {
                    b.Property<int>("EpisodeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Image");

                    b.Property<string>("Name");

                    b.HasKey("EpisodeId");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("LMS_Project.Data.EpisodeDetail", b =>
                {
                    b.Property<int>("ChapterId");

                    b.Property<int>("EpisodeId");

                    b.HasKey("ChapterId", "EpisodeId");

                    b.ToTable("EpisodeDetails");
                });
        }
    }
}
