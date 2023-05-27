﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Entities.DatabaseContexts;

#nullable disable

namespace Shared.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20230527181214_Initial migration")]
    partial class Initialmigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("Shared.Entities.App", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<long?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("INTEGER")
                        .HasColumnName("created_at");

                    b.Property<long?>("DeletedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("deleted_at");

                    b.Property<Guid?>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("key");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<long?>("UpdatedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_app");

                    b.ToTable("app", (string)null);
                });

            modelBuilder.Entity("Shared.Entities.Language", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<long?>("AppId")
                        .IsRequired()
                        .HasColumnType("INTEGER")
                        .HasColumnName("app_id");

                    b.Property<long?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("INTEGER")
                        .HasColumnName("created_at");

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT")
                        .HasColumnName("culture");

                    b.Property<long?>("DeletedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<long?>("UpdatedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_language");

                    b.HasIndex("AppId")
                        .HasDatabaseName("ix_language_app_id");

                    b.ToTable("language", (string)null);
                });

            modelBuilder.Entity("Shared.Entities.TranslationKey", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<long?>("AppId")
                        .IsRequired()
                        .HasColumnType("INTEGER")
                        .HasColumnName("app_id");

                    b.Property<long?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("INTEGER")
                        .HasColumnName("created_at");

                    b.Property<long?>("DeletedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("key");

                    b.Property<long?>("UpdatedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_translation_key");

                    b.HasIndex("AppId")
                        .HasDatabaseName("ix_translation_key_app_id");

                    b.ToTable("translation_key", (string)null);
                });

            modelBuilder.Entity("Shared.Entities.TranslationValue", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<long?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("INTEGER")
                        .HasColumnName("created_at");

                    b.Property<long?>("DeletedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("deleted_at");

                    b.Property<long?>("LanguageId")
                        .IsRequired()
                        .HasColumnType("INTEGER")
                        .HasColumnName("language_id");

                    b.Property<long?>("UpdatedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("updated_at");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("value");

                    b.HasKey("Id")
                        .HasName("pk_translation_value");

                    b.HasIndex("LanguageId")
                        .HasDatabaseName("ix_translation_value_language_id");

                    b.ToTable("translation_value", (string)null);
                });

            modelBuilder.Entity("Shared.Entities.User", b =>
                {
                    b.Property<long?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<long?>("CreatedAt")
                        .IsRequired()
                        .HasColumnType("INTEGER")
                        .HasColumnName("created_at");

                    b.Property<long?>("DeletedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("deleted_at");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("full_name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("TEXT")
                        .HasColumnName("password")
                        .IsFixedLength();

                    b.Property<long?>("UpdatedAt")
                        .HasColumnType("INTEGER")
                        .HasColumnName("updated_at");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_user");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("Shared.Entities.Language", b =>
                {
                    b.HasOne("Shared.Entities.App", "App")
                        .WithMany()
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_language_app_app_id");

                    b.Navigation("App");
                });

            modelBuilder.Entity("Shared.Entities.TranslationKey", b =>
                {
                    b.HasOne("Shared.Entities.App", "App")
                        .WithMany()
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_translation_key_app_app_id");

                    b.Navigation("App");
                });

            modelBuilder.Entity("Shared.Entities.TranslationValue", b =>
                {
                    b.HasOne("Shared.Entities.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_translation_value_language_language_id");

                    b.Navigation("Language");
                });
#pragma warning restore 612, 618
        }
    }
}
