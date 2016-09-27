namespace VisitsRepo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.cities",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        stateid = c.Int(nullable: false),
                        name = c.String(),
                        added = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.states", t => t.stateid, cascadeDelete: true)
                .Index(t => t.stateid);
            
            CreateTable(
                "dbo.states",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        abbreviation = c.String(),
                        added = c.DateTime(nullable: false),
                        updated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.users",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        username = c.String(),
                        firstname = c.String(),
                        lastname = c.String(),
                        status = c.Int(nullable: false),
                        added = c.DateTime(nullable: false),
                        lastaccesstime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.visits",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userid = c.Int(nullable: false),
                        cityid = c.Int(nullable: false),
                        visited = c.DateTime(nullable: false),
                        latitude = c.Double(nullable: false),
                        longitude = c.Double(nullable: false),
                        location = c.Geography(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.cities", t => t.cityid, cascadeDelete: true)
                .ForeignKey("dbo.users", t => t.userid, cascadeDelete: true)
                .Index(t => t.userid)
                .Index(t => t.cityid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.visits", "userid", "dbo.users");
            DropForeignKey("dbo.visits", "cityid", "dbo.cities");
            DropForeignKey("dbo.cities", "stateid", "dbo.states");
            DropIndex("dbo.visits", new[] { "cityid" });
            DropIndex("dbo.visits", new[] { "userid" });
            DropIndex("dbo.cities", new[] { "stateid" });
            DropTable("dbo.visits");
            DropTable("dbo.users");
            DropTable("dbo.states");
            DropTable("dbo.cities");
        }
    }
}
