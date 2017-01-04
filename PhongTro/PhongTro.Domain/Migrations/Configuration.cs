namespace PhongTro.Domain.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PhongTro.Domain.Infracstucture.PhongTroDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(PhongTro.Domain.Infracstucture.PhongTroDbContext context)
        {
            
        }
    }
}
