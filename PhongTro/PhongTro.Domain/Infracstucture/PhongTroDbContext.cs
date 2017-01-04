using Microsoft.AspNet.Identity.EntityFramework;
using PhongTro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Domain.Infracstucture
{
    /// <summary>
    /// Class is responsible to communicate with the database.
    /// </summary>
    public class PhongTroDbContext : IdentityDbContext<PhongTroUser>
    {
        /// <summary>
        /// The constructor is provided with a connection string name 'PhongTro'
        /// </summary>
        public PhongTroDbContext() : base("PhongTro", throwIfV1Schema: false) { }

        public IDbSet<Post> Posts { get; set; }
        public IDbSet<Comment> Comments { get; set; }
        public IDbSet<FavouritePost> FavouritePosts { get; set; }
        public IDbSet<BoardingHouseImage> BoardingHouseImages { get; set; }


        /// <summary>
        /// The static function is called from the Owin Startup class. 
        /// </summary>
        /// <returns>
        /// A new PhongTro's database context.
        /// </returns>
        public static PhongTroDbContext Create()
        {
            return new PhongTroDbContext();
        }
    }
}
