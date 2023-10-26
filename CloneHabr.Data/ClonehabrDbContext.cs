﻿using CloneHabr.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace CloneHabr.Data
{
    public class ClonehabrDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Violation> Violations { get; set; }
        public ClonehabrDbContext(DbContextOptions options) : base(options)
        {

        }
    }

}
