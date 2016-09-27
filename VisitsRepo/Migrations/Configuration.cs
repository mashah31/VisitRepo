namespace VisitsRepo.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using VisitsRepo.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<UserVisitsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UserVisitsContext context)
        {

            context.Users.AddRange(new List<user>() {
                new user()
                {
                    firstname = "John", lastname = "Dale", username = "jdale", status = Status.verified,
                    added = new DateTime(2012, 06, 19), lastaccesstime = new DateTime(2015, 09, 25)
                },
                new user()
                {
                    firstname = "Shaun", lastname = "Marsh", username = "smarsh", status = Status.verified,
                    added = new DateTime(2012, 06, 19), lastaccesstime = new DateTime(2015, 09, 25)
                },
                new user()
                {
                    firstname = "Kyle", lastname = "Barnes", username = "kyleb", status = Status.verified,
                    added = new DateTime(2012, 06, 19), lastaccesstime = new DateTime(2015, 09, 25)
                }
            });

            context.States.AddRange(
                new List<state>()
                {
                    new state()
                    {
                        name = "Alabama", abbreviation = "AL",
                        added = new DateTime(2015, 09, 25), updated = new DateTime(2015, 09, 25),
                        cities = new List<city>()
                        {
                            new city() { name = "Akron", added = new DateTime(2015, 09, 25) },
                            new city() { name = "Huntsville", added = new DateTime(2015, 09, 25) },
                            new city() { name = "Birmingham", added = new DateTime(2015, 09, 25) },
                            new city() { name = "Somerville", added = new DateTime(2015, 09, 25) }
                        }
                    },
                    new state()
                    {
                        name = "Alaska", abbreviation = "AK",
                        added = new DateTime(2015, 09, 25), updated = new DateTime(2015, 09, 25),
                        cities = new List<city>()
                        {
                            new city() { name = "Adak", added = new DateTime(2015, 09, 25) },
                            new city() { name = "Akhiok", added = new DateTime(2015, 09, 25) }
                        }
                    }
                }
            );

            context.SaveChanges();

            var stateDict = context.States.ToDictionary(s => s.id, s => s.abbreviation);
            var usersDict = context.Users.ToDictionary(u => u.username, u => u.id);
            var cityDict = context.Cities.ToDictionary(c => string.Format("{0}_{1}", c.name, stateDict[c.stateid]), c => c.id);

            context.Visits.AddRange(new List<visits>()
            {
                new visits()
                {
                    userid = usersDict["jdale"], cityid = cityDict["Akron_AL"],
                    latitude = 32.87802, longitude = -87.743989,
                    location = DbGeography.PointFromText(string.Format("POINT({0} {1})", -87.743989, 32.87802), Utils.SRID_GPS),
                    visited = new DateTime(2015, 09, 25, 16, 05, 01)
                },
                new visits()
                {
                    userid = usersDict["jdale"], cityid = cityDict["Huntsville_AL"],
                    latitude =  34.729135, longitude = -86.584979,
                    location = DbGeography.PointFromText(string.Format("POINT({0} {1})", -86.584979, 34.729135), Utils.SRID_GPS),
                    visited = new DateTime(2015, 09, 16, 16, 05, 01)
                },
                new visits()
                {
                    userid = usersDict["jdale"], cityid = cityDict["Adak_AK"],
                    latitude =  51.8614265, longitude = -176.6380334,
                    location = DbGeography.PointFromText(string.Format("POINT({0} {1})", -176.6380334, 51.8614265), Utils.SRID_GPS),
                    visited = new DateTime(2015, 10, 01, 16, 05, 01)
                },
                new visits()
                {
                    userid = usersDict["smarsh"], cityid = cityDict["Adak_AK"],
                    latitude =  51.88001, longitude = -176.657569,
                    location = DbGeography.PointFromText(string.Format("POINT({0} {1})", -176.657569, 51.88001), Utils.SRID_GPS),
                    visited = new DateTime(2015, 10, 01, 18, 05, 01)
                },
                new visits()
                {
                    userid = usersDict["smarsh"], cityid = cityDict["Akhiok_AK"],
                    latitude =  51.88001, longitude = -176.657569,
                    location = DbGeography.PointFromText(string.Format("POINT({0} {1})", -154.169998, 56.945599), Utils.SRID_GPS),
                    visited = new DateTime(2015, 09, 25, 15, 05, 01)
                },
                new visits()
                {
                    userid = usersDict["smarsh"], cityid = cityDict["Somerville_AL"],
                    latitude =  34.471155, longitude = -86.799394,
                    location = DbGeography.PointFromText(string.Format("POINT({0} {1})", -86.799394, 34.471155), Utils.SRID_GPS),
                    visited = new DateTime(2015, 10, 03, 15, 05, 01)
                },
                new visits()
                {
                    userid = usersDict["smarsh"], cityid = cityDict["Birmingham_AL"],
                    latitude =  33.520295, longitude = -86.811504,
                    location = DbGeography.PointFromText(string.Format("POINT({0} {1})", -86.811504, 33.520295), Utils.SRID_GPS),
                    visited = new DateTime(2015, 10, 06, 15, 05, 01)
                },
                new visits()
                {
                    userid = usersDict["kyleb"], cityid = cityDict["Somerville_AL"],
                    latitude =  34.471155, longitude = -86.799394,
                    location = DbGeography.PointFromText(string.Format("POINT({0} {1})", -86.799394, 34.471155), Utils.SRID_GPS),
                    visited = new DateTime(2015, 10, 02, 15, 05, 01)
                }
            });
        }
    }
}
