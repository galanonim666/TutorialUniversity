﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using TutorialUniversity.Data;
using TutorialUniversity.Models;

namespace TutorialUniversity.Migrations
{
    [DbContext(typeof(SchoolContext))]
    [Migration("20171108184452_MaxLengthOnNames")]
    partial class MaxLengthOnNames
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("TutorialUniversity.Models.Course", b =>
                {
                    b.Property<int>("CourseID");

                    b.Property<int>("Credits");

                    b.Property<string>("Title");

                    b.HasKey("CourseID");

                    b.ToTable("Course");
                });

            modelBuilder.Entity("TutorialUniversity.Models.Enrollment", b =>
                {
                    b.Property<int>("EnrollmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CourseId");

                    b.Property<int?>("Grade");

                    b.Property<int>("StudentId");

                    b.HasKey("EnrollmentId");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("Enrollment");
                });

            modelBuilder.Entity("TutorialUniversity.Models.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Country");

                    b.Property<DateTime>("EnrollmentDate");

                    b.Property<string>("FirstMidName")
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Student");
                });

            modelBuilder.Entity("TutorialUniversity.Models.Enrollment", b =>
                {
                    b.HasOne("TutorialUniversity.Models.Course", "Course")
                        .WithMany("Enrollments")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TutorialUniversity.Models.Student", "Student")
                        .WithMany("Enrollments")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
