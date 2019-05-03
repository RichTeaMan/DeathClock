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
    [Migration("20190503204728_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DeathClock.Persistence.TmdbPerson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("DataSet")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<DateTime?>("DeathDate");

                    b.Property<string>("ImdbUrl")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<bool>("IsDead");

                    b.Property<string>("KnownFor")
                        .IsRequired();

                    b.Property<double>("Popularity");

                    b.Property<DateTime>("RecordedDate");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int>("TmdbId");

                    b.Property<DateTime>("UpdateDate");

                    b.HasKey("Id");

                    b.ToTable("TmdbPersons");
                });
#pragma warning restore 612, 618
        }
    }
}
