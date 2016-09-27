using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisitsRepo.Models
{
    [DataContract]
    public class UserDTO
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string UserName { get; set; }
    }

    [DataContract]
    public class VisitsDTO
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public List<VisitDTO> NearByVisits { get; set; }
    }

    [DataContract]
    public class VisitDTO
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Distance { get; set; }
        [DataMember]
        public DateTime Visited { get; set; }
    }

    [DataContract]
    public class UsersDTO
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public DateTime CreatedAt { get; set; }
        [DataMember]
        public UserVisitDTO Recent { get; set; }
        [DataMember]
        public List<UserVisitDTO> Visits { get; set; }
        [DataMember]
        public List<VisitDTO> NearBy { get; set; }
        [DataMember]
        public DateTime LastUpdateTime { get; set; }
    }

    [DataContract]
    public class UserVisitDTO
    {
        [DataMember]
        public DateTime Visited { get; set; }
        [DataMember]
        public int CityID { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
    }
}