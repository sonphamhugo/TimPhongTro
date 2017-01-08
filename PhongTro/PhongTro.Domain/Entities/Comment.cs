using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Domain.Entities
{
    public class Comment
    {
        [Key]
        public Guid CommentID { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; }
        
        public DateTime DateComment { get; set; }
        public Guid PostID { get; set; }
        public string PhongTroUserID { get; set; }

        public virtual Post Post { get; set; }
        public virtual PhongTroUser PhongTroUser { get; set; }

        public Comment()
        {
            DateComment = DateTime.Now;
        }
    }
}
