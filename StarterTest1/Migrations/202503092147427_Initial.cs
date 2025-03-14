namespace WindowsFormsApp1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.People",
                c => new
                    {
                        IdPeople = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        FirstName = c.String(maxLength: 50),
                        LastName = c.String(maxLength: 50),
                        SurName = c.String(maxLength: 50),
                        City = c.String(maxLength: 50),
                        Country = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.IdPeople);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.People");
        }
    }
}
