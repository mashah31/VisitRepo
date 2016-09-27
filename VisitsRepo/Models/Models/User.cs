using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace VisitsRepo.Models
{
    public class user
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string username { get; set; }

        public string firstname { get; set; }
        public string lastname { get; set; }

        public Status status { get; set; }

        public DateTime added { get; set; }
        public DateTime lastaccesstime { get; set; }

        public virtual ICollection<visits> visits { get; set; }
    }
}