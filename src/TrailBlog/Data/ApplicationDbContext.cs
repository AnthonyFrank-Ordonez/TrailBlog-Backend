using Microsoft.EntityFrameworkCore;
using TrailBlog.Api.Entities;

namespace TrailBlog.Api.Data
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserCommunity> UserCommunities { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<RecentViewedPost> RecentViewedPosts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.RefreshToken);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.UpdatedAt);
                entity.HasIndex(e => e.IsRevoked);
                entity.HasIndex(e => new { e.IsRevoked, e.CreatedAt });
                entity.HasIndex(e => new { e.IsRevoked, e.UpdatedAt });
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            });


            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.RoleId);
                entity.HasIndex(e => e.AssignedAt);
                entity.HasIndex(e => new { e.UserId, e.AssignedAt });
                entity.HasIndex(e => new { e.RoleId, e.AssignedAt });

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.CommunityId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.UpdatedAt);
                entity.HasIndex(e => new { e.UserId, e.CreatedAt });
                entity.HasIndex(e => new { e.CommunityId, e.CreatedAt });

                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Posts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Community>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.UserId, e.CreatedAt });
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Communities)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserCommunity>(entity =>
            {
                entity.HasKey(uc => new { uc.UserId, uc.CommunityId });
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CommunityId);
                entity.HasIndex(e => e.JoinedDate);

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserCommunities)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cy => cy.Community)
                    .WithMany(c => c.UserCommunities)
                    .HasForeignKey(cy => cy.CommunityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CommentedAt);
                entity.HasIndex(e => e.LastUpdatedAt);
                entity.HasIndex(e => new { e.UserId, e.CommentedAt });
                entity.HasIndex(e => new { e.UserId, e.LastUpdatedAt });
                entity.HasIndex(e => new { e.PostId, e.CommentedAt });
                entity.HasIndex(e => new { e.PostId, e.LastUpdatedAt });
                entity.HasIndex(e => new { e.PostId, e.IsDeleted });
                entity.HasIndex(e => e.IsDeleted);
                entity.Property(e => e.Content).HasMaxLength(2000).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Comments)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Post)
                   .WithMany(e => e.Comments)
                   .HasForeignKey(e => e.PostId)
                   .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Reaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => new { e.UserId, e.PostId, e.ReactionId }).IsUnique();
                entity.HasIndex(e => new { e.UserId, e.ReactedAt });
                entity.HasIndex(e => new { e.PostId, e.ReactedAt });

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Reactions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Post)
                    .WithMany(e => e.Reactions)
                    .HasForeignKey(e => e.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<RecentViewedPost>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.PostId }).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.PostId);
                entity.HasIndex(e => e.ViewedAt);
                entity.HasIndex(e => new { e.UserId, e.ViewedAt });
                entity.HasIndex(e => new { e.PostId, e.ViewedAt });

                entity.HasOne(e => e.User)
                    .WithMany(e => e.RecentViewedPosts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Post)
                    .WithMany(e => e.RecentViewedPosts)
                    .HasForeignKey(e => e.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Administrator with full access" },
                new Role { Id = 2, Name = "User", Description = "Regular user with limited access" }
            );

        }
    }
}   
