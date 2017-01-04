using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Model.DTOs
{
    public class CreatingCommentDTO
    {
        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}
