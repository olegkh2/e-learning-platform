using eLearning.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 80 chars")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        //Relationships
        public List<Topic> Topics { get; set; }
        public List<Note> Notes { get; set; }
        public List<File> Files { get; set; } 
        public List<Course_Student> Courses_Students { get; set; }
        public List<Exercise> Exercises { get; set; }
    }
}
