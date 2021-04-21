﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServerSideData;

namespace ServerSideData.Migrations
{
    [DbContext(typeof(FinanceDbContext))]
    [Migration("20210421113552_Removed_UCP")]
    partial class Removed_UCP
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ServerSideData.Models.Corporation", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("cvrNummer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Corporations");
                });

            modelBuilder.Entity("ServerSideData.Models.FinanceEntry", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("KontiID")
                        .HasColumnType("int");

                    b.Property<DateTime>("addDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("byWho")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("newSaldoKonti")
                        .HasColumnType("float");

                    b.Property<double>("newSaldoMain")
                        .HasColumnType("float");

                    b.Property<DateTime>("payDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("value")
                        .HasColumnType("float");

                    b.HasKey("ID");

                    b.ToTable("FinanceEntries");
                });

            modelBuilder.Entity("ServerSideData.Models.Inventory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CorporationID")
                        .HasColumnType("int");

                    b.Property<string>("itemName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("value")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("ServerSideData.Models.Konti", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CorporationID")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Kontis");
                });

            modelBuilder.Entity("ServerSideData.Models.Member", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CorporationID")
                        .HasColumnType("int");

                    b.Property<string>("firstname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lastname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("mail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("ServerSideData.Models.Permissions", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AddCorporation")
                        .HasColumnType("bit");

                    b.Property<bool>("AddFinance")
                        .HasColumnType("bit");

                    b.Property<bool>("AddInventory")
                        .HasColumnType("bit");

                    b.Property<bool>("AddMember")
                        .HasColumnType("bit");

                    b.Property<bool>("AddUser")
                        .HasColumnType("bit");

                    b.Property<bool>("Admin")
                        .HasColumnType("bit");

                    b.Property<int>("CorporationID")
                        .HasColumnType("int");

                    b.Property<bool>("DeleteInventory")
                        .HasColumnType("bit");

                    b.Property<bool>("DeleteMember")
                        .HasColumnType("bit");

                    b.Property<bool>("DeleteUser")
                        .HasColumnType("bit");

                    b.Property<bool>("EditInventory")
                        .HasColumnType("bit");

                    b.Property<bool>("EditMember")
                        .HasColumnType("bit");

                    b.Property<bool>("EditUser")
                        .HasColumnType("bit");

                    b.Property<bool>("LimitedViewFinance")
                        .HasColumnType("bit");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<bool>("ViewFinance")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("ServerSideData.Models.RepFinanceEntry", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("KontiID")
                        .HasColumnType("int");

                    b.Property<string>("byWho")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("firstExecDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("intervalType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("intervalValue")
                        .HasColumnType("int");

                    b.Property<DateTime>("nextExecDate")
                        .HasColumnType("datetime2");

                    b.Property<double>("value")
                        .HasColumnType("float");

                    b.HasKey("ID");

                    b.ToTable("RepFinanceEntries");
                });

            modelBuilder.Entity("ServerSideData.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("firstname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("hashPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("lastSeen")
                        .HasColumnType("datetime2");

                    b.Property<string>("lastname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("mail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
