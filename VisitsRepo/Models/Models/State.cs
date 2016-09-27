using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace VisitsRepo.Models
{
    public class state
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }
        public string abbreviation { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd:hh-mm}", ApplyFormatInEditMode = true)]
        public DateTime added { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd:hh-mm}", ApplyFormatInEditMode = true)]
        public DateTime updated { get; set; }

        public virtual ICollection<city> cities { get; set; }
    }
}