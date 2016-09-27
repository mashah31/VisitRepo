using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Spatial;

using VisitsRepo.Models;
using System.Data.Entity.Validation;

namespace VisitsRepo.Controllers
{
    [RoutePrefix("api/v1/user")]
    public class UserController : ApiController
    {
        /// <summary>
        /// Get List of Users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetUsers([FromUri]PaginationDTO pagination)
        {
            try
            {
                pagination = Utils.checkPaginationVal(pagination);

                //Hide users; If CreatedDate < 5 Years && LastAccessDate > 1 year
                DateTime lastAccessDateCeeling = DateTime.Now.AddYears(-1);

                using (UserVisitsContext db = new UserVisitsContext())
                {
                    var users = (from user in db.Users
                                 where user.status == (int)Status.verified &&
                                        user.lastaccesstime < lastAccessDateCeeling
                                 orderby user.lastaccesstime descending
                                 select new UsersDTO
                                 {
                                     Name = user.firstname + " " + user.lastname,
                                     UserName = user.username,
                                     Recent = (from visit in db.Visits
                                               join city in db.Cities on visit.cityid equals city.id
                                               join state in db.States on city.stateid equals state.id
                                               where visit.userid == user.id
                                               orderby visit.visited descending
                                               select new UserVisitDTO()
                                               {
                                                   Visited = visit.visited,
                                                   City = city.name,
                                                   CityID = city.id,
                                                   State = state.name,
                                                   Latitude = visit.latitude,
                                                   Longitude = visit.longitude
                                               }).FirstOrDefault(),
                                     CreatedAt = user.added,
                                     LastUpdateTime = user.lastaccesstime
                                 })
                                 .Skip(pagination.PageSize * pagination.PageNumber).Take(pagination.PageSize).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, users);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Get List of Visits by UserName
        /// </summary>
        /// <param name="username">User name</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{username}/visits")]
        public HttpResponseMessage GetVisitsByUser(string username, [FromUri]PaginationDTO pagination)
        {
            try
            {
                pagination = Utils.checkPaginationVal(pagination);
                DateTime dateFrom = DateTime.Now.AddYears(-1);

                if (string.IsNullOrEmpty(username))
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid value provided for username.");
                }

                using (UserVisitsContext db = new UserVisitsContext())
                {
                    var userObj = db.Users.FirstOrDefault(us => us.username == username && us.status == (int)Status.verified);
                    if (userObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Provided account doesn't exists or valid system!!!");
                    }

                    var visitsObj = (from visit in db.Visits
                                     join city in db.Cities on visit.cityid equals city.id
                                     join state in db.States on city.stateid equals state.id
                                     where 
                                        visit.userid == userObj.id && 
                                        visit.visited > dateFrom   // Get lit of last years visit .. 
                                     orderby visit.visited descending
                                     select new UserVisitDTO()
                                     {
                                         Visited = visit.visited,
                                         CityID = city.id,
                                         City = city.name,
                                         State = state.name,
                                         Latitude = visit.latitude,
                                         Longitude = visit.longitude
                                     })
                                     .Skip(pagination.PageSize * pagination.PageNumber).Take(pagination.PageSize).ToList();

                    if (visitsObj != null && visitsObj.Count() > 0)
                    {
                        // Get list of friends visited (pinned) area around 50miles in last 1 year
                        var userLoc = DbGeography.PointFromText(string.Format("POINT({0} {1})", visitsObj[0].Longitude, visitsObj[0].Latitude), Utils.SRID_GPS);
                        var nearByFriends = Utils.findNearByFriends(db, userObj.id, userLoc, visitsObj[0].CityID, 10, -1, 50);

                        var uVisitsObj = new UsersDTO()
                        {
                            Name = userObj.firstname + " " + userObj.lastname,
                            UserName = userObj.username,
                            CreatedAt = userObj.added,
                            Recent = visitsObj[0],
                            Visits = visitsObj,
                            LastUpdateTime = userObj.lastaccesstime,
                            NearBy = nearByFriends
                        };

                        return Request.CreateResponse(HttpStatusCode.OK, uVisitsObj);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Visits not found!!");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Find Closest X Friends from Provided Location
        /// </summary>
        /// <param name="closestX">Closest X Friends</param>
        /// <param name="geoLoc">Geo Location</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{closestX}")]
        public HttpResponseMessage FindClosestXUsers(int closestX, [FromUri]GeoLocationDTO geoLoc)
        {
            try
            {
                if (geoLoc == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid location, Please provide lattitude and longitude.");
                }

                var location = DbGeography.PointFromText(string.Format("POINT({0} {1})", geoLoc.Longitude, geoLoc.Latitude), Utils.SRID_GPS);
                using (UserVisitsContext db = new UserVisitsContext())
                {
                    var users = Utils.findNearByFriends(db, -1, location, -1, 10, -1, 50);
                    return Request.CreateResponse(HttpStatusCode.OK, new VisitsDTO()
                    {
                        Message = "Closest Friend List.",
                        NearByVisits = users
                    });
                }
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Post New User
        /// </summary>
        /// <param name="userDTOObj">User DTO Object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public HttpResponseMessage PostUser([FromUri]UserDTO userDTOObj)
        {
            try
            {
                using (UserVisitsContext db = new UserVisitsContext())
                {
                    DateTime dtNow = DateTime.Now;
                    var user = db.Users.FirstOrDefault(us => us.username == userDTOObj.UserName.Trim());
                    if (user != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.Conflict, "Provided username already exists!");
                    }

                    db.Users.Add(new user()
                    {
                        username = userDTOObj.UserName,
                        firstname = userDTOObj.FirstName,
                        lastname = userDTOObj.LastName,
                        status = Status.pending,
                        added = dtNow,
                        lastaccesstime = dtNow
                    });
                    db.SaveChanges();

                    user = db.Users.FirstOrDefault(st => st.username == userDTOObj.UserName);
                    return Request.CreateResponse(HttpStatusCode.OK, new UserDTO()
                    {
                        UserName = user.username,
                        FirstName = user.firstname,
                        LastName = user.lastname
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is DbEntityValidationException)
                {
                    DbEntityValidationException dbEx = (DbEntityValidationException)ex;
                    var msg = string.Empty;
                    foreach (DbEntityValidationResult entityErr in dbEx.EntityValidationErrors)
                    {
                        foreach (DbValidationError error in entityErr.ValidationErrors)
                        {
                            msg += string.Format("Error Property Name {0} : Error Message: {1}\n", error.PropertyName, error.ErrorMessage);
                        }
                    }
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, msg);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Authanticate User
        /// </summary>
        /// <param name="username">User Name</param>
        /// <param name="decision">Status (Approv/Rejected)</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{username}/auth/{decision}")]
        public HttpResponseMessage PutUser(string username, bool decision)
        {
            try            {                DateTime dtNow = DateTime.Now;                user userObj;                using (UserVisitsContext db = new UserVisitsContext())                {                       userObj = db.Users.Where(us => us.username == username.Trim()).FirstOrDefault<user>();                }                if (userObj == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Provided username doesn't exists!");
                }                userObj.status = (decision) ? Status.verified : Status.rejected;                using (UserVisitsContext dbUpd = new UserVisitsContext())
                {
                    dbUpd.Entry(userObj).State = EntityState.Modified;
                    dbUpd.SaveChanges();
                }                return Request.CreateResponse(HttpStatusCode.OK, string.Format("User {0} {1}", username, (decision) ? "is approved." : "is rejected"));            }            catch (Exception ex)            {                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);            }
        }

        /// <summary>
        /// Post Visit Info
        /// </summary>
        /// <param name="username">User Name</param>
        /// <param name="visitDTO">Visit Object</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{username}/visits")]
        public HttpResponseMessage PostVisit(string username, [FromUri]UserVisitDTO visitDTO)
        {
            try
            {
                using (UserVisitsContext db = new UserVisitsContext())
                {
                    DateTime dtNow = DateTime.Now;

                    // Save user information if needed ...
                    var userObj = db.Users.FirstOrDefault(us => us.username == username.Trim());
                    if (userObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Please provide valid username or register!");
                    }

                    // Save state and city information if needed ... 
                    var state = db.States.Include("Cities").FirstOrDefault(st => st.name == visitDTO.State.Trim());
                    if (state == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Request state doesn't exists!");
                    }

                    if (state.cities.FirstOrDefault(c => c.name == visitDTO.City) == null)
                    {
                        db.States.Find(state.id).cities.Add(new city()
                        {
                            name = visitDTO.City,
                            added = dtNow
                        });
                        db.States.Find(state.id).updated = dtNow;
                    }
                    db.SaveChanges();

                    // Save visit information
                    var city = db.Cities.FirstOrDefault(c => c.name == visitDTO.City);
                    var userLoc = DbGeography.PointFromText(string.Format("POINT({0} {1})", visitDTO.Longitude, visitDTO.Latitude), Utils.SRID_GPS);

                    db.Visits.Add(new visits()
                    {
                        userid = userObj.id,
                        cityid = city.id,
                        latitude = visitDTO.Latitude,
                        longitude = visitDTO.Longitude,
                        visited = dtNow,
                        location = userLoc,
                    });
                    db.SaveChanges();

                    // Get list of friends visited (pinned) area around 50miles in last 1 year
                    var nearByFriends = Utils.findNearByFriends(db, userObj.id, userLoc, city.id, 10, -1, 50);

                    return Request.CreateResponse(HttpStatusCode.OK, new VisitsDTO()
                    {
                        Message = "Visit have been registered!!",
                        NearByVisits = nearByFriends
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is DbEntityValidationException)
                {
                    DbEntityValidationException dbEx = (DbEntityValidationException)ex;
                    var msg = string.Empty;
                    foreach (DbEntityValidationResult entityErr in dbEx.EntityValidationErrors)
                    {
                        foreach (DbValidationError error in entityErr.ValidationErrors)
                        {
                            msg += string.Format("Error Property Name {0} : Error Message: {1}\n", error.PropertyName, error.ErrorMessage);
                        }
                    }
                    if (!string.IsNullOrEmpty(msg))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, msg);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Delete User Info
        /// </summary>
        /// <param name="username">User Name</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{username}")]
        public HttpResponseMessage DeleteUser(string username)
        {
            try
            {
                using (UserVisitsContext db = new UserVisitsContext())
                {
                    user userObj = db.Users.Include("Visits")
                        .FirstOrDefault(usr => usr.username == username.Trim());

                    if (userObj != null)
                    {
                        int visitsToRemove = userObj.visits.Count();
                        db.Users.Remove(userObj);
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("User {0} got removed along with {1} visits.", username, visitsToRemove));
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid username provided for deletion.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Delete Improperly Placed Visit
        /// </summary>
        /// <param name="username">User Name</param>
        /// <param name="visitDTO">Visit Object</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{username}/visits")]
        public HttpResponseMessage DeleteVisit(string username, [FromUri]UserVisitDTO visitDTO)
        {
            try
            {
                using (UserVisitsContext db = new UserVisitsContext())
                {
                    user userObj = db.Users.FirstOrDefault(usr => usr.username == username.Trim());
                    if (userObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Please provide valid username or register!");
                    }

                    city cityObj = (from city in db.Cities.Where(c => c.name == visitDTO.City)
                                    join state in db.States.Where(s => s.name == visitDTO.State) on city.stateid equals state.id
                                    select city).FirstOrDefault();

                    if (cityObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid city and state infor provided!");
                    }

                    visits visitObj = db.Visits.FirstOrDefault(visit =>
                        visit.userid == userObj.id && visit.cityid == cityObj.id &&
                        visit.latitude == visitDTO.Latitude && visit.longitude == visitDTO.Longitude &&
                        visit.visited == visitDTO.Visited
                    );

                    if (visitObj != null)
                    {
                        db.Visits.Remove(visitObj);
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("Visit got removed for user {0}", username));
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid visit have been asked for deletion.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
