using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisitsRepo.Models
{
    [DataContract]
    public class StateDTO
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Abbr { get; set; }
        [DataMember]
        public DateTime Updated { get; set; }
        [DataMember]
        public int visitedCnt { get; set; }
        [DataMember]
        public List<CityDTO> Cities { get; set; }
    }

    [DataContract]
    public class CityDTO
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int visitedCnt { get; set; }
    }
}