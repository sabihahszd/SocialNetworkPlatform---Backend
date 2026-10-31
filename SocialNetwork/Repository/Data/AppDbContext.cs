using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }


        public DbSet<AppUser> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserSettings>(entity =>
            {
                entity.HasKey(settings => settings.UserId);

                entity.Property(settings => settings.DefaultPrivacy)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(settings => settings.Theme)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(settings => settings.BlockedUserPublicIds)
                    .IsRequired();

                entity.HasOne(settings => settings.User)
                    .WithOne(user => user.Settings)
                    .HasForeignKey<UserSettings>(settings => settings.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(user => user.PublicId).IsUnique();
                entity.HasIndex(user => user.Email).IsUnique();
                entity.HasIndex(user => user.Username).IsUnique();
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(post => post.PublicId).IsUnique();

                entity.HasOne(post => post.Author)
                    .WithMany(user => user.Posts)
                    .HasForeignKey(post => post.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(post => post.Community)
                    .WithMany(community => community.Posts)
                    .HasForeignKey(post => post.CommunityId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasIndex(comment => comment.PublicId).IsUnique();

                entity.HasOne(comment => comment.Post)
                    .WithMany(post => post.Comments)
                    .HasForeignKey(comment => comment.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(comment => comment.Author)
                    .WithMany()
                    .HasForeignKey(comment => comment.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(comment => comment.Parent)
                    .WithMany(comment => comment.Replies)
                    .HasForeignKey(comment => comment.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Community>(entity =>
            {
                entity.HasIndex(community => community.PublicId).IsUnique();

                entity.HasOne(community => community.Owner)
                    .WithMany()
                    .HasForeignKey(community => community.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CommunityMember>(entity =>
            {
                entity.HasIndex(member => new { member.CommunityId, member.UserId }).IsUnique();

                entity.HasOne(member => member.Community)
                    .WithMany(community => community.Members)
                    .HasForeignKey(member => member.CommunityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(member => member.User)
                    .WithMany(user => user.CommunityMemberships)
                    .HasForeignKey(member => member.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PostCommunity>(entity =>
            {
                entity.HasIndex(link => new { link.PostId, link.CommunityId }).IsUnique();

                entity.HasOne(link => link.Post)
                    .WithMany(post => post.PostCommunities)
                    .HasForeignKey(link => link.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(link => link.Community)
                    .WithMany()
                    .HasForeignKey(link => link.CommunityId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PostLike>(entity =>
            {
                entity.HasIndex(like => new { like.PostId, like.UserId }).IsUnique();

                entity.HasOne(like => like.Post)
                    .WithMany(post => post.Likes)
                    .HasForeignKey(like => like.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(like => like.User)
                    .WithMany()
                    .HasForeignKey(like => like.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Repost>(entity =>
            {
                entity.HasIndex(repost => new { repost.PostId, repost.UserId }).IsUnique();

                entity.HasOne(repost => repost.Post)
                    .WithMany(post => post.Reposts)
                    .HasForeignKey(repost => repost.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(repost => repost.User)
                    .WithMany()
                    .HasForeignKey(repost => repost.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bookmark>(entity =>
            {
                entity.HasIndex(bookmark => new { bookmark.PostId, bookmark.UserId }).IsUnique();

                entity.HasOne(bookmark => bookmark.Post)
                    .WithMany(post => post.Bookmarks)
                    .HasForeignKey(bookmark => bookmark.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bookmark => bookmark.User)
                    .WithMany()
                    .HasForeignKey(bookmark => bookmark.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasIndex(report => report.PublicId).IsUnique();

                entity.HasOne(report => report.Post)
                    .WithMany(post => post.Reports)
                    .HasForeignKey(report => report.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(report => report.Reporter)
                    .WithMany()
                    .HasForeignKey(report => report.ReporterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasIndex(follow => new { follow.FollowerId, follow.FollowingId }).IsUnique();

                entity.HasOne(follow => follow.Follower)
                    .WithMany()
                    .HasForeignKey(follow => follow.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(follow => follow.Following)
                    .WithMany()
                    .HasForeignKey(follow => follow.FollowingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.HasIndex(friendship => new { friendship.UserId, friendship.FriendId }).IsUnique();

                entity.HasOne(friendship => friendship.User)
                    .WithMany()
                    .HasForeignKey(friendship => friendship.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(friendship => friendship.Friend)
                    .WithMany()
                    .HasForeignKey(friendship => friendship.FriendId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FriendRequest>(entity =>
            {
                entity.HasIndex(request => new { request.FromUserId, request.ToUserId }).IsUnique();

                entity.HasOne(request => request.FromUser)
                    .WithMany()
                    .HasForeignKey(request => request.FromUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(request => request.ToUser)
                    .WithMany()
                    .HasForeignKey(request => request.ToUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChatThread>(entity =>
            {
                entity.HasIndex(thread => thread.PublicId).IsUnique();
                entity.HasIndex(thread => new { thread.UserAId, thread.UserBId }).IsUnique();

                entity.HasOne(thread => thread.UserA)
                    .WithMany()
                    .HasForeignKey(thread => thread.UserAId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(thread => thread.UserB)
                    .WithMany()
                    .HasForeignKey(thread => thread.UserBId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasIndex(message => message.PublicId).IsUnique();

                entity.HasOne(message => message.Thread)
                    .WithMany(thread => thread.Messages)
                    .HasForeignKey(message => message.ThreadId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(message => message.Author)
                    .WithMany()
                    .HasForeignKey(message => message.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(notification => notification.PublicId).IsUnique();

                entity.HasOne(notification => notification.Recipient)
                    .WithMany(user => user.Notifications)
                    .HasForeignKey(notification => notification.RecipientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(notification => notification.Actor)
                    .WithMany()
                    .HasForeignKey(notification => notification.ActorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SiteSettings>();
        }
    }
}
