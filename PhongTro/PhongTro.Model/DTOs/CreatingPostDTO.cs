using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Model.DTOs
{
    public class CreatingPostDTO
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Maximum number of lodgers")]
        public int NumberLodgers { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Images")]
        public string[] Images { get; set; }
        
        public string PhongTroUserID { get; set; }
    }
}
