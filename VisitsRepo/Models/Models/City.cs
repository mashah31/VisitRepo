using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitsRepo.Models
{
    public class city
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int stateid { get; set; }

        public string name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd:hh-mm}", ApplyFormatInEditMode = true)]
        public DateTime added { get; set; }

        public virtual state state { get; set; }
    }
}