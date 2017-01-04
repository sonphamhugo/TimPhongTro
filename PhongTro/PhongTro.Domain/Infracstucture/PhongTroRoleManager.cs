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
    /// Class is reponsible for manage instance of Role class
    /// </summary>
    public class PhongTroRoleManager : RoleManager<IdentityRole>
    {
        public PhongTroRoleManager(IRoleStore<IdentityRole, string> roleStore, IdentityFactoryOptions<PhongTroRoleManager> options) 
            : base(roleStore)
        { }

        ///// <summary>
        ///// Method create a new PhongTroRoleManager from app context
        ///// </summary>
        ///// <param name="options">Identity Factory options</param>
        ///// <param name="context">The Owin context</param>
        ///// <returns></returns>
        public static PhongTroRoleManager Create(IdentityFactoryOptions<PhongTroRoleManager> options, IOwinContext context)
        {
            var appRoleManager = new PhongTroRoleManager(new RoleStore<IdentityRole>(context.Get<PhongTroDbContext>()), options);

            return appRoleManager;
        }
    }
}
