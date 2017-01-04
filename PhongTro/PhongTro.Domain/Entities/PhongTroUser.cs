using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Domain.Entities
{
    /// <summary>
    /// Class represents a user want to register in the PhongTro's membership system. 
    /// This class is extends from the IdentityUser class in order to add some specific 
    /// data properties for the user.
    /// </summary>
    public class PhongTroUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<FavouritePost> FavouritePosts { get; set; }

        /// <summary>
        /// Helper method collects the user identity (including roles and claims).
        /// </summary>
        /// <param name="manager">Application user mananger</param>
        /// <param name="authenticationType">String represent authentication type</param>
        /// <returns>A ClaimsIdentity object</returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<PhongTroUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            
            return userIdentity;
        }
    }
}
