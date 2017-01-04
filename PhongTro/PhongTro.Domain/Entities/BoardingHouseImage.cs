using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Domain.Entities
{
    public class BoardingHouseImage
    {
        [Key]
        public Guid BoardingHouseImageID { get; set; }
        
        [Required]
        public string Url { get; set; }

        public Guid PostID { get; set; }

        public virtual Post Post { get; set; }
    }
}
