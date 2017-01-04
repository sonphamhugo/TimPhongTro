using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhongTro.Domain.Entities;
using PhongTro.Domain.Infracstucture;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Domain.Migrations
{
    public class PhongTroInitializer : DropCreateDatabaseIfModelChanges<PhongTroDbContext>
    {
        #region Constants
        // The keys references to values in User.config
        const string KeyUserName = "UserName";
        const string KeyPassword = "Password";
        const string KeyFirstName = "FirstName";
        const string KeyLastName = "LastName";
        const string KeyEmail = "Email";
        const string KeyYearBirth = "YearOfBirth";
        const string KeyMonthBirth = "MonthOfBirth";
        const string KeyDayBirth = "DayOfBirth";
        const string KeyPhone = "Phone";

        // The keys references to values in Role.config
        const string KeyRoleAdmin = "Admin";
        const string KeyRoleLodger = "Lodger";
        const string KeyRoleLandlord = "Landlord";
        #endregion

        protected override void Seed(PhongTroDbContext context)
        {
            var userManager = new UserManager<PhongTroUser>(new UserStore<PhongTroUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // create an administrator (collect infomation from User.config file)
            NameValueCollection userConfig = ConfigurationManager.GetSection("UserConfig") as NameValueCollection;

            var user = new PhongTroUser()
            {
                UserName = userConfig[KeyUserName],
                Email = userConfig[KeyEmail],
                EmailConfirmed = true,
                FirstName = userConfig[KeyFirstName],
                LastName = userConfig[KeyLastName],
                DateOfBirth = new DateTime(int.Parse(userConfig[KeyYearBirth]),
                                           int.Parse(userConfig[KeyMonthBirth]),
                                           int.Parse(userConfig[KeyDayBirth])),
                PhoneNumber = userConfig[KeyPhone]
            };

            userManager.Create(user, userConfig[KeyPassword]);

            // create roles get from Role.config file
            NameValueCollection roleConfig = ConfigurationManager.GetSection("RoleConfig") as NameValueCollection;

            if (roleManager.Roles.Count() == 0)
            {
                roleManager.Create(new IdentityRole { Name = roleConfig[KeyRoleAdmin] });
                roleManager.Create(new IdentityRole { Name = roleConfig[KeyRoleLodger] });
                roleManager.Create(new IdentityRole { Name = roleConfig[KeyRoleLandlord] });
            }

            // assign Admin role to the administrator
            var adminUser = userManager.FindByName(userConfig[KeyUserName]);
            userManager.AddToRoles(adminUser.Id, new string[] { roleConfig[KeyRoleAdmin], "Lodger", "Landlord" });
        }
    }
}
