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
			// ���p�ʧR���A�R���d���s�P�^�Яd���]�@�֧R��
			modelBuilder.Entity<Comment>()
			.HasMany(x => x.CommentReply)
			.WithRequired(x => x.Comment)
			.WillCascadeOnDelete();

		}
	}
}
