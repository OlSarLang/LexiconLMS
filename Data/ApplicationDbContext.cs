﻿using System;
using System.Collections.Generic;
using System.Text;
using LexiconLMS.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Core.ViewModels;

namespace LexiconLMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<SystemUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SystemUser> SystemUsers { get; set; }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<SystemUserCourse> SystemUserCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SystemUserCourse>()
                .HasKey(k => new
                {
                    k.SystemUserId,
                    k.CourseId
                });
        }

        public DbSet<LexiconLMS.Core.ViewModels.SystemUserViewModel> SystemUserViewModel { get; set; }

    }
}
