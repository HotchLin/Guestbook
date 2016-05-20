namespace Guestbook.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CommentReply")]
    public partial class CommentReply
    {
        [Key]
        public int ReplyId { get; set; }

        public int? CommentId { get; set; }

        [StringLength(100)]
        public string ReplyMessage { get; set; }

        public int? MemberId { get; set; }

        public DateTime? CreateTime { get; set; }

        public virtual Comment Comment { get; set; }

        public virtual Member Member { get; set; }
    }
}
