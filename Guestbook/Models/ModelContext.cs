namespace Guestbook.Models
{
	using System;
	using System.Data.Entity;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;

	public partial class ModelContext : DbContext
	{
		public ModelContext()
			: base("name=ModelContext")
		{
		}

		public virtual DbSet<Comment> Comment { get; set; }
		public virtual DbSet<CommentReply> CommentReply { get; set; }
		public virtual DbSet<Member> Member { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// 關聯性刪除，刪除留言連同回覆留言也一併刪除
			modelBuilder.Entity<Comment>()
			.HasMany(x => x.CommentReply)
			.WithRequired(x => x.Comment)
			.WillCascadeOnDelete();

		}
	}
}
