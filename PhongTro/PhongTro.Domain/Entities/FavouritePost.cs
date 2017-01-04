using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Domain.Entities
{
    public class FavouritePost
    {
        [Key]
        public Guid FavouritePostID { get; set; }

        [Required]
        public string PhongTroUserID { get; set; }

        [Required]
        public Guid PostID { get; set; }

        public virtual PhongTroUser PhongTroUser { get; set; }
        public virtual Post Post { get; set; }
    }
}
