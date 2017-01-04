using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using PhongTro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PhongTro.Domain.Infracstucture
{
    /// <summary>
    /// Class is reponsible to manage instance of PhongTroUser class.
    /// This class derives from UserManager class, which facilitates managing user in Identity system.
    /// </summary>
    public class PhongTroUserManager : UserManager<PhongTroUser>
    {
        public PhongTroUserManager(IUserStore<PhongTroUser> store, IdentityFactoryOptions<PhongTroUserManager> options) : base(store) { }

        /// <summary>
        /// Create a PhongTroUserManager
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context">The Owin context</param>
        /// <returns>
        /// An instance of PhongTroUserManager
        /// </returns>
        public static PhongTroUserManager Create(IdentityFactoryOptions<PhongTroUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<PhongTroDbContext>();
            var appUserManager = new PhongTroUserManager(new UserStore<PhongTroUser>(appDbContext), options);

            return appUserManager;
        }
    }
}
