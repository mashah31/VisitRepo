using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisitsRepo.Models
{
    [DataContract]
    public class PaginationDTO
    {
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public int PageNumber { get; set; }
    }
}