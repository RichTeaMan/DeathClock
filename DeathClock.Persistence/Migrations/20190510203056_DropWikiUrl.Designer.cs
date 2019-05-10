﻿// <auto-generated />
using System;
using DeathClock.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeathClock.Persistence.Migrations
{
    [DbContext(typeof(DeathClockContext))]
    [Migration("20190510203056_DropWikiUrl")]
    partial class DropWikiUrl
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DeathClock.Persistence.BasePerson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("DataSet")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<DateTime?>("DeathDate");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("IsDead");

                    b.Property<string>("KnownFor")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("RecordedDate");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime>("UpdateDate");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("BasePerson");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BasePerson");
                });

            modelBuilder.Entity("DeathClock.Persistence.TmdbPerson", b =>
                {
                    b.HasBaseType("DeathClock.Persistence.BasePerson");

                    b.Property<double>("Popularity");

                    b.Property<int>("TmdbId");

                    b.HasDiscriminator().HasValue("TmdbPerson");
                });

            modelBuilder.Entity("DeathClock.Persistence.WikipediaPerson", b =>
                {
                    b.HasBaseType("DeathClock.Persistence.BasePerson");

                    b.Property<int>("DeathWordCount");

                    b.Property<bool>("IsStub");

                    b.Property<int>("WordCount");

                    b.HasDiscriminator().HasValue("WikipediaPerson");
                });
#pragma warning restore 612, 618
        }
    }
}
