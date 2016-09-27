using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Spatial;

using VisitsRepo.Models;

namespace VisitsRepo
{
    public enum Status
    {
        verified = 0,
        pending = 1,
        rejected = 2
    }

    public enum stateValueType
    {
        name = 0,
        abbr = 1,
        invalid = 2
    }

    public class Utils
    {
        public const int SRID_GPS = 4326;
        public const double METERSINMILE = 1609.344;
        
        public static stateValueType checkStateVal(string val)
        {
            if (string.IsNullOrEmpty(val) || val.Trim().Length == 0)
            {
                return stateValueType.invalid;
            }
            if (val.Trim().Length == 2)
            {
                return stateValueType.abbr;
            }
            return stateValueType.name;
        }

        public static PaginationDTO checkPaginationVal(PaginationDTO pageDTO)
        {
            if (pageDTO == null)
            {
                return new PaginationDTO()
                {
                    PageSize = 10,
                    PageNumber = 0
                };
            }
            return pageDTO;
        }

        public static List<VisitDTO>  findNearByFriends(UserVisitsContext db, int currUserID, DbGeography currUserLoc, int cityId, int topXItems, int datesInYear, int proximityInMiles)
        {
            DateTime dtFrom = DateTime.Now.AddYears(datesInYear);
            var proximity = METERSINMILE * proximityInMiles;

            var nearByFriends = (from m_visit in db.Visits
                                 let m_dist = m_visit.location.Distance(currUserLoc)
                                 join m_user in db.Users on m_visit.userid equals m_user.id
                                 where
                                    m_user.id != currUserID && 
                                    (cityId == -1 || (cityId > 0 && m_visit.cityid == cityId)) && 
                                    m_visit.visited > dtFrom && m_dist < proximity
                                 orderby m_dist, m_visit.visited descending
                                 select new VisitDTO()
                                 {
                                     Name = m_user.firstname + " " + m_user.lastname,
                                     Distance = (m_dist.HasValue) ? Math.Round((m_dist.Value / Utils.METERSINMILE), 2) : 0.0,
                                     Visited = m_visit.visited
                                 }).Take(topXItems).ToList();
            return nearByFriends;
        }
    }
}