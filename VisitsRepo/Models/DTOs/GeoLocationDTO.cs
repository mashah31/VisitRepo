using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisitsRepo.Models
{
    [DataContract]
    public class GeoLocationDTO
    {
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
    }
}