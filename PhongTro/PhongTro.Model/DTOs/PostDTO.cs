using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Model.DTOs
{
    /// <summary>
    /// Contains a post data returned to the client
    /// </summary>
    public class PostDTO
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public double Price { get; set; }
        public int NumberLodgers { get; set; }
        public string Description { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public float TotalPoint { get; set; }
        public float NumberReviewers { get; set; }
        public string PhongTroUserID { get; set; }
    }
}
