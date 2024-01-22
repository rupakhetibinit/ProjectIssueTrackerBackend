using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIssueTracker.Models;

namespace ProjectIssueTracker.Data
{
    public class ApiDBContext : DbContext
    {
        public ApiDBContext(DbContextOptions<ApiDBContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ProjectCollaborator> ProjectCollaborators{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u=>u.Comments)
                .WithOne(u=>u.Commenter)
                .HasForeignKey(c=>c.CommenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Issue>()
                .HasMany(i=>i.Comments)
                .WithOne(c=>c.Issue)
                .HasForeignKey(i=>i.IssueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Issue>()
                .HasOne(i=>i.Creator)
                .WithMany(c=>c.CreatedIssues)
                .HasForeignKey(i=>i.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(p=>p.Collaborators)
                .WithOne(pc=>pc.Project)
                .HasForeignKey(pc=>pc.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectCollaborator>()
                .HasKey(pc => new { pc.UserId, pc.ProjectId });
                

            base.OnModelCreating(modelBuilder);
        }

    }

}
