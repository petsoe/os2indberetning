// <auto-generated />
namespace Infrastructure.DataAccess.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.9.6")]
    public sealed partial class RemovedPersonIdFromPerson : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(RemovedPersonIdFromPerson));
        
        string IMigrationMetadata.Id
        {
            get { return "201506301413546_RemovedPersonIdFromPerson"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}