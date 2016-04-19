using MySql.Data.Entity;

namespace Infrastructure.DataAccess.Migrations
{
    using Core.DomainModel;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Infrastructure.DataAccess.DataContext>
    {
        public Configuration()
        {
            CodeGenerator = new MySqlMigrationCodeGenerator();
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator());
        }

        protected override void Seed(Infrastructure.DataAccess.DataContext context)
        {
            context.RateTypes.AddOrUpdate(x => x.Id,
                new RateType() { Id = 1, TFCode = "4861", IsBike = false, RequiresLicensePlate = true, Description = "Bil - høj takst" },
                new RateType() { Id = 2, TFCode = "4862", IsBike = false, RequiresLicensePlate = true, Description = "Bil - lav takst" },
                new RateType() { Id = 3, TFCode = "4866", IsBike = true, RequiresLicensePlate = false, Description = "Cykel/Knallert" },
                new RateType() { Id = 4, TFCode = "4873", IsBike = false, RequiresLicensePlate = true, Description = "Anhænger" },
                new RateType() { Id = 5, TFCode = "4871", IsBike = false, RequiresLicensePlate = true, Description = "Bil - høj takst (skattepligtig)" });
        }
    }
}
