﻿using Microsoft.EntityFrameworkCore;

namespace RAZOR_PAGE9_ENTITY.Models
{
    public class MyBlogContext : DbContext
    {
        public DbSet<Article> articles { set; get; }
        public MyBlogContext(DbContextOptions<MyBlogContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
