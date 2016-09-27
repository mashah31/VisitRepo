using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace VisitsRepo.Models
{
    [Serializable]
    public class visits
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int userid { get; set; }
        public int cityid { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd:hh-mm}", ApplyFormatInEditMode = true)]
        public DateTime visited { get; set; }
        
        public double latitude { get; set; }
        public double longitude { get; set; }
        public DbGeography location { get; set; }

        public virtual user user { get; set; }
        public virtual city city { get; set; }
    }
}