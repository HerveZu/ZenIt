﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebApi.Common.Infrastructure;

#nullable disable

namespace WebApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.BoulderingRoutes.BoulderingRoute", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("GymId")
                        .HasColumnType("uuid");

                    b.Property<long>("Index")
                        .HasColumnType("bigint");

                    b.Property<byte[]>("MaskedPicture")
                        .HasColumnType("bytea");

                    b.Property<byte[]>("OriginalPicture")
                        .HasColumnType("bytea");

                    b.HasKey("Id");

                    b.HasIndex("GymId", "Code")
                        .IsUnique();

                    b.HasIndex("GymId", "Index")
                        .IsUnique();

                    b.ToTable("BoulderingRoute");
                });

            modelBuilder.Entity("Domain.Gyms.Gym", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Gym");
                });

            modelBuilder.Entity("Domain.BoulderingRoutes.BoulderingRoute", b =>
                {
                    b.OwnsMany("Domain.BoulderingRoutes.HoldDetection", "DetectedHolds", b1 =>
                        {
                            b1.Property<Guid>("BoulderingRouteId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<int>("X")
                                .HasColumnType("integer");

                            b1.Property<int>("Y")
                                .HasColumnType("integer");

                            b1.HasKey("BoulderingRouteId", "Id");

                            b1.ToTable("HoldDetection");

                            b1.WithOwner()
                                .HasForeignKey("BoulderingRouteId");
                        });

                    b.Navigation("DetectedHolds");
                });
#pragma warning restore 612, 618
        }
    }
}
