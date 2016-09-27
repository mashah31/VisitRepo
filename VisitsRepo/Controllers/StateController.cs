using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using VisitsRepo.Models;

namespace VisitsRepo.Controllers
{
    [RoutePrefix("api/v1/state")]
    public class StateController : ApiController
    {
        /// <summary>
        /// Get list of states from Repo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            using (StatesContext db = new StatesContext())
            {
                var statesArr = from state in db.States
                                select new StateDTO()
                                {
                                    Name = state.name,
                                    Cities = state.cities.Select(c => new CityDTO(){
                                        Name = c.name, visitedCnt = -1
                                    }).ToList(),
                                    Abbr = state.abbreviation,
                                    Updated = state.updated
                                };
                return Request.CreateResponse(HttpStatusCode.OK, statesArr.ToList());
            }
        }

        /// <summary>
        /// Get most popular states
        /// </summary>
        /// <param name="topX">topX states to select</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{topX}")]
        public HttpResponseMessage GetTopXPopularStates(int topX)
        {
            try
            {
                using (UserVisitsContext db = new UserVisitsContext())
                {
                    var statesArr = (from c in db.Cities
                                    join s in db.States on c.stateid equals s.id
                                    join v in db.Visits on c.id equals v.cityid into k
                                    orderby k.Count() descending
                                    select new StateDTO() {
                                        Name = s.name,
                                        Abbr = s.abbreviation,
                                        Updated = s.updated,
                                        visitedCnt = k.Count()
                                    }).Take(topX);
                    return Request.CreateResponse(HttpStatusCode.OK, statesArr.ToList());
                }
            }
            catch (Exception ex)            {                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);            }
        }

        /// <summary>
        /// Get list of cities from state
        /// </summary>
        /// <param name="state">State value (Name/Abbreviation)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{state}/cities")]
        public HttpResponseMessage GetCitiesByState(string state)
        {
            try
            {
                using (StatesContext db = new StatesContext())
                {
                    stateValueType type = Utils.checkStateVal(state);
                    if (type == stateValueType.invalid)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "API Call must have state (name/abbreviation) defined, it cannot be an empty or null.");
                    }

                    state stateObj = db.States.Include("Cities")
                        .FirstOrDefault(st => ((type == stateValueType.abbr) ? st.abbreviation : st.name) == state.Trim());

                    if (stateObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new StateDTO()
                    {
                        Name = stateObj.name,
                        Abbr = stateObj.abbreviation,
                        Updated = stateObj.updated,
                        Cities = stateObj.cities.Select(c => new CityDTO() {
                            Name = c.name,
                            visitedCnt = -1
                        }).ToList()
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Get List of Popular Cities
        /// </summary>
        /// <param name="state">State (Name/Abbriviation)</param>
        /// <param name="topX">Top X State to Retrive</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{state}/cities/{topX}")]
        public HttpResponseMessage GetTopXPopularCities(string state, int topX)
        {
            try            {                using (UserVisitsContext db = new UserVisitsContext())                {                    stateValueType type = Utils.checkStateVal(state);                    if (type == stateValueType.invalid)                    {                        return Request.CreateResponse(HttpStatusCode.NotFound, "API Call must have state (name/abbreviation) defined, it cannot be an empty or null.");                    }                    state stateObj = db.States
                            .Where(st => ((type == stateValueType.abbr) ? st.abbreviation : st.name) == state.Trim())
                            .FirstOrDefault();                    if (stateObj == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid State Value Provided.");
                    }                    var citisCnt = (from c in db.Cities
                                       join v in db.Visits on c.id equals v.cityid into g
                                       where c.stateid == stateObj.id
                                       orderby g.Count() descending
                                       select new CityDTO() { Name = c.name, visitedCnt = g.Count() }).Take(topX).ToList();                    return Request.CreateResponse(HttpStatusCode.OK, new StateDTO {
                        Name = stateObj.name,
                        Abbr = stateObj.abbreviation,
                        Updated = stateObj.updated,
                        Cities  = citisCnt
                    });                }            }            catch (Exception ex)            {                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);            }
        }

        /// <summary>
        /// Post new state to Repo
        /// </summary>
        /// <param name="stateDTO">State DO Object</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Post([FromBody]StateDTO stateDTO)
        {
            try
            {
                using (StatesContext db = new StatesContext())
                {
                    DateTime dtNow = DateTime.Now;
                    var state = db.States.Include("Cities").FirstOrDefault(st => st.name == stateDTO.Name.Trim());
                    if (state == null)
                    {
                        db.States.Add(new state()
                        {
                            name = stateDTO.Name,
                            abbreviation = stateDTO.Abbr,
                            added = dtNow,
                            updated = dtNow
                        });
                        db.SaveChanges();

                        state = db.States.FirstOrDefault(st => st.name == stateDTO.Name);
                    }

                    bool citiesUpdated = false;
                    state.cities = (state.cities == null) ? new List<city>() : state.cities;
                    foreach (CityDTO city in stateDTO.Cities)
                    {
                        if (state.cities.FirstOrDefault(c => c.name == city.Name) == null)
                        {
                            db.States.Find(state.id).cities.Add(new city()
                            {
                                name = city.Name,
                                added = dtNow
                            });
                            citiesUpdated = true;
                        }
                    }
                    if (citiesUpdated)
                    {
                        db.States.Find(state.id).updated = dtNow;
                    }
                    db.SaveChanges();

                    state = db.States.FirstOrDefault(st => st.name == stateDTO.Name);
                    return Request.CreateResponse(HttpStatusCode.OK, new StateDTO()
                    {
                        Name = state.name,
                        Abbr = state.abbreviation,
                        Updated = state.updated,
                        Cities = state.cities.Select(c => new CityDTO() {
                            Name = c.name,
                            visitedCnt = -1
                        }).ToList()
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
        /// Delete state from Repo
        /// </summary>
        /// <param name="state">State value (Name/Abbreviation)</param>
        /// <returns></returns>
        [HttpDelete]
        public HttpResponseMessage Delete(string state)
        {
            try
            {
                using (StatesContext db = new StatesContext())
                {
                    stateValueType type = Utils.checkStateVal(state);
                    if (type == stateValueType.invalid)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "API Call must have state (name/abbreviation) defined, it cannot be an empty or null.");
                    }

                    state stateObj = db.States.Include("Cities")
                        .FirstOrDefault(st => ((type == stateValueType.abbr) ? st.abbreviation : st.name) == state.Trim());

                    if (stateObj != null)
                    {
                        int totalCitiesWillBeRemoved = stateObj.cities.Count();
                        db.States.Remove(stateObj);
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, string.Format("State {0} got removed along with {1} cities", state, totalCitiesWillBeRemoved));
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Invalid state provided for deletion.");
                }
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
