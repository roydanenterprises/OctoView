﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using OctoView.Github.Contexts;
using System;

namespace OctoView.Github.Migrations
{
    [DbContext(typeof(GithubCacheContext))]
    [Migration("20180502131114_UserRepositorySelection")]
    partial class UserRepositorySelection
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OctoView.Github.Contexts.Models.GithubRepository", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Owner");

                    b.HasKey("Id");

                    b.HasIndex("Owner", "Name")
                        .IsUnique()
                        .HasFilter("[Owner] IS NOT NULL AND [Name] IS NOT NULL");

                    b.ToTable("Repositories");
                });

            modelBuilder.Entity("OctoView.Github.Contexts.Models.GithubRequest", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450);

                    b.Property<string>("Request");

                    b.HasKey("Key");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("OctoView.Github.Contexts.Models.User", b =>
                {
                    b.Property<string>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450);

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OctoView.Github.Contexts.Models.UserRepository", b =>
                {
                    b.Property<int>("RepositoryId");

                    b.Property<string>("UserId");

                    b.HasKey("RepositoryId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRepository");
                });

            modelBuilder.Entity("OctoView.Github.Contexts.Models.UserRepository", b =>
                {
                    b.HasOne("OctoView.Github.Contexts.Models.GithubRepository", "Repository")
                        .WithMany("UserRepositories")
                        .HasForeignKey("RepositoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("OctoView.Github.Contexts.Models.User", "User")
                        .WithMany("UserRepositories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
