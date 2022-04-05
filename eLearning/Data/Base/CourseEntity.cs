using eLearning.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Data.Base
{
    public abstract class  CourseEntity
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(70, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 70 chars")]
        public string Name { get; set; }

        [Display(Name = "Position")]
        public short Position { get; set; }

        [Display(Name = "Visible")]
        public bool Visible { get; set; }

        //Relationships

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
    }
}
