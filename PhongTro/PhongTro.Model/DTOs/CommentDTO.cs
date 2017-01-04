using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Model.DTOs
{
    public class CommentDTO
    {
        public string CommentId { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PostId { get; set; }
        public string Content { get; set; }
        public DateTime DateComment { get; set; }
    }
}
