using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Model.DTOs
{
    public class RateDTO
    {
        public string PostId { get; set; }
        public float NumberReviewers { get; set; }
        public float TotalPoint { get; set; }
    }
}
